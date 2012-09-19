using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using Ecslent2D;
using Ecslent2D.Input;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
using Ecslent2D.ScreenManagement;

namespace KinectSnake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public class Main : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        public static ScreenManager screenManager;
        public static GameManager gameManager;
        public static DiagnosticManager diagonsticManager;

        public static Random numGenerator;
        public static Dictionary<string, Texture2D> sprites,normals, speculars;
        public static Dictionary<string, Animation2D> animatedSprites;

        public static Effect multiTarget,transition;
        public static List<Texture2D> bloodStains, stars, clouds;
        public static Vector2 windowSize,windowScale;
        public static Rectangle screenRect;
        public static List<SoundEffect> backgroundSounds;
        public static List<SoundEffectInstance> backgroundLoops;
        public static List<SoundEffect> killSounds;
        public static bool quit;

        public Main()
        {

            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);
            screenManager = new ScreenManager(this);
            gameManager = new GameManager();
            diagonsticManager = new DiagnosticManager(this);
            this.Components.Add(screenManager);
            this.Components.Add(diagonsticManager);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(LoadFullResolution);
            Content.RootDirectory = "Content";
            numGenerator = new Random();
            bloodStains = new List<Texture2D>();
            stars = new List<Texture2D>();
            clouds = new List<Texture2D>();
            sprites = new Dictionary<string, Texture2D>();
            normals = new Dictionary<string, Texture2D>();
            speculars = new Dictionary<string, Texture2D>();
            animatedSprites = new Dictionary<string, Animation2D>();
            killSounds = new List<SoundEffect>();
            backgroundSounds = new List<SoundEffect>();
            backgroundLoops = new List<SoundEffectInstance>();
        }

        void LoadFullResolution(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            screenRect = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            windowSize = new Vector2((float)Window.ClientBounds.Width, (float)Window.ClientBounds.Height);
            windowScale = new Vector2((float)Window.ClientBounds.Width / 1920.0f, (float)Window.ClientBounds.Height / 1200.0f);
            quit = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            TextSettings.AddFont("snakeFont", content.Load<SpriteFont>("Content\\Fonts\\snake"));
            gameManager.LoadSettings("alex");
            sprites.Add("menuBack", content.Load<Texture2D>("Content\\Images\\back"));
            sprites.Add("grass", content.Load<Texture2D>("Content\\Images\\playField"));
            sprites.Add("snakeHead",  content.Load<Texture2D>("Content\\Images\\snakeHead"));
            sprites.Add("snakeBody",  content.Load<Texture2D>("Content\\Images\\snakeBody"));
            sprites.Add("snakeTail",  content.Load<Texture2D>("Content\\Images\\snakeTail"));
            sprites.Add("handCursor",  content.Load<Texture2D>("Content\\Images\\handCursor"));
            sprites.Add("gridSquare", content.Load<Texture2D>("Content\\Images\\gridSquare"));
            sprites.Add("filledGrid", content.Load<Texture2D>("Content\\Images\\filledGrid"));
            sprites.Add("helpBack", content.Load<Texture2D>("Content\\Images\\helpBack"));
            sprites.Add("points", content.Load<Texture2D>("Content\\Images\\points"));
            sprites.Add("quadrant", content.Load<Texture2D>("Content\\Images\\quadrant"));
            sprites.Add("transition", content.Load<Texture2D>("Content\\Images\\transition"));
            sprites.Add("ecsl", content.Load<Texture2D>("Content\\Images\\ecsl"));
            sprites.Add("lock", content.Load<Texture2D>("Content\\Images\\lock"));
            sprites.Add("unlock", content.Load<Texture2D>("Content\\Images\\unlock"));
            sprites.Add("leftGesture", content.Load<Texture2D>("Content\\Images\\left"));
            sprites.Add("rightGesture", content.Load<Texture2D>("Content\\Images\\right"));
            sprites.Add("upGesture", content.Load<Texture2D>("Content\\Images\\up"));
            sprites.Add("downGesture", content.Load<Texture2D>("Content\\Images\\down"));
            sprites.Add("playGesture", content.Load<Texture2D>("Content\\Images\\play"));
            sprites.Add("pauseGesture", content.Load<Texture2D>("Content\\Images\\pause"));
            sprites.Add("confirmGesture", content.Load<Texture2D>("Content\\Images\\confirm"));
            sprites.Add("cancelGesture", content.Load<Texture2D>("Content\\Images\\cancel"));

            bloodStains.Add(content.Load<Texture2D>("Content\\Images\\bloodStainOne"));
            bloodStains.Add(content.Load<Texture2D>("Content\\Images\\bloodStainTwo"));
            bloodStains.Add(content.Load<Texture2D>("Content\\Images\\bloodStainThree"));
            bloodStains.Add(content.Load<Texture2D>("Content\\Images\\bloodStainFour"));
            bloodStains.Add(content.Load<Texture2D>("Content\\Images\\bloodStainFive"));
            clouds.Add(content.Load<Texture2D>("Content\\Images\\cloudOne"));
            clouds.Add(content.Load<Texture2D>("Content\\Images\\cloudTwo"));
            clouds.Add(content.Load<Texture2D>("Content\\Images\\cloudThree"));
            clouds.Add(content.Load<Texture2D>("Content\\Images\\cloudFour"));
            clouds.Add(content.Load<Texture2D>("Content\\Images\\cloudFive"));
            clouds.Add(content.Load<Texture2D>("Content\\Images\\cloudSix"));

            animatedSprites.Add("mouse",  content.Load<Animation2D>("Content\\Images\\mouse"));
            animatedSprites.Add("rabbit",  content.Load<Animation2D>("Content\\Images\\rabbit"));
            animatedSprites.Add("snakeEating",   content.Load<Animation2D>("Content\\Images\\snakeEating"));
            animatedSprites.Add("bloodSplatter",   content.Load<Animation2D>("Content\\Images\\bloodSplatter"));
            animatedSprites.Add("load", content.Load<Animation2D>("Content\\Images\\load"));
            animatedSprites.Add("mouse_NRM", content.Load<Animation2D>("Content\\Images\\Normals\\mouse"));
            animatedSprites.Add("rabbit_NRM", content.Load<Animation2D>("Content\\Images\\Normals\\rabbit"));
            animatedSprites.Add("rabbit_SPEC", content.Load<Animation2D>("Content\\Images\\Speculars\\rabbit"));
            animatedSprites.Add("mouse_SPEC", content.Load<Animation2D>("Content\\Images\\Speculars\\mouse"));

            normals.Add("grass",content.Load<Texture2D>("Content\\Images\\Normals\\grass"));
            normals.Add("snakeHead", content.Load<Texture2D>("Content\\Images\\Normals\\snakeHead"));
            normals.Add("snakeBody",content.Load<Texture2D>("Content\\Images\\Normals\\snakeBody"));
            normals.Add("snakeTail",content.Load<Texture2D>("Content\\Images\\Normals\\snakeTail"));

            speculars.Add("grass", content.Load<Texture2D>("Content\\Images\\Speculars\\grass"));
            speculars.Add("snakeHead", content.Load<Texture2D>("Content\\Images\\Speculars\\snakeHead"));
            speculars.Add("snakeBody", content.Load<Texture2D>("Content\\Images\\Speculars\\snakeBody"));
            speculars.Add("snakeTail", content.Load<Texture2D>("Content\\Images\\Speculars\\snakeTail"));


            multiTarget = content.Load<Effect>("Content\\Shaders\\MultiTarget");
            transition = content.Load<Effect>("Content\\Shaders\\Transition");

            killSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\killOne"));
            killSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\killTwo"));
            killSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\killThree"));
            killSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\killFour"));
            killSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\killFive"));
            killSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\killSix"));

            backgroundSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\mouseSqueaks"));
            backgroundSounds.Add(content.Load<SoundEffect>("Content\\Sounds\\birdChirps"));
            foreach( SoundEffect sound in backgroundSounds)
            {
                backgroundLoops.Add(sound.CreateInstance());
                backgroundLoops[backgroundLoops.Count-1].IsLooped= true;
            }

            diagonsticManager.LoadContent();
            screenManager.AddScreen(new Screens.MainMenuScreen(), null);
            if (true) //screenManager.input.Kinect.IsEnabled())
            {
                Screens.DominantHandSelectionScreen welcome = new Screens.DominantHandSelectionScreen();
                welcome.Right += (o, e) =>
                {
                    InputState.DominantSide = DominantSide.Right;
                };

                welcome.Left += (o, e) =>
                {
                    InputState.DominantSide = DominantSide.Left;
                };
                screenManager.AddScreen(welcome, null);
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /// 
        protected override void UnloadContent()
        {
            // TODO: Unload any ResourceManagementMode.Automatic content
            //if (kinectSensor != null)
            //{

                //KinectInput.listener.Abort();
                //KinectInput.listener.Join();
                //kinectSensor.Stop();
                //kinectSensor.Dispose();
            //}
            content.Unload();
            screenManager.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || quit)
            {
                this.Exit();
                screenManager.Dispose();
            }


            else
            {
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }
    }
}
