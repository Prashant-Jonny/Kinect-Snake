using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ecslent2D.Graphics;
using Ecslent2D.Collision;
namespace KinectSnake
{
    public interface IDrawableGridAsset:IGridAsset
    {
        Color gridColor { get; set; }
    }

    public class Entity<T> : RenderableAsset<T>, IDrawableGridAsset
        where T: Texture2D
    {
        public Vector2 nose, targetPos;
        public List<Point> gridPositions { get; set; }
        public int speed;
        public EffectSprite effectSprite = null;
        public bool dead {get;set;}
        public bool onGrid = true, spawning = false;
        protected bool onGridLastState = true;
        protected float cooldown = 1.0f;
        public Color gridColor{get;set;}
        public CollisionHandler collision { get; set; }
        public Grid grid;
        public Entity(Grid gridPtr,  Vector2 position, Vector2 direction, Vector2 size)
            : base(position, direction, size)
        {
            grid = gridPtr;
            targetPos = position;
            gridColor = Color.White;
            collision = new CollisionHandler(this, this.HandleCollision);
            gridPositions = new List<Point>();
            onGrid = true;
            dead = false; 
            spawning = false;
        }


        public override void DrawColorMap(SpriteBatch batch)
        {
            base.DrawColorMap(batch);
            if (this.effectSprite != null)
                this.effectSprite.Draw(batch);
        }


        public virtual void ScheduleCollisionHandling(List<Type> conditions = null)
        {
            collision.conditions = conditions;
            grid.collisionHandlers.Insert(0, collision);
        }
        public virtual void RemoveFromCollisionHandling()
        {
            grid.collisionHandlers.Remove(collision);
        }
        public virtual void Update(GameTime gTime = null)
        {
            if (!this.dead)
            {
                if (spawning)
                {

                    this.Position += this.Direction * this.speed;
                    if (Math.Abs(this.Position.X - this.targetPos.X) < 10 && Math.Abs(this.Position.Y - this.targetPos.Y) < 10)
                        this.spawning = false;
                }

                if (onGrid)
                    grid.UpdateGridInfo(this);
                else if (onGridLastState != onGrid)
                    grid.Remove(this);

                onGridLastState = onGrid;
            }
            if (this.effectSprite != null)
            {
                this.effectSprite.Update(gTime);
                if (this.effectSprite.IsDone())
                    this.effectSprite = null;
            }
        }
        public virtual void Spawn()
        {
            while (!grid.GridsEmpty(this.boundry))
            {
                float testNum = (float)Main.numGenerator.NextDouble();
                if (testNum < 0.25)
                    this.Position = new Vector2((float)Main.numGenerator.NextDouble() * Main.windowSize.X, 0);
                else if (testNum < 0.5)
                    this.Position = new Vector2((float)Main.numGenerator.NextDouble() * Main.windowSize.X, Main.windowSize.Y);
                else if (testNum < 0.75)
                    this.Position = new Vector2(0, (float)Main.numGenerator.NextDouble() * Main.windowSize.Y);
                else
                    this.Position = new Vector2(Main.windowSize.X, (float)Main.numGenerator.NextDouble() * Main.windowSize.Y);
                this.Direction = Vector2.Normalize(targetPos - this.Position);
                this.boundry = new Rectangle(this.boundry.Left, this.boundry.Top, this.boundry.Width * 2, this.boundry.Height * 2);
            }
            this.spawning = true;
            this.onGrid = true;
            this.ScheduleCollisionHandling();
        }
        public virtual void HandleCollision(IGridAsset other, Point gPos)
        {
            if ( this.spawning)
                this.spawning = false;
        }


        public void ShowEffect(EffectSprite effectSprite)
        {
            if (this.effectSprite != null && this.effectSprite == effectSprite && this.animated)
                this.effectSprite.ColorMap.Play();
            else
            {
                if (this.effectSprite != null)
                    this.effectSprite.Stop();
                this.effectSprite = effectSprite;
                this.effectSprite.Start();
            }

        }

        public virtual void DrawEffectMap(SpriteBatch batch)
        {
            if (this.effectSprite != null)
                this.effectSprite.Draw(batch);
        }
    }
    public class Mouse : Entity<Animation2D>
    {
        public Vector2 shadowPos;
        public bool moving;

