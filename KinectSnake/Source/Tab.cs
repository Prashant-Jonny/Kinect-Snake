using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;

namespace KinectSnake
{
    public class Tab : DrawableAsset<Texture2D>
    {
        protected Text title;
        protected List<IconText> items;
        protected bool enabled = false;
        protected float padding = 50.0f;
        public Tab(string title, Vector2 position)
            : base(position, new Vector2(0, -1), Main.windowSize * 0.5f)
        {
            this.title = new Text(title, position, 1.0f, Color.Blue);
            items = new List<IconText>();
        }
        public void Add(string text,Color color, float scale, Texture2D iconSprite, TextAlignment alignment)
        {
            Vector2 pos;
            if (items.Count == 0)
                pos = new Vector2(Main.windowSize.X * 0.5f, title.Position.Y + title.Size.Y / 2 + padding);
            else
                pos = new Vector2(Main.windowSize.X * 0.5f, items[items.Count - 1].Position.Y + items[items.Count - 1].Size.Y / 2 + padding);
            items.Add(new IconText(text, iconSprite, new Vector2(200, 200), pos, scale, color, alignment));
        }

        public void Clear()
        {
            items.Clear();
        }
        public void Enable()
        {
            title.effect = TextEffect.Bubble;
            enabled = true;
        }
        public void Disable()
        {
            title.effect = TextEffect.None;
            enabled = false;
        }
        public void Update(GameTime gTime)
        {
            title.Update(gTime);
            foreach (IconText item in items)
                item.Update(gTime);
        }
        public override void Draw(SpriteBatch batch)
        {
            title.Draw(batch);
            if (enabled)
            {
                foreach (IconText item in items)
                    item.Draw(batch);
            }
        }

    }

    public class HelpPanel : DrawableAsset<Texture2D>
    {
        public List<Tab> tabs;
        protected int currentIndex = 0;
        protected DrawableAsset<Texture2D> back;
        protected Text title, exit;
        protected List<DrawableAsset<Texture2D>> clouds;
        public HelpPanel(string name)
            : base(Main.windowSize * 0.25f, new Vector2(0, -1), Main.windowSize * 0.5f)
        {
            title = new Text(name,new Vector2(Main.windowSize.X * 0.5f, TextSettings.CurrentFont.MeasureString(name).Y / 2.0f + 75), 2.0f, Color.PaleGoldenrod);
            exit = new Text("Press Enter to Start!", new Vector2(Main.windowSize.X * 0.5f, Main.windowSize.Y - TextSettings.CurrentFont.MeasureString("Press Enter to Start!").Y / 2.0f - 75), 2.0f, Color.Green);
            tabs = new List<Tab>();
            tabs.Add(new Tab("Instructions", new Vector2(Main.windowSize.X * 0.25f, title.Position.Y + TextSettings.CurrentFont.MeasureString("Instructions").Y / 2.0f + 75)));
            tabs.Add(new Tab("Controls", new Vector2(Main.windowSize.X * 0.75f, title.Position.Y + TextSettings.CurrentFont.MeasureString("Controls").Y / 2.0f + 75)));
            back = new DrawableAsset<Texture2D>(Main.windowSize * 0.25f, new Vector2(0, -1), Main.windowSize * 0.5f, Main.sprites["darkcurrentColor"]);
            tabs[0].Enable();
            clouds = new List<DrawableAsset<Texture2D>>();
            for (int i = 0; i < 100; i++)
            {
                Vector2 pos = new Vector2(Main.numGenerator.Next() % Main.windowSize.X, Main.numGenerator.Next() % Main.windowSize.Y);
                Texture2D sprite = Main.clouds[Main.numGenerator.Next() % (Main.clouds.Count - 1)];
                clouds.Add(new DrawableAsset<Texture2D>(pos, new Vector2(0, -1), new Vector2(sprite.Width, sprite.Height), sprite));
            }
        }

        public void Next()
        {
            tabs[currentIndex].Disable();
            currentIndex++;
            if (currentIndex > tabs.Count - 1)
                currentIndex = 0;
            tabs[currentIndex].Enable();
        }

        public void Previous()
        {
            tabs[currentIndex].Disable();
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = tabs.Count - 1;
            tabs[currentIndex].Enable();
        }

        public override void Draw(SpriteBatch batch)
        {
            foreach (DrawableAsset<Texture2D> cloud in clouds)
                cloud.Draw(batch);
            back.Draw(batch);
            title.Draw(batch);
            foreach (Tab tab in tabs)
                tab.Draw(batch);
            exit.Draw(batch);
        }
        public void Update(GameTime gTime)
        {
            title.Update(gTime);
            exit.Update(gTime);
            foreach (Tab tab in tabs)
                tab.Update(gTime);
            for (int i = 0; i < clouds.Count; i++)
            {
                if (i < clouds.Count / 3 - 1)
                    clouds[i].Position += new Vector2(3, 0);
                else if (i < 2 * clouds.Count / 3)
                    clouds[i].Position += new Vector2(2, 0);
                else
                    clouds[i].Position += new Vector2(1, 0);
                if (clouds[i].Position.X > Main.windowSize.X + clouds[i].Size.X / 2)
                    clouds[i].Position = new Vector2(-clouds[i].Size.X / 2, clouds[i].Position.Y);
            }
        }

        public Tab Instructions { get { return tabs[0]; } }

        public Tab Controls { get { return tabs[1]; } }
    }
}
