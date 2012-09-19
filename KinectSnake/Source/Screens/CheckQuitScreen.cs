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
    class CheckQuitScreen : GesturePromptScreen
    {
        #region Fields
        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public CheckQuitScreen()
            :base("Do you want to quit?", Color.Red)
        {
            IsPopup = true;
        }

        public override void Activate(InputState input)
        {
            AddGestureOption(" - Raise Arms to Confirm", GestureTests.TouchdownTest, OnAccept, Main.sprites["confirmGesture"]);
            AddGestureOption(" - Cross Arms to Cancel", GestureTests.ArmCrossTest, OnCancel, Main.sprites["cancelGesture"]);
            base.Activate(input);
        }
        #endregion

        #region Handle Input

        protected void OnAccept(PlayerIndex playerIndex)
        {
            if (Accepted != null)
                Accepted(this, new PlayerIndexEventArgs(playerIndex));
            ExitScreen();
        }
        protected void OnCancel(PlayerIndex playerIndex)
        {
            if (Cancelled != null)
                Cancelled(this, new PlayerIndexEventArgs(playerIndex));
            ExitScreen();
        }

        #endregion


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (input.IsNewButtonPress(Buttons.A, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
            {
                gestureOptions[0].callback.Invoke(playerIndex);
            }
            if (input.IsNewButtonPress(Buttons.B, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Escape, ControllingPlayer, out playerIndex))
            {
                gestureOptions[1].callback.Invoke(playerIndex);
            }


        }


    }
}