        public Mouse(Grid gridPtr, Vector2 position, int speed)
            : base(gridPtr,position, new Vector2(0, -1), new Vector2(80, 80) )
        {
            this.gridColor = Color.Aqua;
            this.animated = true;
            this.SetMaps(Main.animatedSprites["mouse"],Main.animatedSprites["mouse_SPEC"], Main.animatedSprites["mouse_NRM"]);
            this.moving = speed != 0;
            this.speed = speed;
            grid.Add(this);
        }
        public override void Update(GameTime gTime)
        {
            base.Update(gTime);
            if (this.moving )
            {
                if (!Main.screenRect.Contains(new Point((int)nose.X, (int)nose.Y)) && !this.spawning)
                {
                    this.Direction = Vector2.Negate(this.Direction);
                }

                this.Position += this.Direction * this.speed;
                this.nose = this.Position + this.Direction * (this.Size / 2);

            }
            cooldown -= gTime.ElapsedGameTime.Milliseconds / 1000.0f;
        }
        public override void HandleCollision(IGridAsset other, Point gPos)
        {
            Vector2 dirToEnt = Vector2.Normalize(new Vector2(other.Position.X - this.Position.X, other.Position.Y - this.Position.Y));

            if (this.moving && Utilities.AngleBetween(dirToEnt, this.Direction) < (float)Math.PI / 2)
            {
                this.Direction = Vector2.Negate(this.Direction);
            }
            base.HandleCollision(other, gPos);
        }
    }

