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
using Ecslent2D.Input;
using Ecslent2D.Collision;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
using Ecslent2D.ScreenManagement;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    /// 

    public enum AnimalText
    {
        None,
        Quadrant,
        Position
    }
    class QuadrantGameScreen : GamePlayScreen
    {
        #region Fields

        IconText quadrantCommand;
        Text resultText;
        int quadrantTarget;
        int lives, correctText;
        CoordinateDraw coordinateType;
        List<DrawableAsset<Texture2D>> lifeSprites;
        AnimalText animalTextDraw;
        List<Text> animalText;
        QuadrantExcercises currentExcercise;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public QuadrantGameScreen(QuadrantExcercises excercise)
            :base()
        {
            currentExcercise = excercise;
            this.startSnakeLength = 5;
            this.rabbitPercentage = 0.0f;
            this.mouseSpeed = 0;
            this.animalTextDraw = AnimalText.None;
            this.animalText = new List<Text>();
            this.targetScore = 1000;

            if (currentExcercise == QuadrantExcercises.NumbersOne)
            {
                this.snakeSpeed = 5;
                coordinateType = CoordinateDraw.Quadrant;
            }
            else if (currentExcercise == QuadrantExcercises.NumbersTwo)
            {
                this.snakeSpeed = 5;
                coordinateType = CoordinateDraw.Axes;
            }
            else if (currentExcercise == QuadrantExcercises.NumbersThree)
            {
                this.snakeSpeed = 5;
                coordinateType = CoordinateDraw.Axes;
                this.animalTextDraw = AnimalText.Quadrant;
            }
            else if (currentExcercise == QuadrantExcercises.PolarityOne)
            {
                this.snakeSpeed = 5;
                coordinateType = CoordinateDraw.Position;
            }
            else if (currentExcercise == QuadrantExcercises.PolarityTwo)
            {
                this.snakeSpeed = 5;
                coordinateType = CoordinateDraw.Axes;
            }
            else if (currentExcercise == QuadrantExcercises.PolarityThree)
            {
                this.snakeSpeed = 5;
                coordinateType = CoordinateDraw.Axes;
                this.animalTextDraw = AnimalText.Position;
            }
            

            //KinectInput.SetNewSpeechVocabulary(new Microsoft.Speech.Recognition.Choices("play", "pause"));
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        /// 
        public override void Activate(InputState input)
        {

            grid = new DrawableGrid(3);
            landscape = new Landscape();
            effectSprites = new List<EffectSprite>();
            //for (int i = 0; i < Main.backgroundLoops.Count; i++)
            //Main.backgroundLoops[i].Play();
            scoreText = new IconText("", Main.sprites["points"],new Vector2(100,100), new Vector2(Main.windowSize.X / 6.0f, 100.0f), 0.5f, Color.Goldenrod, TextAlignment.Right);

            quadrantTarget = Main.numGenerator.Next() % 4 + 1;
            if (currentExcercise == QuadrantExcercises.PolarityOne || currentExcercise == QuadrantExcercises.PolarityTwo)
                quadrantCommand = new IconText(Utilities.QuadrantToPolarity(quadrantTarget), Main.sprites["quadrant"], new Vector2(100, 100), new Vector2(Main.windowSize.X / 3.0f, 100), 1.0f, Color.Azure, TextAlignment.Right, TextEffect.Bubble);
               
            else
                quadrantCommand = new IconText(quadrantTarget.ToString(), Main.sprites["quadrant"], new Vector2(100, 100), new Vector2(Main.windowSize.X / 3.0f, 100), 1.0f, Color.Azure, TextAlignment.Right, TextEffect.Bubble);
            resultText = new Text("", new Vector2(Main.windowSize.X / 2.0f, 100), 1.0f, Color.Red, TextEffect.Fade);

            this.lives = 5;
            lifeSprites = new List<DrawableAsset<Texture2D>>();
            for (int i = 0; i < lives; i++)
                lifeSprites.Add(new DrawableAsset<Texture2D>(new Vector2(Main.windowSize.X - (i + 1) * 80, 80), new Vector2(0, -1), new Vector2(80, 80), Main.sprites["snakeBody"]));

            landscape.AddStain(new CoordinateSystem(coordinateType));
            snake = new Snake(grid, Main.windowSize * 0.5f,  this.snakeSpeed, this.startSnakeLength);
            landscape.AddComponent(snake);

            animals = new List<IDrawableGridAsset>();
            for (int i = 0; i < 4; i++)
                this.PlaceAnimal(i + 1);

            List<Type> snakeConditions = new List<Type>();
            snakeConditions.Add(typeof(Mouse));
            snakeConditions.Add(typeof(Rabbit));

            snake.ScheduleCollisionHandling(snakeConditions);
            //ScreenManager.Game.ResetElapsedTime();

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

        }


        #endregion
        public override void OnLose()
        {
            ScreenManager.AddScreen(new GameOverScreen(this.score, new QuadrantGameScreen(currentExcercise)), ControllingPlayer);
        }

        public override void OnWin()
        {
            if (currentExcercise == QuadrantExcercises.PolarityTwo)
                ScreenManager.AddScreen(new GameWonScreen(this.score, new QuadrantGameScreen(currentExcercise)), ControllingPlayer);
            else
                ScreenManager.AddScreen(new GameWonScreen(this.score, new QuadrantGameScreen(currentExcercise), new QuadrantGameScreen(currentExcercise + 1)), ControllingPlayer);
        }


        public override void OnPause(PlayerIndex playerIndex)
        {
            if (!paused)
                ScreenManager.AddScreen(new PauseMenuScreen(GameType.Quadrant, Difficulty.Easy), ControllingPlayer);
        }

        public override void AnimalDead(IDrawableGridAsset animal)
        {
            int quadrant = Utilities.PositionToQuadrant(animal.Position);
            int index = animals.IndexOf(animal);
            bool success = false;

            if (quadrant == quadrantTarget)
            {
                quadrantTarget = Main.numGenerator.Next() % 4 + 1;
                if (currentExcercise == QuadrantExcercises.PolarityOne || currentExcercise == QuadrantExcercises.PolarityTwo)
                    quadrantCommand.text.String = Utilities.QuadrantToPolarity(quadrantTarget);
                else
                    quadrantCommand.text.String = quadrantTarget.ToString();

                success = true;
            }

            if (success)
            {
                score += 100;
                this.effectSprites.Add(new PointExplosion(animal.Position, 100, animal.gridColor));
                resultText.String = "CORRECT!";
                resultText.SetMainColor(Color.Blue);
            }
            else
            {
                resultText.String = "WRONG QUADRANT!";
                resultText.SetMainColor(Color.Red);
                lives--;
            }
            this.landscape.AddStain(new BloodStain(animal.Position));
            this.snake.head.ShowEffect(new EffectSprite(this.snake.head, this.snake.head.Direction * this.snake.head.Size * 0.3f, new Vector2(Main.animatedSprites["bloodSplatter"].Width, Main.animatedSprites["bloodSplatter"].Height), Main.animatedSprites["bloodSplatter"]));

            if (this.animalTextDraw != AnimalText.None)
                this.animalText.RemoveAt(animals.IndexOf(animal));
            animal.RemoveFromCollisionHandling();
            grid.Remove(animal);
            landscape.RemoveComponent(animal as IRenderableAsset);
            this.animals.Remove(animal);
            this.PlaceAnimal(quadrant);
            this.snake.Enlarge();
            animal.dead = true;

        }
        public void PlaceAnimal(int quadrant)
        {
            Rectangle quad = Utilities.QuadrantToRect(quadrant);
            Rectangle placementRect = new Rectangle(quad.Left, quad.Top, 160, 160);
            Vector2 dirToRect = new Vector2(placementRect.Center.X - this.snake.head.Position.X, placementRect.Center.Y - this.snake.head.Position.Y);
            while (!grid.GridsEmpty(placementRect) ||
                (quad.Contains(new Point((int)snake.head.Position.X, (int)snake.head.Position.Y)) && Utilities.AngleBetween(dirToRect, this.snake.head.Direction) < Math.PI / 3.0))
            {
                placementRect.X = Main.numGenerator.Next(quad.Left, quad.Right-100);
                placementRect.Y = Main.numGenerator.Next(quad.Top, quad.Bottom - 100);
                dirToRect = new Vector2(placementRect.Center.X - this.snake.head.Position.X, placementRect.Center.Y - this.snake.head.Position.Y);
            }

            animals.Add(new Mouse(grid, new Vector2(placementRect.Center.X, placementRect.Center.Y), this.mouseSpeed));

            if (animalTextDraw != AnimalText.None)
            {

                string str = "";
                if (this.animalTextDraw == AnimalText.Quadrant)
                {
                    str = quadrantTarget.ToString();
                    foreach (Text t in animalText)
                        t.String = str;
                }

                else if (this.animalTextDraw == AnimalText.Position)
                {
                    str = Utilities.QuadrantToPolarity(quadrantTarget);
                    foreach (Text t in animalText)
                        t.String = str;
                }

                Text newText = new Text(str, animals[animals.Count - 1].Position, 1.0f, Color.Cyan, TextEffect.None);
                newText.SetAlternateColor(Color.DarkCyan);
                animalText.Add(newText);

            }


            animals[animals.Count - 1].ScheduleCollisionHandling();

            landscape.AddComponent(animals[animals.Count - 1] as IRenderableAsset);
        }


        #region Update and Draw



        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                quadrantCommand.Update(gameTime);
                resultText.Update(gameTime);
                if (lives <= 0)
                    this.snake.dead = true;
                foreach (Text t in animalText)
                    t.Update(gameTime);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        
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

            if (this.animalTextDraw == AnimalText.None)
                quadrantCommand.Draw(ScreenManager.SpriteBatch);
            resultText.Draw(ScreenManager.SpriteBatch);

            foreach (Text str in animalText)
                str.Draw(ScreenManager.SpriteBatch);
            for (int i = 0; i < lives; i++)
                lifeSprites[i].Draw(ScreenManager.SpriteBatch);
            base.Draw();

        }
#endregion
    }
}
