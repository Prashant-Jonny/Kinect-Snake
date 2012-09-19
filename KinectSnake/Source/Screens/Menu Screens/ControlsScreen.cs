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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Speech.Recognition;
using Ecslent2D.ScreenManagement;
using Ecslent2D.ScreenManagement.Screens;
using Ecslent2D.Input;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    /// 


    class ControlScreen : MenuScreen
    {
        #region Initialization

        List<IconText> controls;
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public ControlScreen()
            :base("Controls",Color.Goldenrod)
        {
            this.controls = new List<IconText>();
            KinectDependencies.Add(KinectDependency.Skeleton);
        }

        void Advance(PlayerIndexEventArgs e)
        {
            this.ExitScreen();

        }
        public override void  Activate(Ecslent2D.Input.InputState input)
        {
            Texture2D[] controlIcons = new Texture2D[6] { Main.sprites["upGesture"],Main.sprites["downGesture"],Main.sprites["leftGesture"],
                Main.sprites["rightGesture"],Main.sprites["pauseGesture"],Main.sprites["playGesture"]};
            string[] controlTexts = new string[6] {" - UP", " - DOWN"," - LEFT"," - RIGHT"," - PAUSE"," - PLAY"};
            Vector2 size = new Vector2(Main.windowSize.X / 8.0f, Main.windowSize.X / 8.0f);
            for (int i = 0; i < controlTexts.Length; i ++)
            {
                if (i % 2 == 0)
                    this.controls.Add(new IconText(controlTexts[i], controlIcons[i], size, new Vector2(Main.windowSize.X * 0.33f, Main.windowSize.Y * 0.2f * (i / 2 + 2)), 1.0f, Color.White, TextAlignment.Right));
                else
                    this.controls.Add(new IconText(controlTexts[i], controlIcons[i], size, new Vector2(Main.windowSize.X * 0.66f, Main.windowSize.Y * 0.2f * (i / 2 + 2)), 1.0f, Color.White, TextAlignment.Right));
            }
            base.Activate(input);
            AddMenuEntry("Done", Advance);
            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
            SetHandCursor(Main.sprites["handCursor"]);
            SetSelectionSprite(Main.animatedSprites["load"]);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>


        #endregion
        public override void Draw()
        {
            base.Draw();
            foreach (IconText control in controls)
                control.Draw(ScreenManager.SpriteBatch);
        }
    }
}
