using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Speech.Recognition;
using Ecslent2D.Collision;
using Ecslent2D.Graphics;
using Ecslent2D.Graphics.Text;
namespace KinectSnake
{
    
    public class EffectSprite:DrawableAsset<Animation2D>
    {
        protected bool done = false;
        protected IGridAsset parent;
        protected Vector2 startPos, offset;
        public EffectSprite(Vector2 position, Vector2 offset, Vector2 size)
            : base(position + offset, new Vector2(0,-1), size)
        {
            startPos = position + offset;
        }
        public EffectSprite(IGridAsset parent, Vector2 offset, Vector2 size)
            : base(parent.Position + offset, parent.Direction, size)
        {
            this.parent = parent;
        }

        public EffectSprite(IGridAsset parent, Vector2 offset, Vector2 size, Animation2D sprite)
            : base(parent.Position + offset, parent.Direction, size, sprite)
        {
            this.parent = parent;
            animated = true;
        }

        public EffectSprite(Vector2 position, Vector2 offset, Vector2 size, Animation2D sprite)
            : base(position + offset, position, size, sprite)
        {
            startPos = position + offset;
            animated = true;
        }

        public virtual void Start()
        {
            if (animated)
                this.ColorMap.Play();
        }
        public virtual void Stop()
        {
            this.done = true;
            if (animated)
                this.ColorMap.Stop();
        }
        public virtual void Update(GameTime gTime)
        {
            if (animated)
            {
                this.ColorMap.Update(gTime.ElapsedGameTime.Ticks);
                if (this.ColorMap.CurrentFrame == this.ColorMap.FrameCount - 1)
                {
                    this.done = true;
                    this.Stop();
                }
            }
            this.Position = this.parent.Position + this.parent.Direction *this.offset ;

            this.rotation = this.parent.rotation;
        }
        public virtual bool IsDone()
        {
            return done;
        }
    }

    public class PointExplosion : EffectSprite
    {
        List<Text> pointText;
        Vector2[] pointDirections;
        float duration = 3.0f, timeActive = 0.0f, speed = 1.0f;
        public PointExplosion(Vector2 startPos, int points, Color color)
            : base(startPos, Vector2.Zero, new Vector2(500, 500))
        {
            pointText = new List<Text>();
            this.Size *= Main.windowSize.X / 1920.0f;
            pointDirections = new Vector2[4]{new Vector2(0,-1),new Vector2(0,1),new Vector2(-1,0),new Vector2(1,0)};
            this.speed = 1.0f;
            for (int i = 0; i < 4; i++)
            {
                pointText.Add(new Text(points.ToString(), startPos, 0.75f, color, TextEffect.Fade));
            }
        }

        public override void Start()
        {
            this.timeActive = 0.0f;
            foreach (Text p in pointText)
            {
                p.Position = startPos;
            }
        }
        public override void Stop()
        {
            this.done = true;
            this.timeActive = 0.0f;
        }
        public override void Update(GameTime gTime)
        {
            this.timeActive += gTime.ElapsedGameTime.Milliseconds / 1000.0f;
            for (int i = 0; i < pointText.Count; i ++)
            {

                pointText[i].Position += pointDirections[i] * speed;
                pointText[i].Update(gTime);
            }
            if (timeActive >= duration)
                this.done = true;
        }
        public override bool IsDone()
        {
            return done;
        }
        public override void Draw(SpriteBatch batch)
        {
            foreach (Text p in pointText)
                p.Draw(batch);
        }
    
    }

    public class Scheme
    {
        public List<Lights.Light> lights;
        public Vector3 startPos, endPos, activeStartPos;
        public Color ambientLight;
        public bool reachedEnd, affectAmbient;
        public Lights.Light main = null;
        public float speed;

