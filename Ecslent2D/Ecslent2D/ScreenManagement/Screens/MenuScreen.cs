using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Speech.Recognition;
using Microsoft.Kinect;
using Ecslent2D.Graphics.Text;
using Ecslent2D.Graphics;
using Ecslent2D.Input;
using Microsoft.Xna.Framework.Input;

namespace Ecslent2D.ScreenManagement.Screens
{
    public class MenuEntry : Text
    {
        bool locked = false;
        Color unlockedColor;
        public Color lockColor = Color.Gray;
        public event InputHandler callback;
        public MenuEntry(string text, Vector2 position, float scale, Color color, InputHandler c)
            : base(text, position, scale, color)
        {
            unlockedColor = color;
            callback = c;
        }
        public void Lock()
        {
            locked = true;
            this.SetMainColor(lockColor * 0.5f);
        }
        public void Unlock()
        {
            locked = true;
            this.SetMainColor(unlockedColor);
        }
        public bool IsLocked()
        {
            return locked;
        }
        
        public void Select(PlayerIndex playerIndex)
        {
            if (!locked)
                callback(new PlayerIndexEventArgs(playerIndex));
        }
    }


    public abstract class MenuScreen : Screen
    {
        #region Fields

        protected Text title;
        protected List<MenuEntry> entries;
        protected int hoverIndex;
        protected Color textColor, titleColor;
        protected float padding = 100f;
        protected int entryOver = -1;
        protected string menuTitle;
        protected DrawableAsset<Texture2D> background;


        protected Choices language;
        protected DrawableAsset<Texture2D> handCursor;
        protected DrawableAsset<Animation2D> selectionLoader;
        protected bool usingKinect = false;

        #endregion

        #region Properties

        protected List<MenuEntry> Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        /// 
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle, Color titleColor)
            : base()
        {
            this.menuTitle = menuTitle;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            entries = new List<MenuEntry>();
            hoverIndex = -1;
            textColor = Color.White;
            this.titleColor = titleColor;
            KinectDependencies = new List<KinectDependency>();
            KinectDependencies.Add(KinectDependency.Skeleton);
            language = new Choices("select");
            //handCursor = new DrawableAsset<Texture2D>(screenDimensions * 0.5f, new Vector2(0, -1), new Vector2(200, 200), Main.sprites["handCursor"]);
            //load = new DrawableAsset<GifAnimation.GifAnimation>(screenDimensions * 0.5f, new Vector2(0, -1), new Vector2(500, 500), Main.animatedSprites["load"]);
            //ecslSprite = new DrawableAsset<Texture2D>(new Vector2(100, screenDimensions.Y - 100), new Vector2(0, -1), new Vector2(200, 200), Main.sprites["ecsl"]);
        }


        public override void Activate(InputState input)
        {
            Vector2 screenDimensions = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            float scale = (screenDimensions.X / 1920.0f) * 2.0f;
            padding = (screenDimensions.Y / 1200.0f) * padding;
            title = new Text(menuTitle, new Vector2(screenDimensions.X * 0.5f, screenDimensions.Y * 0.15f), scale, titleColor);
            base.Activate(input);
        }
        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;
            
            usingKinect = input.Kinect.IsEnabled() && input.Kinect.IsTrackingPlayer();
            if (usingKinect)
            {
                Vector2 handPos;
                Vector2 screenDimensions = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
                Vector3 rawHandPos, referencePos;
                Vector3 hipPos = input.Kinect.GetJointPosition(JointType.HipCenter);
                float trackedWidth = input.Kinect.GetJointPosition(JointType.ShoulderRight).X - input.Kinect.GetJointPosition(JointType.ShoulderLeft).X;
                if (Input.InputState.DominantSide == DominantSide.Right)
                {
                    rawHandPos = input.Kinect.GetJointPosition(JointType.HandRight);
                    referencePos = hipPos + new Vector3(trackedWidth / 2.0f, -trackedWidth / 2.0f, 0);
                }
                else
                {
                    rawHandPos = input.Kinect.GetJointPosition(JointType.HandLeft);
                    referencePos = hipPos - new Vector3(trackedWidth / 2.0f, trackedWidth / 2.0f,0);
                }
                Vector3 diff = rawHandPos - referencePos;
                handPos = screenDimensions * 0.5f + new Vector2(diff.X, diff.Y) * screenDimensions.X/2;
                selectionLoader.Position = handCursor.Position = handPos;

                int nowOver = HoverItemAt(handPos);
                if (nowOver == -1 || nowOver != entryOver)
                {
                    selectionLoader.ColorMap.Play();
                }

                entryOver = nowOver;

            }
            if (input.IsNewButtonPress(Buttons.DPadUp, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Up, ControllingPlayer, out playerIndex))
                HoverLastItem();

            if (input.IsNewButtonPress(Buttons.DPadDown, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Down, ControllingPlayer, out playerIndex))
                HoverNextItem();

            if (input.IsNewButtonPress(Buttons.A, ControllingPlayer, out playerIndex) || input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
                SelectAtHover(playerIndex);
            base.HandleInput(gameTime, input);

        }



        #endregion

        #region Update and Draw



        /// <summary>
        /// Updates the menu.
        /// </summary>
        /// 
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 transition;

            if (transitionOffset != 0)
            {
                Vector2 screenDimensions = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
                if (ScreenState == ScreenState.TransitionOn)
                    transition = new Vector2(screenDimensions.X / 2 - (transitionOffset * screenDimensions.X / 2) - entries[0].Position.X, 0);
                else
                    transition = new Vector2(screenDimensions.X / 2 + (transitionOffset * screenDimensions.X / 2) - entries[0].Position.X, 0);


                for (int i = 0; i < entries.Count; i++)
                {
                    entries[i].Position += transition;
                    if (entries[i].IsLocked())
                        entries[i].SetMainColor(entries[i].lockColor * 0.5f * TransitionAlpha);
                    else
                        entries[i].SetMainColor(textColor );
                    entries[i].alpha = TransitionAlpha;
                }
            }


