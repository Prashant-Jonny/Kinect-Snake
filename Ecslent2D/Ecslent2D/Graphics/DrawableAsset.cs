using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ecslent2D.Graphics
{
    public interface IDrawableAsset : IGraphicalAsset
    {
        void Draw(SpriteBatch batch);
    }

    public class DrawableAsset<T> : GraphicalAsset, IDrawableAsset
        where T : Texture2D
    {
        public Color overlay = Color.White;
        protected T colorMap = null;
        public bool animated = false;
        protected float imgScale = 1.0f;

        public virtual T ColorMap
        {
            get
            {
                return this.colorMap;
            }
            set
            {
                if (value.GetType() == typeof(T))
                {
                    this.colorMap = value;
                    this.imgScale = this.Size.X / (float)this.colorMap.Width;
                }
            }
        }


        public new Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                if (value.GetType() == typeof(Vector2))
                {
                    base.Size = value;
                    this.boundry = new Rectangle((int)this.position.X - (int)size.X / 2, (int)this.position.Y - (int)size.Y / 2, (int)size.X, (int)size.Y);
                    if (ColorMap != null)
                        this.imgScale = this.size.X / (float)ColorMap.Width;
                }
            }
        }

        public DrawableAsset(Vector2 position, Vector2 direction, Vector2 size)
            : base(position, direction, size) { }

        public DrawableAsset(Vector2 position, Vector2 direction, Vector2 size, T img)
            : base(position, direction, size)
        {
            this.Size = size;
            this.Position = position;
            this.Direction = direction;
            this.colorMap = img;
            this.imgScale = this.Size.X / (float)this.colorMap.Width;
            if (img.GetType() == typeof(Animation2D))
                this.animated = true;
        }

        public bool Intersects(IDrawableAsset other)
        {
            return this.boundry.Intersects(other.boundry);
        }

        public override void Draw(SpriteBatch batch)
        {
            if (animated)
                batch.Draw((this.ColorMap as Animation2D).GetTexture(), this.Position, null, this.overlay, this.rotation, new Vector2(this.ColorMap.Width, this.ColorMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
            else
                batch.Draw(this.ColorMap, this.Position, null, this.overlay, this.rotation, new Vector2(this.ColorMap.Width, this.ColorMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
        }




    }
}