        public Scheme(bool affectAmbient = true)
        {
            this.startPos = new Vector3(0 - Main.screenRect.Width * 1.5f, Main.screenRect.Height / 2, 100);
            this.activeStartPos = new Vector3(0, Main.screenRect.Height / 2, 100);
            this.endPos = new Vector3(Main.screenRect.Width * 2.5f, Main.screenRect.Height / 2, 100);
            ambientLight = new Color(0.75f, 0.75f, 0.75f);
            lights = new List<Lights.Light>();
            this.affectAmbient = affectAmbient;
            speed = 0.05f;
        }
        public void AddLight(Vector3 pos, Vector4 color, float power, int lightDecay, bool enabled, Texture2D t = null)
        {
            Lights.PointLight newLight = new Lights.PointLight()
            {
                Color = color,
                Power = power,
                LightDecay = lightDecay,
                Position = pos,
                IsEnabled = enabled,
                Overlay = t
            };
            if (lights.Count == 0)
                this.main = newLight;
            lights.Add(newLight);
        }
        public void Update(GameTime gTime)
        {
            if (main.Position.X >= Main.screenRect.Width * 2.5f)
                reachedEnd = true;
            else
                foreach (Lights.Light light in lights)
                    light.Position = new Vector3(light.Position.X + speed, light.Position.Y, light.Position.Z);
            if (affectAmbient)
            {
                float ambientPower = 0.9f - Math.Min(0.8f, ((Math.Abs(main.Position.X - Main.screenRect.Center.X) / (3 * Main.screenRect.Width / 2))));
                ambientLight = new Color(ambientPower, ambientPower, ambientPower);
            }
        }

        public void Reset(bool active)
        {
            if (active)
                main.Position = activeStartPos;
            else
                main.Position = startPos;
        }
    }

    public class Landscape : RenderableAsset<RenderTarget2D>
    {
        private List<IRenderableAsset> components;
        private RenderTarget2D[] stainTargets;
        private int currentStain = 0;

        public VertexPositionColorTexture[] Vertices;
        public VertexBuffer VertexBuffer;
        public Effect lightEffect;

