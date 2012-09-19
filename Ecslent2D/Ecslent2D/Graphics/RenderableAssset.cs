using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ecslent2D.Graphics
{

    public interface IRenderableAsset : IDrawableAsset
    {
        void DrawColorMap(SpriteBatch batch);
        void DrawSpecularMap(SpriteBatch batch);
        void DrawNormalMap(SpriteBatch batch);

    }

    public class RenderableAsset<T> : DrawableAsset<T>, IRenderableAsset
        where T : Texture2D
    {
        protected T specularMap, normalMap;


        public T SpecularMap
        {
            get
            {
                return this.specularMap;
            }
            set
            {
                if (value.GetType() == typeof(T))
                {
                    this.specularMap = value;
                    this.imgScale = this.Size.X / (float)this.specularMap.Width;
                }
            }
        }
        public T NormalMap
        {
            get
            {
                return this.normalMap;
            }
            set
            {
                if (value.GetType() == typeof(T))
                {
                    this.normalMap = value;
                    this.imgScale = this.Size.X / (float)this.normalMap.Width;
                }
            }
        }

        public RenderableAsset(Vector2 position, Vector2 direction, Vector2 size)
            : base(position, direction, size)
        {
        }
        public RenderableAsset(Vector2 position, Vector2 direction, Vector2 size, T color, T specular, T normal)
            : base(position, direction, size)
        {
            this.ColorMap = color;
            this.specularMap = specular;
            this.normalMap = normal;
        }
        public void SetMaps(T color, T specular, T normal)
        {
            this.ColorMap = color;
            this.specularMap = specular;
            this.normalMap = normal;

        }

        public override void Draw(SpriteBatch batch)
        {
            DrawColorMap(batch);
        }

        public virtual void DrawColorMap(SpriteBatch batch)
        {
            if (animated)
                batch.Draw((this.ColorMap as Animation2D).GetTexture(), this.Position, null, this.overlay, this.rotation, new Vector2(this.ColorMap.Width, this.ColorMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
            else
                batch.Draw(this.ColorMap, this.Position, null, this.overlay, this.rotation, new Vector2(this.ColorMap.Width, this.ColorMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
        }
        public virtual void DrawSpecularMap(SpriteBatch batch)
        {
            if (animated)
                batch.Draw((this.SpecularMap as Animation2D).GetTexture(), this.Position, null, this.overlay, this.rotation, new Vector2(this.ColorMap.Width, this.ColorMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
            else
                batch.Draw(this.SpecularMap, this.Position, null, this.overlay, this.rotation, new Vector2(this.SpecularMap.Width, this.SpecularMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
        }
        public virtual void DrawNormalMap(SpriteBatch batch)
        {
            if (animated)
                batch.Draw((this.NormalMap as Animation2D).GetTexture(), this.Position, null, this.overlay, this.rotation, new Vector2(this.ColorMap.Width, this.ColorMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
            else
                batch.Draw(this.NormalMap, this.Position, null, this.overlay, this.rotation, new Vector2(this.NormalMap.Width, this.NormalMap.Height) * 0.5f, this.imgScale * this.scale, SpriteEffects.None, 0f);
        }

    }
}
