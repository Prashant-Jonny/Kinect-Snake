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
using Ecslent2D.Input;
using Ecslent2D.ScreenManagement;
using Ecslent2D.ScreenManagement.Screens;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        Difficulty gameDiff;
        GameType gameType;
        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(GameType currentGameType, Difficulty currentDifficulty)
            : base("Paused", Color.RoyalBlue)
        {
            IsPopup = true;
            KinectDependencies.Add(KinectDependency.Skeleton);
            gameType = currentGameType;
            gameDiff = currentDifficulty;
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.

        void CheckMainMenu(PlayerIndexEventArgs e)
        {

            CheckQuitScreen confirmMainMenu = new CheckQuitScreen();

            confirmMainMenu.Accepted += ConfirmMainMenuMessageBoxAccepted;

            ScreenManager.AddScreen(confirmMainMenu, ControllingPlayer);
        }

        public override void Activate(InputState input)
        {
            if (input.Kinect.IsEnabled())
            {
                //input.Kinect.SetNewSpeechVocabulary(new Microsoft.Speech.Recognition.Choices("play"));
                  input.Kinect.EnableGestureTest( GestureTests.Resume,Resume);
            }
            base.Activate(input);
            AddMenuEntry("Resume Game", ResumeGame);
            AddMenuEntry("Restart Game", RestartGame);
            AddMenuEntry("To Main Menu", CheckMainMenu);
            SetHandCursor(Main.sprites["handCursor"]);
            SetSelectionSprite(Main.animatedSprites["load"]);
        }

        public override void Deactivate(InputState input)
        {
            if (input.Kinect.IsEnabled())
                input.Kinect.DisableGestureTest(GestureTests.Resume);
            base.Deactivate(input);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {


            //RecognitionResult r = input.Kinect.GetNewestCommand();
            //if ((r != null && r.likelihood > 0.55 && r.word == "play") ||
                //this.ExitScreen();
            PlayerIndex playerIndex;
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Space,ControllingPlayer,out playerIndex))
                this.ExitScreen();
            base.HandleInput(gameTime, input);
        }
        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmMainMenuMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new MainMenuScreen());
        }
        void RestartGame(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo( Main.gameManager.LaunchGameScreen(gameType, gameDiff));
        }

        void ResumeGame(PlayerIndexEventArgs e)
        {
            this.ExitScreen();
        }

        public void Resume(PlayerIndex playerIndex)
        {
            this.ExitScreen();
        }
        public override void Draw()
        {
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw();
        }
        #endregion
    }
}
