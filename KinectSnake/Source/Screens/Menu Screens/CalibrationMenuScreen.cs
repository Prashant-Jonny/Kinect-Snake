#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ecslent2D.Graphics.Text;
using Ecslent2D.Input;
using Ecslent2D.ScreenManagement.Screens;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class CalibrationMenuScreen : MenuScreen
    {
        Texture2D rgbVideoCopy;
        Text[] instructions;
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CalibrationMenuScreen()
            : base("Calibrate Kinect", Color.RoyalBlue)
        {
            AddMenuEntry("Done", Advance);
            instructions = new Text[2];
            instructions[0] = new Text("Move closer/farther away from the screen", new Vector2(Main.windowSize.X * 0.5f, Main.windowSize.Y - 300),1.0f, Color.Gold);
            instructions[1] = new Text("until your head approaches the top of the frame", new Vector2(Main.windowSize.X * 0.5f, Main.windowSize.Y - 200), 1.0f, Color.Gold);
            KinectDependencies.Add(KinectDependency.Color);
            SetHandCursor(Main.sprites["handCursor"], new Vector2(200, 200));
            SetSelectionSprite(Main.animatedSprites["load"], new Vector2(400, 400));
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        /// 
        void Advance(PlayerIndexEventArgs e)
        {
            this.ExitScreen();
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input.Kinect.IsColorCaptureEnabled())
                rgbVideoCopy = input.Kinect.GetLastColorFrame();
            base.HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            foreach (Text ins in instructions)
                ins.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        public override void Draw()
        {
            if (rgbVideoCopy != null)
            {
                ScreenManager.SpriteBatch.Draw(rgbVideoCopy, Main.screenRect, Color.White);
            }
            base.Draw();
            foreach (Text ins in instructions)
                ins.Draw(ScreenManager.SpriteBatch);
        }

        #endregion

        public override void Unload()
        {
            base.Unload();
        }
    }
}
