#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
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
    class EducationalMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public EducationalMenuScreen()
            : base("Educational Modes",Color.LightGreen)
        {
            AddMenuEntry("Quadrant", QuadrantEntrySelected);
            AddMenuEntry("Back", BackEntrySelected);
            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
            SetHandCursor(Main.sprites["handCursor"], new Vector2(200, 200));
            SetSelectionSprite(Main.animatedSprites["load"], new Vector2(400, 400));
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void QuadrantEntrySelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameSelectionScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        /// 
        void BackEntrySelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new MainMenuScreen());
        }

        public override void Draw( )
        {
            base.Draw();
        }

        #endregion
    }
}
