using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ecslent2D.Collision;
using Ecslent2D.Graphics;
using Ecslent2D.ScreenManagement;

namespace KinectSnake
{
    class DrawableGrid:Grid
    {
        protected DrawableAsset<RenderTarget2D> visualGrid;
        protected List<RenderTarget2D> visualGridSprites;
        protected RenderTarget2D swapTarget;
        public DrawableGrid(int startfocus = 3)
            :base(startfocus,Main.windowSize)
        {
            swapTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Main.windowSize.X, (int)Main.windowSize.Y);
            visualGridSprites = new List<RenderTarget2D>();
            for (int f = 0; f <= MAX_FOCUS; f++)
            {
                int w = (int)(Main.graphics.GraphicsDevice.Viewport.AspectRatio * 5) * (int)Math.Pow(2, f);
                int h = (int)(5 * (int)Math.Pow(2, f));
                Vector2 gSize = new Vector2(Main.screenRect.Width / w, Main.screenRect.Height / h);

                visualGridSprites.Add(new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Main.windowSize.X, (int)Main.windowSize.Y));

                Main.graphics.GraphicsDevice.SetRenderTarget(visualGridSprites[f]);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                ScreenManager.spriteBatch.Begin();
                for (int i = 0; i < w; i++)
                    for (int j = 0; j < h; j++)
                        ScreenManager.spriteBatch.Draw(Main.sprites["gridSquare"], new Rectangle((int)(i * gSize.X), (int)(j * gSize.Y), (int)gSize.X, (int)gSize.Y), Color.White);


                ScreenManager.spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            }
        }
        public void Draw(SpriteBatch batch)
        {
            if (enabled && render)
            {
                Main.graphics.GraphicsDevice.SetRenderTarget(swapTarget);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                batch.End();
                batch.Begin();
                batch.Draw(visualGridSprites[focus], Main.screenRect, Color.White);

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        if (grid[i, j].inhabitants.Count > 0)
                        {
                            batch.Draw(Main.sprites["filledGrid"], new Rectangle((int)(i * gridSize.X), (int)(j * gridSize.Y), (int)gridSize.X, (int)gridSize.Y), (grid[i, j].inhabitants[0] as IDrawableGridAsset).gridColor);
                        }

                    }
                batch.End();
                batch.Begin();

                Main.graphics.GraphicsDevice.SetRenderTarget(null);
                visualGrid = new DrawableAsset<RenderTarget2D>(new Vector2(Main.windowSize.X / 2, Main.windowSize.Y / 2), new Vector2(0, -1), Main.windowSize, swapTarget);
                visualGrid.Draw(batch);
            }
        }
    }
}