        private Texture2D backColor, backSpecular, backNormal;
        private RenderTarget2D shadow, blurredShadow, final;
        private Scheme currentScheme;
        public Landscape()
            : base(Main.windowSize * 0.5f, new Vector2(0, 1), Main.windowSize)
        {
            this.components = new List<IRenderableAsset>();
            this.stainTargets = new RenderTarget2D[2];

            this.SetMaps(new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y),
                new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y),
                new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y));


            backColor = Main.sprites["grass"];
            backSpecular = Main.speculars["grass"];
            backNormal = Main.normals["grass"];
            shadow = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            blurredShadow = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            final = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);

            this.stainTargets[0] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            this.stainTargets[1] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            Main.graphics.GraphicsDevice.SetRenderTarget(this.stainTargets[currentStain]);
            Main.screenManager.SpriteBatch.Begin();
            Main.screenManager.SpriteBatch.Draw(this.backColor, this.boundry, this.overlay);
            Main.screenManager.SpriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTarget(null);



            Vertices = new VertexPositionColorTexture[4];
            Vertices[0] = new VertexPositionColorTexture(new Vector3(-1, 1, 0), Color.White, new Vector2(0, 0));
            Vertices[1] = new VertexPositionColorTexture(new Vector3(1, 1, 0), Color.White, new Vector2(1, 0));
            Vertices[2] = new VertexPositionColorTexture(new Vector3(-1, -1, 0), Color.White, new Vector2(0, 1));
            Vertices[3] = new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 1));
            VertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColorTexture), Vertices.Length, BufferUsage.None);
            VertexBuffer.SetData(Vertices);

            lightEffect = Main.multiTarget;

            lightEffect.CurrentTechnique = lightEffect.Techniques["ShadowMapRender"];
            currentScheme = new Scheme();
            currentScheme.AddLight(new Vector3(Main.screenRect.Width / 2, Main.screenRect.Height / 2, 100), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), 1.0f, Main.screenRect.Height, true);

        }

        public void AddStain(IDrawableAsset stain)
        {
            currentStain++;
            if (currentStain > 1)
                currentStain = 0;
            int background = currentStain -1;
            if (background < 0)
                background = 1;

            Main.graphics.GraphicsDevice.SetRenderTarget(this.stainTargets[currentStain]);
            Main.screenManager.SpriteBatch.Begin();
            Main.screenManager.SpriteBatch.Draw(this.stainTargets[background], this.boundry, this.overlay);
            stain.Draw(Main.screenManager.SpriteBatch);
            Main.screenManager.SpriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTarget(null);


        }


        public void Reset()
        {

            this.stainTargets[0] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            this.stainTargets[1] = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
                        Main.graphics.GraphicsDevice.SetRenderTarget(this.stainTargets[currentStain]);
            Main.screenManager.SpriteBatch.Begin();
            Main.screenManager.SpriteBatch.Draw(this.backColor, this.boundry, this.overlay);
            Main.screenManager.SpriteBatch.End();
            Main.graphics.GraphicsDevice.SetRenderTarget(null);
            this.components.Clear();
        }
        public void AddComponent(IRenderableAsset comp)
        {
            this.components.Add(comp);
        }
        public void RemoveComponent(IRenderableAsset comp)
        {
            this.components.Remove(comp);
        }
        public void Update(GameTime gameTime)
        {
            currentScheme.Update(gameTime);
        }
        public override void Draw(SpriteBatch batch)
        {
            Main.screenManager.SpriteBatch.End();
            Main.screenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            Main.graphics.GraphicsDevice.SetRenderTarget(this.colorMap);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            foreach (IRenderableAsset comp in this.components)
                comp.DrawColorMap(Main.screenManager.SpriteBatch);
            

            Main.graphics.GraphicsDevice.SetRenderTarget(null);


            Main.graphics.GraphicsDevice.SetRenderTarget(this.specularMap);
            Main.graphics.GraphicsDevice.Clear(Color.Black);
            Main.screenManager.SpriteBatch.Draw(this.backSpecular, this.boundry, this.overlay);
            foreach (IRenderableAsset comp in this.components)
                comp.DrawSpecularMap(Main.screenManager.SpriteBatch);

            Main.graphics.GraphicsDevice.SetRenderTarget(null);


            Main.graphics.GraphicsDevice.SetRenderTarget(this.normalMap);
            Main.graphics.GraphicsDevice.Clear(Color.Black);
            Main.screenManager.SpriteBatch.Draw(this.backNormal, this.boundry, this.overlay);
            foreach (IRenderableAsset comp in this.components)
                comp.DrawNormalMap(Main.screenManager.SpriteBatch);

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Main.screenManager.SpriteBatch.End();

            Main.screenManager.SpriteBatch.Begin();
            RenderTarget2D[] targets = new RenderTarget2D[] { this.shadow, this.blurredShadow, this.final };
            string[] techniques = new string[] { "ShadowMapRender", "Blur", "PointLightRender" };

            for (int i = 0; i <3; i++)
            {
                lightEffect.CurrentTechnique = lightEffect.Techniques[techniques[i]];
                Main.graphics.GraphicsDevice.SetRenderTarget(targets[i]);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                if (targets[i] == this.blurredShadow)
                    lightEffect.Parameters["ShadowMap"].SetValue(this.shadow);
                if (targets[i] == this.final)
                    lightEffect.Parameters["ShadowMap"].SetValue(this.blurredShadow);
                lightEffect.Parameters["SpecularMap"].SetValue(this.SpecularMap);
                lightEffect.Parameters["NormalMap"].SetValue(this.NormalMap);
                lightEffect.Parameters["rotation"].SetValue(this.rotation);
                lightEffect.Parameters["EntityMap"].SetValue(this.colorMap);
                lightEffect.Parameters["BackgroundMap"].SetValue(this.stainTargets[currentStain]);
                lightEffect.Parameters["imageSize"].SetValue(this.Size);
                lightEffect.Parameters["spritePosition"].SetValue(new Vector3(this.Position.X, this.Position.Y, 0));
                //lightEffect.Parameters["time"].SetValue(gameTime.TotalGameTime.Milliseconds / 1000.0f);

                foreach (Lights.Light light in currentScheme.lights)
                {
                    if (light.IsEnabled)
                    {
                        // Draw all the light sources
                        lightEffect.Parameters["lightStrength"].SetValue(light.ActualPower);
                        lightEffect.Parameters["lightPosition"].SetValue(light.Position);
                        lightEffect.Parameters["lightColor"].SetValue(light.Color);
                        lightEffect.Parameters["lightDecay"].SetValue(light.LightDecay); // Value between 0.00 and 2.00   
                        lightEffect.Parameters["ambientColor"].SetValue(currentScheme.ambientLight.ToVector4());


                        lightEffect.CurrentTechnique.Passes[0].Apply();

                        Main.graphics.GraphicsDevice.BlendState = BlendBlack;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, 2);
                    }
                }

                // Deactive the rander targets to resolve them
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            }
            Main.screenManager.SpriteBatch.Draw(this.final, this.Position, null, this.overlay, (float)Math.PI - this.rotation, this.Size * 0.5f, this.scale, SpriteEffects.None, 0f);
            

        }
        public static BlendState BlendBlack = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,

            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One
        };

    }

    public class BloodStain : DrawableAsset<Texture2D>
    {
        public float alpha;
        public BloodStain(Vector2 position)
            :base(position,new Vector2(Main.numGenerator.Next(-1, 1), Main.numGenerator.Next(-1, 1)), new Vector2(150,150),Main.bloodStains[Main.numGenerator.Next() % Main.bloodStains.Count])
        {
            this.alpha = 1.0f;
        }
    }


    public enum CoordinateDraw
    {
        Quadrant,
        Position,
        Axes,
        All
    }
    public class CoordinateSystem : DrawableAsset<RenderTarget2D>
    {   

        public CoordinateSystem(CoordinateDraw drawType = CoordinateDraw.All)
            :base(Main.windowSize * 0.5f,new Vector2(0,-1),Main.windowSize,new RenderTarget2D(Main.graphics.GraphicsDevice, (int)Main.windowSize.X, (int)Main.windowSize.Y))
        {

                Main.graphics.GraphicsDevice.SetRenderTarget(this.colorMap);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                Main.screenManager.SpriteBatch.Begin();
                Utilities.DrawLine(Main.screenManager.SpriteBatch, 10, Color.DarkGoldenrod, new Vector2(0, Main.windowSize.Y / 2), new Vector2(Main.windowSize.X, Main.windowSize.Y / 2));
                Utilities.DrawLine(Main.screenManager.SpriteBatch, 10, Color.DarkGoldenrod, new Vector2(Main.windowSize.X / 2, 0), new Vector2(Main.windowSize.X / 2, Main.windowSize.Y));
                if (drawType == CoordinateDraw.All || drawType == CoordinateDraw.Position)
                {
                    Vector2 xSize = TextSettings.CurrentFont.MeasureString("X");
                    Vector2 ySize = TextSettings.CurrentFont.MeasureString("Y") ;
                    Text negX = new Text("-", new Vector2(xSize.X, Main.windowSize.Y / 2.0f + xSize.Y * 0.25f), 2.0f, Color.PaleGreen);
                    Text negY = new Text("-", new Vector2(Main.windowSize.X / 2.0f, Main.windowSize.Y - ySize.Y), 2.0f, Color.PaleGreen);
                    Text posX = new Text("+", new Vector2(Main.windowSize.X - xSize.X, Main.windowSize.Y / 2.0f + xSize.Y * 0.25f), 2.0f, Color.PaleGreen);
                    Text posY = new Text("+", new Vector2(Main.windowSize.X / 2.0f, ySize.Y), 2.0f, Color.PaleGreen);

                    negX.Draw(Main.screenManager.SpriteBatch);
                    negY.Draw(Main.screenManager.SpriteBatch);
                    posX.Draw(Main.screenManager.SpriteBatch);
                    posY.Draw(Main.screenManager.SpriteBatch);

                    Vector2[] offsets = new Vector2[4] { new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1) };
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 pos = Main.windowSize * 0.5f + offsets[i] * Main.windowSize * 0.25f;
                        Main.screenManager.SpriteBatch.DrawString(TextSettings.CurrentFont, Utilities.QuadrantToPolarity(i + 1), pos, Color.Cyan, 0.0f, TextSettings.CurrentFont.MeasureString((i + 1).ToString()) * 0.5f, 2.0f, SpriteEffects.None, 0.0f);
                    }
                }
                if (drawType == CoordinateDraw.All || drawType == CoordinateDraw.Quadrant)
                {
                    Vector2[] offsets = new Vector2[4] { new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1,1) };
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 pos = Main.windowSize * 0.5f + offsets[i] * Main.windowSize * 0.25f;
                        Main.screenManager.SpriteBatch.DrawString(TextSettings.CurrentFont, (i + 1).ToString(), pos, Color.Cyan, 0.0f, TextSettings.CurrentFont.MeasureString((i + 1).ToString()) * 0.5f, 2.0f, SpriteEffects.None, 0.0f);
                    }
                }
                Main.screenManager.SpriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            
        }

    }

}
