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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;
using Ecslent2D.Input;
using Ecslent2D.Input.Kinect;
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
    /// 

    public class GestureOptionEntry : IconText
    {
        public Color unlockedColor;
        public Color lockColor = Color.Gray;
        public GestureTest test;
        public GestureHandler callback;

        public GestureOptionEntry(string text, Texture2D icon, Vector2 position, float scale, Color color, TextAlignment alignment, GestureTest test, GestureHandler callback)
            : base(text, icon, new Vector2(200, 200), position, scale, color, alignment)
        {
            unlockedColor = color;
            this.callback = callback;
            this.test = test;
        }

        public void Activate(InputState input)
        {
            input.Kinect.EnableGestureTest(test, callback);
        }

        public void Deactivate(InputState input)
        {
            input.Kinect.DisableGestureTest(test);
        }
    }
    class GesturePromptScreen : Screen
    {
        #region Fields
        
        Text title;
        Texture2D gradientTexture;
        protected List<GestureOptionEntry> gestureOptions;
        Rectangle backgroundRectangle;
        Color textColor = Color.White;
        TextAlignment textAlignment = TextAlignment.Right;
        float backgroundPanelWidth = 0;
        bool backgroundPanelDirty = true;
        protected float padding = 100f;

        #endregion


        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public GesturePromptScreen(string title, Color titleColor)
        {
            this.KinectDependencies.Add(KinectDependency.Skeleton);
            this.title = new Text(title, new Vector2(Main.windowSize.X * 0.5f,100), 2.0f, titleColor );
            this.gestureOptions = new List<GestureOptionEntry>();
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        public void AddGestureOption(string actionString, GestureTest test, GestureHandler callback, Texture2D icon, float scale = 1.0f)
        {
            Vector2 pos;
            if (gestureOptions.Count == 0)
                pos = new Vector2(Main.windowSize.X * 0.5f, title.Position.Y + title.Size.Y / 2 + padding*3);
            else
                pos = new Vector2(Main.windowSize.X * 0.5f, gestureOptions[gestureOptions.Count - 1].Position.Y + gestureOptions[gestureOptions.Count - 1].Size.Y / 2 + padding);
            GestureOptionEntry newEntry = new GestureOptionEntry(actionString,icon, pos, scale, textColor,textAlignment, test, callback);
            backgroundPanelWidth = Math.Max(newEntry.Size.X,backgroundPanelWidth);
            gestureOptions.Add(newEntry);
            backgroundPanelDirty = true;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (backgroundPanelDirty)
            {

                // The background includes a border somewhat larger than the text itself.
                const int hPad = 32;
                const int vPad = 16;

                float topHeight = gestureOptions[0].Position.Y - gestureOptions[0].Size.Y / 2;
                float bottomHeight = gestureOptions[gestureOptions.Count - 1].Position.Y + gestureOptions[gestureOptions.Count - 1].Size.Y / 2;

                backgroundRectangle = new Rectangle((int)(gestureOptions[0].Position.X - gestureOptions[0].Size.X / 2 - hPad),
                                                              (int)(gestureOptions[0].Position.Y - gestureOptions[0].Size.Y / 2 - vPad),
                                                              (int)( backgroundPanelWidth + hPad * 2),
                                                              (int)(bottomHeight - topHeight + vPad *2));
                backgroundPanelDirty = false;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        
        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(InputState input)
        {

            base.Activate(input);
            foreach (GestureOptionEntry option in gestureOptions)
                option.Activate(input);



            ContentManager content = Main.content;
            gradientTexture = content.Load<Texture2D>("Content\\Images\\gradient");

        }

        #endregion

        public override void Deactivate(InputState input)
        {
            if (input.Kinect.IsEnabled())
            {
                foreach (GestureOptionEntry option in gestureOptions)
                    option.Deactivate(input);
            }
        }



        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw()
        {

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);


            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            // Draw the background rectangle.
            ScreenManager.SpriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            title.Draw(ScreenManager.spriteBatch);
            foreach (GestureOptionEntry option in gestureOptions)
                option.Draw(ScreenManager.spriteBatch);

        }


        #endregion
    }
}
