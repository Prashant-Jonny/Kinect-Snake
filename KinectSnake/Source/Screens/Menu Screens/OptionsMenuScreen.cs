#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ecslent2D.Input;
using Ecslent2D.Graphics;
using Ecslent2D.ScreenManagement.Screens;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class OptionsScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public OptionsScreen()
            : base("Options", Color.Goldenrod)
        {
            AddMenuEntry("Calibrate Kinect", LaunchCalibrationScreen);
            AddMenuEntry("Controls", LaunchControlScreen);
            AddMenuEntry("Back", LaunchMainMenu);
            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
            SetHandCursor(Main.sprites["handCursor"], new Vector2(200, 200));
            SetSelectionSprite(Main.animatedSprites["load"], new Vector2(400, 400));
        }


        #endregion

        #region Handle Input


        void LaunchCalibrationScreen(PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CalibrationMenuScreen(), PlayerIndex.One);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void LaunchControlScreen(PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ControlScreen(), PlayerIndex.One);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        /// 
        void LaunchMainMenu(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new MainMenuScreen());
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            Main.quit = true;
        }

        public override void Draw()
        {
            base.Draw();
        }
        #endregion
    }
}
