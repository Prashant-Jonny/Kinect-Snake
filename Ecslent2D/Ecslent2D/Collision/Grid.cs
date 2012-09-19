using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ecslent2D.Graphics;
using Ecslent2D.ScreenManagement;

namespace Ecslent2D.Collision
{
    public class GridSquare
    {
        public List<IGridAsset> inhabitants;

        public GridSquare()
        {
            inhabitants = new List<IGridAsset>();
        }
    }

    public class C
    {
        public static int numCollisions = 0;
    }
    public class CollisionHandler
    {
        public IGridAsset actuator;
        public List<Type> conditions = null;
        public delegate void CollisionEventHandler(IGridAsset participants, Point gridPos);
        public event CollisionEventHandler action;
        public float cooldown = 2.0f;
        public bool ignoreOwnType = false;
        public CollisionHandler(IGridAsset act, CollisionEventHandler a)
        {
            actuator = act;
            action = a;
        }
        public void Update(GameTime gTime)
        {
            cooldown -= gTime.ElapsedGameTime.Ticks/1000000.0f;
        }
        public void PerformAction(IGridAsset participant, Point point)
        {
            cooldown = 2.0f;
           action(participant, point);
            
        }
    }

    public class Grid
    {
        protected int MAX_FOCUS = 4;
        protected GridSquare[,] grid;
        protected Vector2 gridSize;
        protected int width, height;
        protected bool enabled = false;
        protected int focus;
        public List<CollisionHandler> collisionHandlers;
        public bool render = false;
        protected Vector2 screenSize;
	    public Grid(int startFocus, Vector2 screenSize)
	    {
            this.screenSize = screenSize;
            collisionHandlers = new List<CollisionHandler>();
            focus = startFocus;
            LoadFocus(focus);
	    }
        private void LoadFocus(int newFocus)
        {
            width = (int)((screenSize.X/screenSize.Y) * 5) * (int)Math.Pow(2, newFocus);
            height = (int)(5 * (int)Math.Pow(2, newFocus));

            gridSize = new Vector2(screenSize.X / width, screenSize.Y / height);
            grid = new GridSquare[width, height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    grid[i, j] = new GridSquare();
            enabled = true;
            focus = newFocus;
        }
        public Vector2 GetGridSize()
        {
            return gridSize;
        }
        public void Reset()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    grid[i, j].inhabitants.Clear();
                }
            collisionHandlers.Clear();
        }
        public Point WorldToGrid(Vector2 worldPos)
        {
            return new Point((int)(worldPos.X / gridSize.X), (int)(worldPos.Y / gridSize.Y));
        }

