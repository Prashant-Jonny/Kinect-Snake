using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KinectSnake
{
    public static class Utilities
    {
        public static void DrawLine(SpriteBatch batch, float width, Color color, Vector2 point1, Vector2 point2)
        {
            Texture2D dot = new Texture2D(Main.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            dot.SetData(new[] { Color.White });
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(dot, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public static string QuadrantToPolarity(int quadrant)
        {
            if (quadrant == 1)
                return "(+ , +)";
            else if (quadrant == 2)
                return "(- , +)";
            else if (quadrant == 3)
                return "(- , -)";
            else if (quadrant == 4)
                return "(+ , -)";
            else
                return null;
        }
        public static int PositionToQuadrant(Vector2 pos)
        {
            Vector2 midPoint = Main.windowSize * 0.5f;
            if (pos.X >= midPoint.X && pos.Y <= midPoint.Y)
                return 1;
            else if (pos.X <= midPoint.X && pos.Y <= midPoint.Y)
                return 2;
            else if (pos.X <= midPoint.X && pos.Y >= midPoint.Y)
                return 3;
            else 
                return 4;

        }
        public static Vector2 PositionToCoordinate(Vector2 pos)
        {
            Vector2 midPoint = Main.windowSize * 0.5f;
            return pos - midPoint;

        }
        public static Rectangle QuadrantToRect(int quadrant)
        {
            Vector2 midPoint = Main.windowSize * 0.5f;
            if (quadrant == 1)
                return new Rectangle((int)midPoint.X, 0, (int)(Main.screenRect.Width * 0.5f), (int)(Main.screenRect.Height * 0.5f));
            else if (quadrant == 2)
                return new Rectangle(0, 0, (int)(Main.screenRect.Width * 0.5f), (int)(Main.screenRect.Height * 0.5f));
            else if (quadrant == 3)
                return new Rectangle(0, (int)midPoint.Y, (int)(Main.screenRect.Width * 0.5f), (int)(Main.screenRect.Height * 0.5f));
            else
                return new Rectangle((int)midPoint.X, (int)midPoint.Y, (int)(Main.screenRect.Width * 0.5f), (int)(Main.screenRect.Height * 0.5f));

        }
        public static float Gauss(float mean, float stdDev)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return (float)(mean + stdDev * randStdNormal); //random normal(mean,stdDev^2)
        }
        public static Vector2 ScreenWrap(Vector2 pos)
        {
            if (pos.X < 0)
                pos.X += Main.windowSize.X;
            else if (pos.Y < 0)
                pos.Y += Main.windowSize.Y;
            else if (pos.X > Main.windowSize.X)
                pos.X -= Main.windowSize.X;
            else if (pos.Y > Main.windowSize.Y)
                pos.Y -= Main.windowSize.Y;
            return pos;
        }
        public static Rectangle RotateRect(Rectangle rect,float rotation)
        {
            Vector2 smallest = new Vector2(1000000, 100000);
            Vector2 highest = new Vector2(-1000000, -100000);
            int iterations = 0;
            for (float i = rect.Left; i <= rect.Right; i += rect.Width)
                for (float j = rect.Top; j <= rect.Bottom; j += rect.Height)
                {
                    float x = rect.Center.X + i * (float)Math.Cos(rotation) + j * (float)Math.Sin(rotation);
                    float y = rect.Center.Y - i * (float)Math.Sin(rotation) + j * (float)Math.Cos(rotation);
                    if (x < smallest.X)
                        smallest.X = x;
                    if (x > highest.X)
                        highest.X = x;
                    if (y < smallest.Y)
                        smallest.Y = y;
                    if (y > highest.Y)
                        highest.Y = y;
                    iterations++;
                }

            int w = (int)(highest.X - smallest.X);
            int h= (int)(highest.Y - smallest.Y);
            return new Rectangle((int)rect.Center.X - (int)w / 2, (int)rect.Center.Y - (int)h / 2, (int)w, (int)h);
        }
        public static double AngleBetween(Vector2 lineOne, Vector2 lineTwo)
        {
            double angle = Math.Acos(Vector2.Dot(Vector2.Normalize(lineOne),Vector2.Normalize(lineTwo)));
            if (angle > Math.PI)
                angle = 2 * Math.PI - angle;
            else if (angle < 0)
                angle += 2 * Math.PI;
            return angle;
        }
    }
}
