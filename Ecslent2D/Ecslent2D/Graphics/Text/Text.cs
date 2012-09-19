using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ecslent2D.Graphics.Text
{
    public enum TextEffect
    {
        None,
        Bubble,
        Fade
    }
    public static class TextSettings
    {
        private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
        private static string currentFont = null;

        public static SpriteFont CurrentFont
        {
            get
            {
                return fonts[currentFont];
            }
        }
        public static void SetCurrentFont(string name)
        {
            currentFont = name;
        }
        public static void AddFont(string name, SpriteFont font)
        {
            fonts.Add(name,font);
            if (currentFont == null)
                currentFont = name;
        }
        public static void RemoveFont(string name)
        {
            if (fonts.ContainsKey(name))
                fonts.Remove(name);
        }
    }
    public class Text : GraphicalAsset
    {
        public float sScale, alpha, maxAlpha, rotationIncrement, cooldown;
        public TextEffect effect;
        public string text;
        private Color mainColor, altColor, currentColor;
        protected float internalScale;

        public Text(string text, Vector2 position, TextEffect effect = TextEffect.None, float alpha = 0.7f)
            : base(position, new Vector2(1, 0), TextSettings.CurrentFont.MeasureString(text))
        {
            this.text = text;
            this.scale = this.sScale = 1.0f;
            this.effect = effect;
            this.mainColor = Color.White;
            this.altColor = Color.Yellow;
            this.maxAlpha = this.alpha = alpha;
            this.rotation = 0.0f;
            this.rotationIncrement = 0.001f;
            this.currentColor = this.mainColor;
            cooldown = 3.0f;
        }
        public Text(string text,  Vector2 position, float scale, Color color, TextEffect effect = TextEffect.None, float alpha = 1.0f)
            : base(position, new Vector2(1, 0), TextSettings.CurrentFont.MeasureString(text) * scale)
        {
            this.text = text;
            this.scale = this.sScale =  scale;
            this.effect = effect;
            this.mainColor = color;
            this.altColor = Color.Yellow;
            this.maxAlpha = this.alpha = alpha;
            this.rotation = 0.0f;
            this.rotationIncrement = 0.001f;
            this.currentColor = this.mainColor;
            cooldown = 3.0f;
        }

        public void SetMainColor(Color color)
        {
            this.mainColor = color;
        }

        
        public void SetAlternateColor(Color color)
        {
            this.altColor = color;
        }
        public string String
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.Size = TextSettings.CurrentFont.MeasureString(value) * scale;
                this.currentColor = this.mainColor;
                cooldown = 3.0f;
            }
        }

        public virtual void Update(GameTime gTime)
        {
            cooldown -= gTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (this.effect == TextEffect.Bubble)
            {
                this.scale = this.sScale + 1.0f + ((float)Math.Sin(gTime.TotalGameTime.TotalSeconds * 5) + 1) * 0.05f;
                this.currentColor = this.altColor;
                //this.alpha = 0.3f + ((float)Math.Sin(gTime.TotalGameTime.TotalSeconds * 5) + 1) * this.maxAlpha;
                if (this.rotation < -0.1f || this.rotation > 0.1f)
                    this.rotationIncrement = -this.rotationIncrement;

                this.Size = TextSettings.CurrentFont.MeasureString(this.text) * this.scale;
                this.rotation += this.rotationIncrement;
            }
            else if (this.effect == TextEffect.Fade)
            {
                if (cooldown < 2.0f)
                    this.alpha = cooldown / 2.0f;
                else
                    this.alpha = 0.8f;
                this.currentColor = this.mainColor * this.alpha;

            }
            else
            {
                this.scale = this.sScale;
                this.Size = TextSettings.CurrentFont.MeasureString(this.text) * this.scale;
                this.currentColor = this.mainColor * this.alpha;
                //this.alpha = this.maxAlpha;
                this.rotation = 0.0f;
            }
        }


        public void SetEffect(TextEffect e)
        {
            this.effect = e;
        }
        public override void Draw(SpriteBatch batch)
        {

            batch.DrawString(TextSettings.CurrentFont, this.text, this.Position + new Vector2(1, 1), Color.Black* this.alpha, this.rotation, this.Size/this.scale * 0.5f, this.scale, SpriteEffects.None, 0.0f);
            batch.DrawString(TextSettings.CurrentFont, this.text, this.Position, this.currentColor, this.rotation, this.Size / this.scale * 0.5f, this.scale, SpriteEffects.None, 0.0f);
        }
    }
    

    


}
