#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Ecslent2D.Collision;
using Ecslent2D.Input;
using Ecslent2D.Graphics.Text;
using Ecslent2D.ScreenManagement;
using Ecslent2D.ScreenManagement.Screens;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    /// 
    


    class GamePlayScreen : Screen
    {
        #region Fields
        public int score;
        public IconText scoreText;
        public Snake snake;
        public List<IDrawableGridAsset> animals;
        public Landscape landscape;
        public List<EffectSprite> effectSprites;
        public int startSnakeLength;
        public int snakeSpeed;
        public float rabbitPercentage;
        public int mouseSpeed, targetScore = -1;
        protected DrawableGrid grid;
        private Vector3 tempMove = new Vector3(0,0,0);
        protected bool paused;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GamePlayScreen()
            :base()
        {
            KinectDependencies.Add(KinectDependency.Skeleton);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            score = 0;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(InputState input)
        {
            if (input.Kinect.IsEnabled())
            {
                //input.Kinect.SetNewSpeechVocabulary(new Microsoft.Speech.Recognition.Choices( "pause"));
                input.Kinect.EnableGestureTest(GestureTests.TurnLeft,OnLeft);
                input.Kinect.EnableGestureTest (GestureTests.TurnRight,OnRight);
                input.Kinect.EnableGestureTest( GestureTests.TurnUp,OnUp);
                input.Kinect.EnableGestureTest( GestureTests.TurnDown,OnDown);
                input.Kinect.EnableGestureTest( GestureTests.Pause,OnPause);
            }
            base.Activate(input);
        }

        public override void Deactivate(InputState input)
        {

            if (input.Kinect.IsEnabled())
            {

                input.Kinect.DisableGestureTest(GestureTests.TurnLeft);
                input.Kinect.DisableGestureTest(GestureTests.TurnRight);
                input.Kinect.DisableGestureTest(GestureTests.TurnUp);
                input.Kinect.DisableGestureTest(GestureTests.TurnDown);
                input.Kinect.DisableGestureTest(GestureTests.Pause);
            }
            base.Deactivate(input);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);



            paused = coveredByOtherScreen || otherScreenHasFocus;

            if (IsActive)
            {
                if (snake.dead == true)
                {
                    for (int i = 0; i < Main.backgroundLoops.Count; i++)
                        Main.backgroundLoops[i].Stop();

                    OnLose();
                }
                else if (targetScore != -1 && score >= targetScore)
                {
                    OnWin();
                }
                else
                {
                    grid.Update(gameTime);
                    Main.animatedSprites["mouse"].Update(gameTime.ElapsedGameTime.Ticks);
                    Main.animatedSprites["rabbit"].Update(gameTime.ElapsedGameTime.Ticks);

                    this.snake.Update(gameTime);
                    int index = -1;
                    for (int i = 0; i < this.animals.Count; i++)
                    {
                        if (this.animals[i].dead)
                        {
                            //Main.killSounds[Main.numGenerator.Next() % Main.killSounds.Count].Play();
                            index = this.animals.IndexOf(this.animals[i]);
                            i--;
                            this.AnimalDead(this.animals[index]);
                        }
                        else
                            this.animals[i].Update(gameTime);
                    }

                }
                scoreText.text.String = score.ToString();
                if (this.targetScore != -1)
                    scoreText.text.String = scoreText.text.String + " / " + targetScore.ToString();
                scoreText.Update(gameTime);
                for (int i = 0; i < this.effectSprites.Count; i++)
                {
                    if (this.effectSprites[i].IsDone())
                    {
                        this.effectSprites.RemoveAt(i);
                        i--;
                    }
                    else
                        this.effectSprites[i].Update(gameTime);
                }
            }
        }

        public virtual void AnimalDead(IDrawableGridAsset animal)
        {
        }

        public virtual void AnimalHit(IDrawableGridAsset animal)
        {
        }
        public virtual void PlaceAnimal()
        {
        }
        public virtual void OnLose()
        {
        }

        public virtual void OnWin()
        {
        }
        public virtual void OnPause(PlayerIndex playerIndex)
        {
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex playerIndex;
            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            //bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];




            if (input.IsNewButtonPress(Buttons.Start, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Space, ControllingPlayer, out playerIndex))
            {
                OnPause(playerIndex);
            }
            
            else
            {

            
                if (input.IsNewKeyPress(Keys.OemPlus, ControllingPlayer, out playerIndex))
                    grid.Enlarge();
                else if (input.IsNewKeyPress(Keys.OemMinus, ControllingPlayer, out playerIndex))
                    grid.Shrink();
                if (input.IsNewKeyPress(Keys.G, ControllingPlayer, out playerIndex))
                    grid.render = !grid.render;

                if (input.IsNewButtonPress(Buttons.DPadUp, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Up, ControllingPlayer, out playerIndex))
                    OnUp(playerIndex);

                else if (input.IsNewButtonPress(Buttons.DPadDown, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Down, ControllingPlayer, out playerIndex))
                    OnDown(playerIndex);

                else if (input.IsNewButtonPress(Buttons.DPadLeft, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Left, ControllingPlayer, out playerIndex))
                    OnLeft(playerIndex);

                else if (input.IsNewButtonPress(Buttons.DPadRight, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Right, ControllingPlayer, out playerIndex))
                    OnRight(playerIndex);
            }
        }

        protected void OnUp(PlayerIndex player)
        {
            if (!paused)
                this.snake.ChangeDirection(new Vector2(0, -1));
        }
        protected void OnDown(PlayerIndex player)
        {
            if (!paused)
                this.snake.ChangeDirection(new Vector2(0, 1));
        }
        protected void OnLeft(PlayerIndex player)
        {
            if (!paused)
                this.snake.ChangeDirection(new Vector2(-1,0));
        }

        protected void OnRight(PlayerIndex player)
        {
            if (!paused)
                this.snake.ChangeDirection(new Vector2(1,0));
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw()
        {

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 )
            {

                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
            }
            base.Draw();
        }


        #endregion
    }
}
