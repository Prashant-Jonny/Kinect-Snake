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

            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
        }

        public override void Activate(InputState input)
        {
            base.Activate(input);
            AddMenuEntry("Calibrate Kinect", LaunchCalibrationScreen);
            AddMenuEntry("Change Dominant Side", LaunchDominantHandScreen);
            AddMenuEntry("Controls", LaunchControlScreen);
            AddMenuEntry("Back", LaunchMainMenu);
            SetHandCursor(Main.sprites["handCursor"]);
            SetSelectionSprite(Main.animatedSprites["load"]);
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

        void LaunchDominantHandScreen(PlayerIndexEventArgs e)
        {
            if (true) //screenManager.input.Kinect.IsEnabled())
            {
                handCursor.Position = new Vector2(-1000, -1000);
                Screens.DominantHandSelectionScreen welcome = new Screens.DominantHandSelectionScreen();
                welcome.Right += (o, p) =>
                {
                    InputState.DominantSide = DominantSide.Right;
                };

                welcome.Left += (o, p) =>
                {
                    InputState.DominantSide = DominantSide.Left;
                };
                ScreenManager.AddScreen(welcome, PlayerIndex.One);
            }
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