            title.Update(gameTime);
            for (int i = 0; i < entries.Count; i++)
                entries[i].Update(gameTime);
            if (otherScreenHasFocus || coveredByOtherScreen)
                selectionLoader.Position = handCursor.Position = new Vector2(-1000, -1000);
            else if( usingKinect && entryOver != -1)
            {
                selectionLoader.ColorMap.Update(gameTime.ElapsedGameTime.Ticks);
                if (selectionLoader.ColorMap.CurrentFrame == selectionLoader.ColorMap.FrameCount - 1 && this.PosOverHover(handCursor.Position))
                {
                    SelectAtHover(PlayerIndex.One);
                    selectionLoader.ColorMap.Play();
                }
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw()
        {
            // make sure our entries are in the right place before we draw them

            if (background != null)
                background.Draw(ScreenManager.spriteBatch);
            if (title != null)
                title.Draw(ScreenManager.spriteBatch);
            foreach (Text item in entries)
                item.Draw(ScreenManager.spriteBatch);


            if (usingKinect)
            {
                handCursor.Draw(ScreenManager.SpriteBatch);
                selectionLoader.Draw(ScreenManager.SpriteBatch);
            }

            //ecslSprite.Draw(ScreenManager.spriteBatch);
            base.Draw();
        }


        #endregion

        #region Menu Entry Selection Functions
        protected void SelectAtHover(PlayerIndex playerIndex)
        {
            entries[hoverIndex].Select(playerIndex);
        }

        protected void HoverLastItem()
        {
            if (hoverIndex != -1)
            {
                entries[hoverIndex].SetEffect(TextEffect.None);
                MenuEntry newHover;
                do
                {
                    if (hoverIndex == 0)
                        hoverIndex = entries.Count - 1;
                    else
                        hoverIndex--;
                    newHover = entries[hoverIndex];
                } while (newHover.IsLocked());
                entries[hoverIndex].SetEffect(TextEffect.Bubble);
            }
        }
        protected void HoverNextItem()
        {
            if (hoverIndex != -1)
            {
                entries[hoverIndex].SetEffect(TextEffect.None);
                MenuEntry newHover;
                do
                {
                    if (hoverIndex == entries.Count - 1)
                        hoverIndex = 0;
                    else
                        hoverIndex++;
                    newHover = entries[hoverIndex];
                } while (newHover.IsLocked());
                entries[hoverIndex].SetEffect(TextEffect.Bubble);
            }
        }

        protected int HoverItemAt(Vector2 pos)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].boundry.Contains(new Point((int)pos.X, (int)pos.Y)) && !entries[i].IsLocked())
                {
                    entries[hoverIndex].effect = TextEffect.None;
                    hoverIndex = i;
                    entries[hoverIndex].effect = TextEffect.Bubble;
                    return i;
                }
            }

            return -1;
        }
        protected bool PosOverHover(Vector2 pos)
        {

            return entries[hoverIndex].boundry.Contains(new Point((int)pos.X, (int)pos.Y));
        }
        protected bool HoverEntryNamed(string speech)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].text == speech && !entries[i].IsLocked())
                {
                    entries[hoverIndex].effect = TextEffect.None;
                    hoverIndex = i;
                    entries[hoverIndex].effect = TextEffect.Bubble;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Functions for Derived Classes

        protected virtual void SetBackground(DrawableAsset<Texture2D> back)
        {
            background = back;
        }

        public void SetHandCursor(Texture2D cursor)
        {
            Vector2 screenDimensions = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            handCursor = new DrawableAsset<Texture2D>(Vector2.Zero, new Vector2(0, -1), new Vector2(screenDimensions.X / 8.0f, screenDimensions.X / 8.0f), cursor);
        }

        public void SetSelectionSprite(Animation2D selectionSprite)
        {
            Vector2 screenDimensions = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            selectionLoader = new DrawableAsset<Animation2D>(Vector2.Zero, new Vector2(0, -1), new Vector2(screenDimensions.X / 4.0f, screenDimensions.X / 4.0f), selectionSprite);
        }

        protected virtual void AddMenuEntry(string text, InputHandler callback, bool lockEntry = false, float scale = 1.0f)
        {
            //float wScale = (float)ScreenManager.GraphicsDevice.Viewport.Width / 1920.0f;
            //float hScale = (float)ScreenManager.GraphicsDevice.Viewport.Height / 1200.0f;
            //scale *= wScale;
            //padding *= hScale;
            Vector2 pos;
            if (entries.Count == 0)
                pos = new Vector2(0, title.Position.Y + title.Size.Y / 2 + padding);
            else
                pos = new Vector2(0, entries[entries.Count - 1].Position.Y + entries[entries.Count - 1].Size.Y / 2 + padding);
            Vector2 screenDimensions = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            float newScale = (screenDimensions.X / 1920.0f) * scale;
            MenuEntry newEntry = new MenuEntry(text, pos, newScale, textColor, callback);
            if (lockEntry)
                newEntry.Lock();

            if (hoverIndex == -1 && !newEntry.IsLocked())
            {
                hoverIndex = 0;
                newEntry.SetEffect(TextEffect.Bubble);
            }
            entries.Add(newEntry);
            language.Add(text);
        }

        #endregion

    }


}
