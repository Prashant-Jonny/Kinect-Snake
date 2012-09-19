using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ecslent2D.Graphics.Text
{
    public enum TextAlignment
    {
        Left,
        Right,
        Center,
        Top,
        Bottom
    }
    public class IconText : GraphicalAsset
    {
        public IDrawableAsset icon;
        public Text text;
        bool showIcon = true;
        TextAlignment alignment;


        public IconText(string t, Texture2D iconSprite, Vector2 iconSize, Vector2 position, float scale, Color color, TextAlignment alignment, TextEffect effect = TextEffect.None, float alpha = 1.0f)
            :base(position,new Vector2(0,-1),Vector2.Zero)
        {
            text = new Text(t,  position, scale, color, effect);
            icon = new DrawableAsset<Texture2D>(Vector2.Zero, new Vector2(0, -1), iconSize, iconSprite);

            this.alignment = alignment;
            UpdateAlignment();
        }

        public void HideIcon()
        {
            showIcon = false;
        }
        public void ShowIcon()
        {
            showIcon = true;
        }
        public void UpdateAlignment()
        {
            if (alignment == TextAlignment.Left || alignment == TextAlignment.Right)
            {
                this.Size = new Vector2(text.Size.X + icon.Size.X, Math.Max(text.Size.Y, icon.Size.Y));
                if (alignment == TextAlignment.Left)
                {
                    icon.Position = new Vector2(this.Position.X + (this.Size.X * 0.5f - icon.Size.X * 0.5f), this.Position.Y);
                    text.Position = new Vector2(this.Position.X - (this.Size.X * 0.5f - text.Size.X * 0.5f), this.Position.Y);
                }
                else
                {
                    icon.Position = new Vector2(this.Position.X - (this.Size.X * 0.5f - icon.Size.X * 0.5f), this.Position.Y);
                    text.Position = new Vector2(this.Position.X + (this.Size.X * 0.5f - text.Size.X * 0.5f), this.Position.Y);
                }
            }
            else if (alignment == TextAlignment.Top || alignment == TextAlignment.Bottom)
            {
                this.Size = new Vector2(Math.Max(text.Size.X, icon.Size.X), text.Size.Y + icon.Size.Y );
                if (alignment == TextAlignment.Top)
                {
                    icon.Position = new Vector2(this.Position.X, this.Position.Y + (this.Size.Y * 0.5f - icon.Size.Y * 0.5f));
                    text.Position = new Vector2(this.Position.X,this.Position.Y - (this.Size.Y * 0.5f - text.Size.Y * 0.5f) );
                }
                else
                {
                    icon.Position = new Vector2(this.Position.X, this.Position.Y - (this.Size.Y * 0.5f - icon.Size.Y * 0.5f));
                    text.Position = new Vector2(this.Position.X,this.Position.Y + (this.Size.Y * 0.5f - text.Size.Y * 0.5f) );
                }
            }

        }

        public void Update(GameTime gTime)
        {
            text.Update(gTime);
            UpdateAlignment();
        }
        public override void Draw(SpriteBatch batch)
        {
            if (showIcon)
                icon.Draw(batch);
            text.Draw(batch);
        }
    }
}