        public Vector2 GridToWorld(Point gPos)
        {
            return new Vector2((gPos.X * gridSize.X) + gridSize.X / 2, (gPos.Y * gridSize.Y) + gridSize.Y / 2);
        }

        
        public void Enlarge()
        {
            if (focus < MAX_FOCUS)
                this.LoadFocus(focus +1);
        }
        public void Shrink()
        {
            if (focus > 0)
                this.LoadFocus(focus - 1);

        }
        public void Add(IGridAsset ent)
        {
            if (!enabled)
                return;


            CoarseGridFill(ent);
        }
        public void Add(IGridAssetCongolmerate ent)
        {
            if (!enabled)
                return;

            foreach (IGridAsset member in ent.members)
                CoarseGridFill(member,ent);
        }
        private void CoarseGridFill(IGridAsset ent, IGridAssetCongolmerate conglomerate = null)
        {
            IGridAsset toFillGrid = ent;
            if (conglomerate != null)
                toFillGrid = conglomerate;

            for (int i = (int)(ent.boundry.Left / gridSize.X); i <= (int)(ent.boundry.Right / gridSize.X); i++)
            {
                for (int j = (int)(ent.boundry.Top / gridSize.Y); j <= (int)(ent.boundry.Bottom / gridSize.Y); j++)
                {
                    Point gridPos = new Point(i, j);
                    if (GridPosValid(gridPos))
                    {
                        if (!grid[gridPos.X, gridPos.Y].inhabitants.Contains(toFillGrid))
                        {
                            toFillGrid.gridPositions.Add(gridPos);
                            grid[gridPos.X, gridPos.Y].inhabitants.Add(toFillGrid);
                            if (grid[gridPos.X, gridPos.Y].inhabitants.Count > 1)
                                HandleCollision(grid[gridPos.X, gridPos.Y].inhabitants,gridPos);
                        }
                    }
                }
            }
        }
        /*private void FineGridFill(IGridAsset ent)
        {

            RenderTarget2D rotatedColorMap = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)ent.Size.X, (int)ent.Size.Y);
            ScreenManager.spriteBatch.Begin();
            Main.graphics.GraphicsDevice.SetRenderTarget(rotatedColorMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            ent.Draw(ScreenManager.spriteBatch);

            ScreenManager.spriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Color[] entData;
            entData = new Color[rotatedColorMap.Width * rotatedColorMap.Height];
            rotatedColorMap.GetData(entData);

            Point lastEnd = new Point(0, 0);
            for (int i = (int)((ent.Position.X - (rotatedColorMap.Width * ent.scale / 2)) / gridSize.X); i <= (int)((ent.Position.X + (rotatedColorMap.Width * ent.scale / 2)) / gridSize.X); i++)
            {
                Point stepSize = new Point((int)gridSize.X, (int)gridSize.Y);
                if (lastEnd.X == 0)
                    stepSize.X = (i + 1) * (int)gridSize.X - (int)((ent.Position.X - (rotatedColorMap.Width * ent.scale / 2)));

                for (int j = (int)((ent.Position.Y - (rotatedColorMap.Height * ent.scale / 2)) / gridSize.Y); j <= (int)((ent.Position.Y + (rotatedColorMap.Height * ent.scale / 2)) / gridSize.Y); j++)
                {
                    Point gridPos = new Point(i, j);
                    bool gridChecked = false;
                    if (lastEnd.Y == 0)
                        stepSize.Y = (j + 1) * (int)gridSize.Y - (int)((ent.Position.Y - (rotatedColorMap.Height * ent.scale / 2)));
                    else
                        stepSize.Y = (int)gridSize.Y;

                    if (GridPosValid(gridPos) && !grid[gridPos.X, gridPos.Y].inhabitants.Contains(ent))
                    {

                        for (int x = lastEnd.X; x < (int)Math.Min(lastEnd.X + stepSize.X, rotatedColorMap.Width); x++)
                        {
                            for (int y = lastEnd.Y; y < (int)Math.Min(lastEnd.Y + stepSize.Y, rotatedColorMap.Height); y++)
                            {
                                if (entData[x + y * rotatedColorMap.Width].A > 200 )
                                {
                                    gridChecked = true;
                                    ent.gridPositions.Add(gridPos);
                                    grid[gridPos.X, gridPos.Y].inhabitants.Add(ent);
                                    if (grid[gridPos.X, gridPos.Y].inhabitants.Count > 1)
                                        HandleCollision(grid[gridPos.X, gridPos.Y].inhabitants,gridPos);
                                    break;
                                }
                            }
                            if (gridChecked)
                                break;
                        }
                        lastEnd = new Point((int)lastEnd.X, (int)stepSize.Y + lastEnd.Y);
                    }
                }
                lastEnd = new Point((int)lastEnd.X + stepSize.X, 0);
            }
        }*/
        private void HandleCollision(List<IGridAsset> participants, Point gPos)
        {

            foreach (CollisionHandler handler in collisionHandlers)
                if (participants.Contains(handler.actuator) && participants.Count > 1 && handler.cooldown <=0)
                    for (int i = 0; i < participants.Count; i++)
                        if (handler.actuator != participants[i])
                            if (handler.conditions == null || handler.conditions.Contains(participants[i].GetType()))
                                handler.PerformAction(participants[i], gPos);

        }
        public void Update(GameTime gTime)
        {
            foreach (CollisionHandler handler in collisionHandlers)
                handler.Update(gTime);
        }
        public void Remove(IGridAsset ent)
        {
            if (!enabled || ent.gridPositions == null)
                return;
            foreach (Point gridPos in ent.gridPositions)
            {
                if (gridPos.X < width && gridPos.Y < height)
                    grid[gridPos.X, gridPos.Y].inhabitants.Remove(ent);
            }
            ent.gridPositions.Clear();
        }

        public bool GridsEmpty(Rectangle rect, bool ignoreOutsideGrids = true)
        {
            if (!enabled)
                return false;

            for (int i = (int)(rect.Left / gridSize.X) ; i <= (int)(rect.Right / gridSize.X) ; i++)
            {
                for (int j = (int)(rect.Top / gridSize.Y) ; j <= (int)(rect.Bottom / gridSize.Y) ; j++)
                {
                    if (GridPosValid(new Point(i, j)))
                    {
                        if (grid[i, j].inhabitants.Count > 0)
                            return false;
                    }
                    else if (!ignoreOutsideGrids)
                        return false;
                }
            }
            return true;
        }
        public bool GridPosValid(Point gPos)
        {
            if (!enabled)
                return false;
            if (gPos.X < 0 || gPos.X > width-1 || gPos.Y < 0 || gPos.Y > height-1)
                return false;
            return true;
        }
        public void UpdateGridInfo(IGridAsset ent)
        {
            if (!enabled)
                return;
            Remove(ent);
            Add(ent);

        }
        public void UpdateGridInfo(IGridAssetCongolmerate ent)
        {
            if (!enabled)
                return;
            Remove(ent);
            Add(ent);

        }
        public GridSquare GetGrid(Point gPos)
        {
            if (!enabled)
                return null;
            if (GridPosValid(gPos))
                return grid[gPos.X, gPos.Y];
            else
                return null;
        }
        public GridSquare GetGrid(Vector2 worldPos)
        {
            if (!enabled)
                return null;
            Point gPos = WorldToGrid(worldPos);
            if (GridPosValid(gPos))
                return grid[gPos.X, gPos.Y];
            else
                return null;
        }
    }
}