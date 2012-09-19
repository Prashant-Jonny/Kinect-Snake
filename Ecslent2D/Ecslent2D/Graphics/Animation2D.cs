namespace Ecslent2D.Graphics
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;


    public class Animation2D : Texture2D
    {
        private int m_CurrentFrame;
        private long m_CurrentTick;
        private bool m_IsPaused;
        private bool m_IsStopped;
        private Texture2D[] m_Textures;


        public int CurrentFrame
        {
            get
            {
                return this.m_CurrentFrame;
            }
        }

        public int FrameCount
        {
            get
            {
                return this.m_Textures.Length;
            }
        }

        public new int Height
        {
            get
            {
                return this.m_Textures[0].Height;
            }
        }

        public new int Width
        {
            get
            {
                return this.m_Textures[0].Width;
            }
        }

        private Animation2D(GraphicsDevice device, int width, int height)
            :base(device,width,height)
        {
        }
        public static Animation2D FromTextures(Texture2D[] frames)
        {
            return new Animation2D(frames[0].GraphicsDevice, frames[0].Width, frames[0].Height) { m_Textures = frames };
        }

        public Texture2D GetTexture()
        {
            return this.m_Textures[this.m_CurrentFrame];
        }

        public Texture2D GetTexture(int frameIndex)
        {
            return this.m_Textures[frameIndex];
        }

        public void Pause()
        {
            this.m_IsPaused = true;
        }

        public void Play()
        {
            this.m_CurrentFrame = 0;
            this.m_IsPaused = false;
            this.m_IsStopped = false;
        }

        public void Resume()
        {
            this.m_IsPaused = false;
        }

        public void Stop()
        {
            this.m_CurrentFrame = 0;
            this.m_IsPaused = false;
            this.m_IsStopped = true;
        }

        public override string ToString()
        {
            return string.Format("Playing frame {0} of {1} -- {2:F}%", this.m_CurrentFrame, this.FrameCount, (100f * this.m_CurrentFrame) / ((float) this.FrameCount));
        }

        public void Update(long elapsedTicks)
        {
            if (!this.m_IsPaused && !this.m_IsStopped)
            {
                this.m_CurrentTick += elapsedTicks;
                if (this.m_CurrentTick >= 0xf4240L)
                {
                    this.m_CurrentTick = 0L;
                    this.m_CurrentFrame++;
                    if (this.m_CurrentFrame >= this.m_Textures.Length)
                    {
                        this.m_CurrentFrame = 0;
                    }
                }
            }
        }
        public void GetData<T>(T[][] container)
            where T: struct
        {
            for (int i = 0; i < FrameCount; i++)
            {
                this.m_Textures[i].GetData<T>(container[i]);
            }
        }

        public void SetData<T>(T[][] container)
            where T : struct
        {
            for (int i = 0; i < FrameCount; i++)
            {
                this.m_Textures[i].SetData<T>(container[i]);
            }
        }
        public new Rectangle Bounds
        {
            get
            {
                return GetTexture().Bounds;
            }
        }
    }
}

