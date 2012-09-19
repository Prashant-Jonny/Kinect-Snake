#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Ecslent2D.Input;
using Ecslent2D.ScreenManagement.Screens;
#endregion

namespace Ecslent2D.ScreenManagement
{
    /// <summary>
    /// The screen manager is a component which manages one or more Screens.GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    /// 

    
    public class ScreenManager: DrawableGameComponent
    {
        #region Fields

        private const string StateFilename = "ScreenManagerState.xml";

        protected List<Screens.Screen> screens = new List<Screens.Screen>();
        List<Screens.Screen> tempScreensList = new List<Screens.Screen>();

        protected InputState input;
        public static SpriteBatch spriteBatch;
        public Texture2D fadeTexture;

        public int transitionOver = -1;
        protected bool isInitialized;
        protected bool inFullTransition = false;
        protected Screen[] screensToLoad = null;
        bool traceEnabled;

        #endregion

        #region Properties


        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            traceEnabled = false;
            input = new InputState();
        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }


        public void TransitionTo(params Screen[] gameScreens)
        {
            foreach (Screen screen in screens) 
                screen.ExitScreen();

            screensToLoad = gameScreens;
            inFullTransition = true;

        }

        
        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {

            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            fadeTexture = new Texture2D(GraphicsDevice, 1, 1);
            fadeTexture.SetData<Color>(new Color[1] { Color.Black });

            // Tell each of the screens to load their content.
            foreach (Screens.Screen screen in screens)
            {
                screen.Activate(input);
            }
        }


        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (Screens.Screen screen in screens)
            {
                screen.Unload();
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update(gameTime);

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            tempScreensList.Clear();

            foreach (Screens.Screen screen in screens)
                tempScreensList.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (tempScreensList.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                Screens.Screen screen = tempScreensList[tempScreensList.Count - 1];

                tempScreensList.RemoveAt(tempScreensList.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == Screens.ScreenState.TransitionOn ||
                    screen.ScreenState == Screens.ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (traceEnabled)
                TraceScreens();
        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (Screens.Screen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
            foreach (Screens.Screen screen in screens)
            {
                if (screen.ScreenState == Screens.ScreenState.Hidden)
                    continue;

                screen.Draw();
            }
            spriteBatch.End();
        }


        #endregion

        #region Public Methods



        private bool ScreenHasDependency(Screens.KinectDependency dependency)
        {
            bool hasDependency = true;
            foreach (Screen s in screens)
            {
                if (s.KinectDependencies.Contains(dependency))
                    hasDependency = false;
            }
            return hasDependency;
        }
        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        /// 
        public virtual void AddScreen(Screens.Screen screen, PlayerIndex? controllingPlayer, int indexAt = -1)
        {

            if (input.Kinect.IsEnabled())
            {
                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Skeleton) && !input.Kinect.IsSkeletalTrackingEnabled())
                    input.Kinect.EnableSkeletalTracking();

                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Color) && !input.Kinect.IsColorCaptureEnabled())
                    input.Kinect.EnableColorCapture(GraphicsDevice, Microsoft.Kinect.ColorImageFormat.RgbResolution1280x960Fps12);

                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Depth) && !input.Kinect.IsDepthCaptureEnabled())
                    input.Kinect.EnableDepthCapture(GraphicsDevice);

                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Voice) && !input.Kinect.IsVoiceRecognitionEnabled())
                    input.Kinect.EnableVoiceRecognition();
            }
            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.ScreenManager = this;
                screen.ControllingPlayer = controllingPlayer;
                screen.Activate(input);
            }
            if (indexAt == -1 )
                screens.Add(screen);
            else if (indexAt <= screens.Count)
                screens.Insert(indexAt, screen);

        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use Screens.GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public virtual void RemoveScreen(Screens.Screen screen)
        {

            if (inFullTransition && screens.Count == 1)
            {
                for (int i = 0; i < screensToLoad.Length; i++)
                    AddScreen(screensToLoad[i], PlayerIndex.One);
                inFullTransition = false;
                screensToLoad = null;
            }

            // If we have a graphics device, tell the screen to unload content.
            screen.Deactivate(input);
            if (isInitialized)
            {
                screen.Unload();
            }

            screens.Remove(screen);
            tempScreensList.Remove(screen);
            if (input.Kinect.IsEnabled())
            {
                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Skeleton) && ScreenHasDependency(Screens.KinectDependency.Skeleton))
                    input.Kinect.DisableSkeletalTracking();

                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Voice) && ScreenHasDependency(Screens.KinectDependency.Voice))
                    input.Kinect.DisableVoiceRecognition();

                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Color) && ScreenHasDependency(Screens.KinectDependency.Color))
                    input.Kinect.DisableColorCapture();

                if (screen.KinectDependencies.Contains(Screens.KinectDependency.Depth) && ScreenHasDependency(Screens.KinectDependency.Depth))
                    input.Kinect.DisableDepthCapture();

            }
           
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public Screens.Screen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            spriteBatch.Draw(fadeTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            
        }
        public void FadeBackBufferToColor(Color color, float alpha)
        {
            fadeTexture.SetData<Color>(new Color[1] { color });
            spriteBatch.Draw(fadeTexture, GraphicsDevice.Viewport.Bounds, color * alpha);
        }

        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public virtual void Deactivate() { }
        public virtual bool Activate(bool instancePreserved) { return true; }
        public void Unload()
        {
            input.Dispose();
            base.Dispose();
        }
        #endregion
    }
}
