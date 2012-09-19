using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ecslent2D.Graphics.Text;

namespace KinectSnake
{
    public class DiagnosticManager : DrawableGameComponent
    {
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Text framerateString;
        bool render = false;

        public DiagnosticManager(Game game)
            : base(game)
        {
        }

        new public void LoadContent()
        {
            framerateString = new Text("", new Vector2(200, 100));
        }
        public override void Update(GameTime gameTime)
        {
            //if (Keyboard.IsNewKeyPress(Keys.D))
                //render = !render;
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            framerateString.text = string.Format("fps: {0}", frameRate);

            if (render)
            {
                //Main.renderManager.ScheduleRender(framerateString);
            }
        }
    }
}
