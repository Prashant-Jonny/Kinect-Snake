#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using Ecslent2D.Input;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
using Ecslent2D.ScreenManagement;
using Ecslent2D.ScreenManagement.Screens;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class DominantHandSelectionScreen : GesturePromptScreen
    {
        #region Fields
        protected DrawableAsset<Animation2D> selectionLoader;
        Text help, armIndicator;
        bool onRight = false, onLeft = false;
        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Right;
        public event EventHandler<PlayerIndexEventArgs> Left;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public DominantHandSelectionScreen()
            : base("Select Hand Preference", Color.Red)
        {
            Vector2 iconSize = new Vector2(175, 175);
            AddGestureOption(" - Hold Up Right Arm to Select Right", GestureTests.RightArmUp, OnRight, Main.sprites["confirmGesture"]);
            AddGestureOption(" - Hold Up Left Arm to Left Right", GestureTests.LeftArmUp, OnLeft, Main.sprites["cancelGesture"]);
            IsPopup = true;
            help = new Text("", gestureOptions[gestureOptions.Count - 1].Position + new Vector2(0, padding * 3),2.0f,Color.Goldenrod);
            armIndicator = new Text("", Main.windowSize * 0.5f, 3.0f, Color.Lime);
        }

        #endregion

        public override void Activate(InputState input)
        {
            base.Activate(input);
            selectionLoader = new DrawableAsset<Animation2D>(Main.windowSize * 0.5f, new Vector2(0, -1), new Vector2(500, 500), Main.content.Load<Animation2D>("Content\\Images\\load"));
            if (input.Kinect.IsEnabled())
            {
                input.Kinect.EnableGestureTest(GestureTests.RightArmDown, OnRightDown);
                input.Kinect.EnableGestureTest(GestureTests.LeftArmDown, OnLeftDown);
            }
        }
        public override void Deactivate(InputState input)
        {
            base.Deactivate(input);
            if (input.Kinect.IsEnabled())
            {
                input.Kinect.DisableGestureTest(GestureTests.RightArmDown);
                input.Kinect.DisableGestureTest(GestureTests.LeftArmDown);
            }
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (onRight && onLeft)
            {
                help.String = " Only Raise One Arm!";
                armIndicator.String = "";
            }
            else if (onRight || onLeft)
                selectionLoader.ColorMap.Update(gameTime.ElapsedGameTime.Ticks);
            else
            {
                onLeft = false;
                onRight = false;
                selectionLoader.ColorMap.Stop();
                armIndicator.String = "";
            }
            help.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #region Handle Input

        protected void OnRight(PlayerIndex playerIndex)
        {
            if (!onRight)
                selectionLoader.ColorMap.Play();
            if (selectionLoader.ColorMap.CurrentFrame == selectionLoader.ColorMap.FrameCount - 1)
            {
                if (Right != null)
                    Right(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
            onRight = true;
            armIndicator.String = "R";
        }

        protected void OnRightDown(PlayerIndex playerIndex)
        {
            onRight = false;
            help.String = "";
        }
        protected void OnLeftDown(PlayerIndex playerIndex)
        {
            onLeft = false;
            help.String = "";
        }
        protected void OnLeft(PlayerIndex playerIndex)
        {
            if (!onLeft)
                selectionLoader.ColorMap.Play();
            if (selectionLoader.ColorMap.CurrentFrame == selectionLoader.ColorMap.FrameCount - 1)
            {
                if (Left != null)
                    Left(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
            onLeft = true;
            armIndicator.String = "L";
        }


        public override void Draw()
        {
            base.Draw();
            help.Draw(ScreenManager.spriteBatch);
            selectionLoader.Draw(ScreenManager.spriteBatch);
            armIndicator.Draw(ScreenManager.spriteBatch);
        }

        #endregion
    }
}
