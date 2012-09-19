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
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
using Ecslent2D.Input;
using Ecslent2D.ScreenManagement;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class ClassicGameScreen : GamePlayScreen
    {
        #region Fields
        public Difficulty currentDifficulty;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ClassicGameScreen(Difficulty diff)
            :base()
        {
            currentDifficulty = diff;
            this.startSnakeLength = 5;
            if (diff == Difficulty.VeryEasy)
            {
                this.snakeSpeed = 5;
                this.mouseSpeed = 1;
                this.rabbitPercentage = 0.0f;

            }
            else if (diff == Difficulty.Easy)
            {
                this.snakeSpeed = 5;
                this.mouseSpeed = 1;
                this.rabbitPercentage = 0.0f;
            }
            else if (diff == Difficulty.Normal)
            {
                this.snakeSpeed = 10;
                this.mouseSpeed = 2;
                this.rabbitPercentage = 0.0f;
            }
            else if (diff == Difficulty.Hard)
            {
                this.snakeSpeed = 10;
                this.mouseSpeed = 3;
                this.rabbitPercentage = 0.2f;
            }
            else if (diff == Difficulty.Hardest)
            {
                this.snakeSpeed = 10;
                this.mouseSpeed = 4;
                this.rabbitPercentage = 0.4f;
            }

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(InputState input)
        {

            grid = new DrawableGrid(3);
            landscape = new Landscape();
            effectSprites = new List<EffectSprite>();
            //for (int i = 0; i < Main.backgroundLoops.Count; i++)
            //Main.backgroundLoops[i].Play();

            scoreText = new IconText("", Main.sprites["points"], new Vector2(Main.windowSize.X / 10.0f, Main.windowSize.X / 10.0f), new Vector2(Main.windowSize.X / 6.0f, Main.windowSize.X / 12.0f), 0.5f, Color.Goldenrod, TextAlignment.Right);

            snake = new Snake(grid, Main.windowSize * 0.5f,  this.snakeSpeed, this.startSnakeLength);
            landscape.AddComponent(snake);


            List<Type> snakeConditions = new List<Type>();
            snakeConditions.Add(typeof(Mouse));
            snakeConditions.Add(typeof(Rabbit));

            snake.ScheduleCollisionHandling(snakeConditions);
            animals = new List<IDrawableGridAsset>();
            for (int i = 0; i < 5; i++)
                this.PlaceAnimal();
            ScreenManager.Game.ResetElapsedTime();
            base.Activate(input);
        }


        public override void Deactivate(InputState input)
        {

            base.Deactivate(input);
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        public override void OnLose()
        {
            ScreenManager.AddScreen(new GameOverScreen(this.score, new ClassicGameScreen(currentDifficulty)), ControllingPlayer);
        }

        public override void OnWin()
        {
            if (currentDifficulty == Difficulty.Hardest)
                ScreenManager.AddScreen(new GameWonScreen(this.score, new ClassicGameScreen(currentDifficulty)), ControllingPlayer);
            else
                ScreenManager.AddScreen(new GameWonScreen(this.score, new ClassicGameScreen(currentDifficulty), new ClassicGameScreen(currentDifficulty + 1)), ControllingPlayer);
        }

        public override void OnPause(PlayerIndex playerIndex)
        {
            if (!paused)
                ScreenManager.AddScreen(new PauseMenuScreen(GameType.Classic, currentDifficulty), ControllingPlayer);
            paused = true;
        }

        public override void AnimalDead(IDrawableGridAsset animal)
        {
            score += ((int)this.currentDifficulty + 1) * 50;
            this.effectSprites.Add(new PointExplosion(animal.Position, ((int)this.currentDifficulty + 1) * 50, animal.gridColor));
            this.landscape.AddStain(new BloodStain(animal.Position));
            animal.RemoveFromCollisionHandling();
            grid.Remove(animal);
            this.snake.head.ShowEffect(new EffectSprite(this.snake.head, this.snake.head.Direction * this.snake.head.Size * 0.3f, new Vector2(Main.animatedSprites["bloodSplatter"].Width, Main.animatedSprites["bloodSplatter"].Height), Main.animatedSprites["bloodSplatter"]));

            this.PlaceAnimal();
            this.snake.Enlarge();
            animal.dead = true;

            landscape.RemoveComponent(animal as IRenderableAsset);
            this.animals.Remove(animal);
        }


        public override void PlaceAnimal()
        {
            if (Main.numGenerator.NextDouble() < this.rabbitPercentage)
                animals.Add(new Rabbit(grid, new Vector2(Main.numGenerator.Next(80, (int)Main.windowSize.X) - 80, Main.numGenerator.Next(80, (int)Main.windowSize.Y) - 80), this.mouseSpeed));
            else
                animals.Add(new Mouse(grid, new Vector2(Main.numGenerator.Next(80, (int)Main.windowSize.X) - 80, Main.numGenerator.Next(80, (int)Main.windowSize.Y) - 80), this.mouseSpeed));

            animals[animals.Count - 1].Spawn();
            landscape.AddComponent(animals[animals.Count - 1] as IRenderableAsset);
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw()
        {
            this.landscape.Draw(ScreenManager.SpriteBatch);
            grid.Draw(ScreenManager.SpriteBatch);
            foreach (EffectSprite e in effectSprites)
                e.Draw(ScreenManager.SpriteBatch);
            scoreText.Draw(ScreenManager.SpriteBatch);

            base.Draw();
        }


        #endregion
    }
}
