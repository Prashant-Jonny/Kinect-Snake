using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ecslent2D.Graphics
{
    public interface IGraphicalAsset
    {
        float scale { get; set; }
        float rotation { get; set; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        Vector2 Size { get; set; }
        Rectangle boundry { get; }
    }

    public class GraphicalAsset : IGraphicalAsset
    {
        protected Vector2 position, direction, size;
        public float rotation { get; set; }
        public float scale { get; set; }
        public Rectangle boundry { get; set; }

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (value.GetType() == typeof(Vector2))
                {
                    this.position = value;
                    this.boundry = new Rectangle((int)this.position.X - (int)size.X / 2, (int)this.position.Y - (int)size.Y / 2, (int)size.X, (int)size.Y);
                }
            }
        }

        public Vector2 Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                if (value.GetType() == typeof(Vector2))
                {
                    this.direction = value;
                    this.rotation = (float)Math.Atan2(this.direction.Y, this.direction.X) + (float)(Math.PI / 2.0);
                }
            }
        }
        public Vector2 Size
        {
            get
            {
                return this.size;
            }
            set
            {
                if (value.GetType() == typeof(Vector2))
                {
                    this.size = value;
                    this.boundry = new Rectangle((int)this.position.X - (int)size.X / 2, (int)this.position.Y - (int)size.Y / 2, (int)size.X, (int)size.Y);
                }
            }
        }

        public GraphicalAsset(Vector2 position, Vector2 direction, Vector2 size)
        {
            this.rotation = 0.0f;
            this.scale = 1.0f;
            this.Size = size;
            this.Position = position;
            this.Direction = direction;
        }

        

        public virtual void Draw(SpriteBatch batch) { }


    }
}