    public class Rabbit : Entity<Animation2D>
    {
        public Vector2 shadowPos, jumpDir;
        float sightAngle = 3.0f * (float)Math.PI /4.0f;
        int numJumpSteps, stepsTaken, jumpSpeed = 20;
        public bool jumping = false;
        public Rectangle landRect;
        public Rabbit(Grid gridPtr, Vector2 position, int speed)
            : base( gridPtr,position, Vector2.Zero, new Vector2(125, 125))
        {
            this.gridColor = Color.DarkOrange;
            this.animated = true;

            this.SetMaps(Main.animatedSprites["rabbit"],Main.animatedSprites["rabbit_SPEC"], Main.animatedSprites["rabbit_NRM"]);
            this.speed = speed;
            grid.Add(this);
        }
        public override void Update(GameTime gTime)
        {
            if (jumping)
            {
                this.Position += this.jumpDir * this.jumpSpeed;
                this.nose = this.Position + this.Direction * (this.Size / 2);
                stepsTaken++;
                if (stepsTaken < numJumpSteps / 2)
                    this.scale += 0.5f / (numJumpSteps);
                else if (this.scale > 1.0f)
                    this.scale -= 0.5f / (numJumpSteps);

                if (stepsTaken == numJumpSteps)
                {
                    jumping = false;
                    this.onGrid = true;
                    this.scale = 1.0f;
                }
                if (!grid.GridsEmpty(landRect))
                    Jump();
            }
            base.Update(gTime);
        }
        public override void HandleCollision(IGridAsset other, Point gPos)
        {
            if (other.GetType() == typeof(Snake))
            {
                Snake snake = (Snake)other;
                Vector2 dirToSnake = Vector2.Normalize(new Vector2(snake.head.Position.X - this.Position.X, snake.head.Position.Y - this.Position.Y));

                if (Utilities.AngleBetween(dirToSnake, this.Direction) < this.sightAngle)
                {
                    Jump();
                    base.Update(null);
                }
            }
            base.HandleCollision(other, gPos);
        }
        private void Jump()
        {
            bool canJump = false;
            Vector2 landPos = this.Position;
            int iteration = 0;
            int stepSize = jumpSpeed * 4;
            while (!canJump)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if ((i == j && i == 0) /*|| new Vector2(i, j) == ScreenManager.CurrentGame.snake.head.Direction*/)
                            continue;
                        jumpDir = new Vector2(i, j);
                        landPos = this.Position + new Vector2(stepSize * i, stepSize * j) * (iteration + 1);
                        if (Main.screenRect.Contains(new Point((int)landPos.X, (int)landPos.Y)))
                        {
                            landRect = new Rectangle((int)(landPos.X - (this.Size.X / 2)), (int)(landPos.Y - (this.Size.Y / 2)), (int)this.Size.X, (int)this.Size.Y);
                            canJump = grid.GridsEmpty(landRect, false);
                            if (canJump)
                                break;
                        }
                    }
                    if (canJump)
                        break;
                }
                iteration++;
            }
            numJumpSteps = ((iteration + 1) * stepSize) / jumpSpeed;
            stepsTaken = 0;
            jumping = true;
            this.onGrid = false;
        }
    }


    public class BodyPart : Entity<Texture2D>
    {
        public BodyPart parent;
        public int offsetFromParent;
        public bool readyForTurn;

        public BodyPart(Grid gridPtr, Vector2 position, Vector2 direction, Texture2D sprite, Texture2D specular, Texture2D normal, int speed, BodyPart parent = null)
            : base(gridPtr,position, direction, new Vector2(80, 80))
        {
            this.gridColor = Color.DarkMagenta;
            this.speed = speed;
            this.parent = parent;
            this.offsetFromParent = (int)Size.X / 2;
            this.SetMaps(sprite, specular, normal);
            this.onGridLastState = this.onGrid = false;
        }

        public override void Update(GameTime gTime)
        {

            base.Update(gTime);
            if (this.parent != null)
            {
                Vector2 nextPos = Utilities.ScreenWrap(this.Position + this.Direction * this.speed);
                Vector2 parentBackPos = Utilities.ScreenWrap(this.parent.Position - this.parent.Direction * this.offsetFromParent);

                if (Math.Abs(this.parent.Position.X - this.Position.X) > 0.0 && Math.Abs(this.parent.Position.Y - this.Position.Y) > 0.0)
                {
                    this.Position = nextPos;
                    if (this.Position == parentBackPos)
                        this.Direction = this.parent.Direction;

                }
                else if (this.Direction == this.parent.Direction)
                {
                    this.Position = parentBackPos;
                    if (parent.parent == null)
                        parent.readyForTurn = true;
                }
                else
                {
                    this.Position += this.Direction * this.speed;
                    if (parent.parent == null)
                        parent.readyForTurn = false;
                }
            }
        }


    }
    public class Head : BodyPart
    {
        public Animation2D eatingAnimation;
        public Head(Grid gridPtr, Vector2 position, Vector2 direction, int speed)
            : base( gridPtr,position, direction, Main.sprites["snakeHead"], Main.speculars["snakeHead"], Main.normals["snakeHead"], speed, null)
        {
            eatingAnimation = Main.animatedSprites["snakeEating"];
        }

        public override void Update(GameTime gTime)
        {

            base.Update(gTime);
            this.Position += this.Direction * this.speed;
            this.nose = this.Position + this.Direction * (this.Size / 2);
            if (this.animated)
            {
                eatingAnimation.Update(gTime.ElapsedGameTime.Ticks);
                if (eatingAnimation.CurrentFrame == eatingAnimation.FrameCount - 1)
                {
                    this.animated = false;
                    eatingAnimation.Stop();
                }

            }

        }

    }

    public class Tail : BodyPart
    {
        public Tail(Grid gridPtr,Vector2 position, Vector2 direction, int speed, BodyPart parent)
            : base( gridPtr,position, direction, Main.sprites["snakeTail"], Main.speculars["snakeTail"], Main.normals["snakeTail"], speed, parent) { }

    }

    public class Body : BodyPart
    {
        public Body(Grid gridPtr, Vector2 position, Vector2 direction, int speed, BodyPart parent)
            : base( gridPtr,position, direction, Main.sprites["snakeBody"], Main.speculars["snakeBody"], Main.normals["snakeBody"], speed, parent) { }

    }

    public class EntityConglomerate : Entity<RenderTarget2D>, IGridAssetCongolmerate
    {
        public List<IGridAsset> members { get; set; }
        public EntityConglomerate(Grid gridPtr, Vector2 position, Vector2 direction, Vector2 size)
            : base(gridPtr,position, direction, size)
        {
            members = new List<IGridAsset>();
        }
    }
    public class Snake : EntityConglomerate
    {
        public Head head;
        Tail tail;
        Vector2 turnToPerform,nextTurn = Vector2.Zero;
        int length;

        public Snake(Grid gridPtr, Vector2 position, int startSpeed, int startLength)
            : base(gridPtr,position, new Vector2(0, -1), Main.windowSize)
        {
            this.dead = false;
            this.scale = 1.0f;
            this.speed = startSpeed;
            this.length = startLength;
            turnToPerform = new Vector2(0, 0);

            this.SetMaps(new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenRect.Width, Main.screenRect.Height),
                new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenRect.Width, Main.screenRect.Height),
                new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenRect.Width, Main.screenRect.Height));
            this.Position = Main.windowSize * 0.5f;
            for (int i = 0; i < this.length; i++)
            {
                BodyPart newPart;
                if (i == 0)
                {
                    newPart = this.head = new Head(gridPtr,position + new Vector2(0,40) * i, Direction, this.speed);
                }
                else if (i == this.length - 1)
                    newPart = this.tail = new Tail(gridPtr, position + new Vector2(0, 40) * i, Direction, this.speed, this.members[i - 1] as BodyPart);
                else
                    newPart = new Body(gridPtr, position + new Vector2(0, 40) * i, Direction, this.speed, this.members[i - 1] as BodyPart);
                members.Add(newPart);
            }
            grid.Add(this);
        }

        public override void HandleCollision(IGridAsset other, Point gPos)
        {
            Vector2 gridSize = grid.GetGridSize();
            Rectangle Boundry = new Rectangle((int)(this.head.boundry.Left / gridSize.X), (int)(this.head.boundry.Top / gridSize.Y),
                (int)(this.head.boundry.Width / gridSize.X), (int)(this.head.boundry.Height / gridSize.Y));

            if (gPos.X + 1 >= Boundry.Left && gPos.X - 1 <= Boundry.Right && gPos.Y + 1 >= Boundry.Top && gPos.Y - 1 <= Boundry.Bottom)
            {
                other.dead = true;
            }

        }

        public void Enlarge()
        {
            this.members.Insert(this.members.Count - 1, new Body(grid, this.tail.Position, this.tail.Direction, this.tail.speed, this.members[this.members.Count - 2] as BodyPart));
            this.tail.Position += Vector2.Negate(this.tail.Direction) * this.head.speed;
            this.tail.parent = this.members[this.members.Count - 2] as BodyPart;


        }
        public void ChangeDirection(Vector2 newDir)
        {
            if (this.head.readyForTurn && this.head.Direction != Vector2.Negate(newDir))
            {
                if (turnToPerform != new Vector2(0, 0))
                {
                    this.head.Direction = (turnToPerform);
                    turnToPerform = new Vector2(0, 0);
                }
                else
                    this.head.Direction = newDir;
            }
            else if (!this.head.readyForTurn && this.head.Direction != Vector2.Negate(newDir) && this.head.Direction != newDir)
                turnToPerform = newDir;
        }
        public override void Update(GameTime gTime)
        {
            grid.UpdateGridInfo(this);


            if (Main.screenRect.Contains(this.head.boundry) == false)
            {
                if (this.head.Position.X < 0)
                    this.head.Position = new Vector2(this.head.Position.X + Main.windowSize.X, this.head.Position.Y);
                if (this.head.Position.Y < 0)
                    this.head.Position = new Vector2(this.head.Position.X, this.head.Position.Y + Main.windowSize.Y);
                if (this.head.Position.X > Main.windowSize.X)
                    this.head.Position = new Vector2(this.head.Position.X - Main.windowSize.X, this.head.Position.Y);
                if (this.head.Position.Y > Main.windowSize.Y)
                    this.head.Position = new Vector2(this.head.Position.X, this.head.Position.Y - Main.windowSize.Y);
            }
            for (int i = this.members.Count - 1; i >= 0; i--)
            {
                Point headTip = new Point((int)this.head.Position.X + (int)this.head.Direction.X * (int)(this.head.Size.X / 1.5), (int)this.head.Position.Y + (int)this.head.Direction.Y * (int)(this.head.Size.Y / 1.5));
                if (i != 0 && this.members[i].boundry.Contains(headTip))
                    this.dead = true;
                this.members[i].Update(gTime);

            }
        }
        public override void DrawColorMap(SpriteBatch batch)
        {
            for (int i = this.members.Count - 1; i >= 0; i--)
                (this.members[i] as BodyPart).DrawColorMap(batch);
        }
        public override void DrawSpecularMap(SpriteBatch batch)
        {
            for (int i = this.members.Count - 1; i >= 0; i--)
                (this.members[i] as BodyPart).DrawSpecularMap(batch);
        }
        public override void DrawNormalMap(SpriteBatch batch)
        {
            for (int i = this.members.Count - 1; i >= 0; i--)
                (this.members[i] as BodyPart).DrawNormalMap(batch);
        }
        public override void DrawEffectMap(SpriteBatch batch)
        {
            for (int i = this.members.Count - 1; i >= 0; i--)
                if ((this.members[i] as BodyPart).effectSprite != null)
                   (this.members[i] as BodyPart).DrawEffectMap(batch);
        }

    }

}
