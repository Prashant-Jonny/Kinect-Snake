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
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu", Color.Goldenrod)
        {
            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
            SetHandCursor(Main.sprites["handCursor"], new Vector2(200, 200));
            SetSelectionSprite(Main.animatedSprites["load"], new Vector2(400, 400));
        }

        public override void Activate(InputState input)
        {
            AddMenuEntry("Classic", ClassicEntrySelected);
            AddMenuEntry("Educational", EducationalEntrySelected);
            AddMenuEntry("Options", LaunchOptionsScreen);
            AddMenuEntry("Quit", OnExit);
            base.Activate(input);
        }
        #endregion

        #region Handle Input


        void LaunchOptionsScreen(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new OptionsScreen());
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void ClassicEntrySelected( PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new ClassicDifficultySelectionScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        /// 
        void EducationalEntrySelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new EducationalMenuScreen());
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected void OnExit(PlayerIndexEventArgs e)
        {
            

            CheckQuitScreen confirmExitMessageBox = new CheckQuitScreen();

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender,PlayerIndexEventArgs e)
        {
            Main.quit = true;
        }

        public override void Draw( )
        {
            base.Draw();
        }
        #endregion
    }
}
