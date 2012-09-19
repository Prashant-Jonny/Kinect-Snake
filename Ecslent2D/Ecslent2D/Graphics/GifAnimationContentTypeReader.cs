namespace Ecslent2D.Graphics
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using Microsoft.Xna.Framework;

    public sealed class GifAnimationContentTypeReader : ContentTypeReader<Animation2D>
    {
        protected override Animation2D Read(ContentReader input, Animation2D existingInstance)
        {
            int num = input.ReadInt32();
            Texture2D[] frames = new Texture2D[num];
            IGraphicsDeviceService service = (IGraphicsDeviceService) input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            if (service == null)
            {
                throw new ContentLoadException();
            }
            GraphicsDevice graphicsDevice = service.GraphicsDevice;
            if (graphicsDevice == null)
            {
                throw new ContentLoadException();
            }
            for (int i = 0; i < num; i++)
            {
                SurfaceFormat format = (SurfaceFormat) input.ReadInt32();
                int width = input.ReadInt32();
                int height = input.ReadInt32();
                int numberLevels = input.ReadInt32();
                frames[i] = new Texture2D(graphicsDevice, width, height, false, format);
                for (int j = 0; j < numberLevels; j++)
                {
                    int count = input.ReadInt32();
                    byte[] data = input.ReadBytes(count);

                    Color[] colors = new Color[width * height];

                    // Go through each pixel and set the bytes correctly
                    // Remember, each pixel got a Rad, Green and Blue
                    int index = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, index += 4)
                        {
                            colors[y * width + x] = new Color(data[index + 2], data[index + 1], data[index],data[index+3]);
                        }
                    }
                    frames[i].SetData(colors);
                }
            }
            input.Close();
            return Animation2D.FromTextures(frames);
        }
    }
}

