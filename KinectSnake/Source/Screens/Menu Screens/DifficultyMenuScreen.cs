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
    /// 

    class ClassicDifficultySelectionScreen : MenuScreen
    {
        bool TransitioningToGame = true;
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public ClassicDifficultySelectionScreen()
            : base("Select Difficulty",Color.Coral)
        {
        }


        public override void Activate(InputState input)
        {
            base.Activate(input);
            AddMenuEntry("Very Easy", MegaWeenieSelected, false);
            AddMenuEntry("Easy", PansySelected, false);
            AddMenuEntry("Normal", NormalSelected, false);
            AddMenuEntry("Hard", HairySelected, false);
            AddMenuEntry("Hardest ", SuperBadassSelected, false);
            AddMenuEntry("Back ", ToMainMenu, false);
            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
            SetHandCursor(Main.sprites["handCursor"]);
            SetSelectionSprite(Main.animatedSprites["load"]);
        }
        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void MegaWeenieSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new ClassicGameScreen( Difficulty.VeryEasy));
        }
        void PansySelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new ClassicGameScreen(Difficulty.Easy));
        }
        void NormalSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new ClassicGameScreen(Difficulty.Normal));
        }
        void HairySelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new ClassicGameScreen(Difficulty.Hard));
        }
        void SuperBadassSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new ClassicGameScreen(Difficulty.Hardest));
        }
        void ToMainMenu(PlayerIndexEventArgs e)
        {
            TransitioningToGame = false;
            ScreenManager.TransitionTo(new MainMenuScreen());
        }

        #endregion
        public override void Draw( )
        {
            base.Draw();

            // Darken down any other screens that were drawn beneath the popup.
            if (ScreenState == ScreenState.TransitionOff && TransitioningToGame)
                ScreenManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
        }
    }
}
