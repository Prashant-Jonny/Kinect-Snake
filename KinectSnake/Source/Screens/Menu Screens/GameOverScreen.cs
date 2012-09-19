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
using Ecslent2D.ScreenManagement.Screens;
using Ecslent2D.Input;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class GameOverScreen : MenuScreen
    {
        Text finalScoreText;
        Screen replayScreen;
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverScreen(int previousScore, Screen replayScreen)
            : base("Game Over",Color.Red)
        {
            IsPopup = true;
            AddMenuEntry("Replay", ReplayGame);
            AddMenuEntry("To Main Menu", ToMainMenu);
            AddMenuEntry("Quit Game", CheckQuitGame);
            this.replayScreen = replayScreen;

            Vector2 finalScorePos = new Vector2(Main.windowSize.X / 2, Entries[Entries.Count - 1].boundry.Bottom + TextSettings.CurrentFont.MeasureString("Final Score: " + previousScore.ToString()).Y / 2 * 2.0f);
            finalScoreText = new Text("Final Score: " + previousScore.ToString(), finalScorePos, 1.0f, Color.Goldenrod);
            
            SetHandCursor(Main.sprites["handCursor"], new Vector2(200, 200));
            SetSelectionSprite(Main.animatedSprites["load"], new Vector2(400, 400));
        }



        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void CheckQuitGame(PlayerIndexEventArgs e)
        {

            CheckQuitScreen confirmQuitMessageBox = new CheckQuitScreen();

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);

        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            Main.quit = true;
        }
        void ToMainMenu( PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new MainMenuScreen());
        }
        void ReplayGame(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(replayScreen);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            finalScoreText.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        public override void Draw( )
        {
            ScreenManager.FadeBackBufferToColor(Color.DarkRed, TransitionAlpha * 2/3);
            base.Draw();
            finalScoreText.Draw(Main.screenManager.SpriteBatch);
        }

        #endregion
    }
}
