using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;
using Ecslent2D.Input;

namespace KinectSnake
{
    public static class GestureTests
    {
        public static ArmBias Bias = ArmBias.Left;
        public static bool TouchdownTest(JointCollection currentJoints)
        {
            Vector3 rightHandPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
            Vector3 rightElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            Vector3 leftHandPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
            Vector3 leftElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);

            return rightHandPos.Y < rightElbowPos.Y && leftHandPos.Y < leftElbowPos.Y;
        }


        public static bool EgyptianTest(JointCollection currentJoints)
        {
            Vector3 rightHandPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
            Vector3 rightElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            Vector3 leftHandPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
            Vector3 leftElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);

            return rightHandPos.Y > rightElbowPos.Y && leftHandPos.Y > leftElbowPos.Y;
        }

        public static bool ArmCrossTest(JointCollection currentJoints)
        {
            Vector3 rightHandPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
            Vector3 rightElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            Vector3 leftHandPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
            Vector3 leftElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);

            return rightHandPos.X < leftHandPos.X && rightElbowPos.Y < rightHandPos.Y && leftElbowPos.Y < leftHandPos.Y;
        }


        public static bool RightArmUp(JointCollection currentJoints)
        {
            Vector3 rightHandPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
            Vector3 rightElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            Vector3 rightShoulderPos = new Vector3(2.0f * currentJoints[JointType.ShoulderRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ShoulderRight].Position.Y + 0.5f, currentJoints[JointType.ShoulderRight].Position.Z);
            return (rightHandPos.Y < rightElbowPos.Y) && (rightHandPos.Y  < rightShoulderPos.Y); 
        }

        public static bool LeftArmUp(JointCollection currentJoints)
        {
            Vector3 leftHandPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
            Vector3 leftElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);
            Vector3 leftShoulderPos = new Vector3(2.0f * currentJoints[JointType.ShoulderLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ShoulderLeft].Position.Y + 0.5f, currentJoints[JointType.ShoulderLeft].Position.Z);
            return (leftHandPos.Y < leftElbowPos.Y) && (leftHandPos.Y < leftShoulderPos.Y);
        }

        public static bool RightArmDown(JointCollection currentJoints)
        {
            Vector3 rightHandPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
            Vector3 rightElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            Vector3 rightShoulderPos = new Vector3(2.0f * currentJoints[JointType.ShoulderRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ShoulderRight].Position.Y + 0.5f, currentJoints[JointType.ShoulderRight].Position.Z);
            return (rightHandPos.Y > rightElbowPos.Y) && (rightElbowPos.Y > rightShoulderPos.Y);
        }

        public static bool LeftArmDown(JointCollection currentJoints)
        {
            Vector3 leftHandPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
            Vector3 leftElbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);
            Vector3 leftShoulderPos = new Vector3(2.0f * currentJoints[JointType.ShoulderLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ShoulderLeft].Position.Y + 0.5f, currentJoints[JointType.ShoulderLeft].Position.Z);
            return (leftHandPos.Y > leftElbowPos.Y) && (leftElbowPos.Y > leftShoulderPos.Y);
        }

        public static bool TurnLeft(JointCollection currentJoints)
        {
            Vector3 handPos, elbowPos;
            if (Bias == ArmBias.Left)
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);
            }
            else
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            }
            Vector2 elbowToHand = Vector2.Normalize(new Vector2(handPos.X - elbowPos.X, handPos.Y - elbowPos.Y));

            return handPos.X < elbowPos.X && Utilities.AngleBetween(elbowToHand, new Vector2(-1, 0)) < Math.PI / 8;
        }


        public static bool TurnRight(JointCollection currentJoints)
        {
            Vector3 handPos, elbowPos;
            if (Bias == ArmBias.Left)
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);
            }
            else
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            }
            Vector2 elbowToHand = Vector2.Normalize(new Vector2(handPos.X - elbowPos.X, handPos.Y - elbowPos.Y));

            return handPos.X > elbowPos.X && Utilities.AngleBetween(elbowToHand, new Vector2(1, 0)) < Math.PI / 8;
        }


        public static bool TurnUp(JointCollection currentJoints)
        {
            Vector3 handPos, elbowPos;
            if (Bias == ArmBias.Left)
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);
            }
            else
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            }
            Vector2 elbowToHand = Vector2.Normalize(new Vector2(handPos.X - elbowPos.X, handPos.Y - elbowPos.Y));

            return handPos.Y < elbowPos.Y && Utilities.AngleBetween(elbowToHand, new Vector2(0,-1)) < Math.PI / 8;
        }


        public static bool TurnDown(JointCollection currentJoints)
        {
            Vector3 handPos, elbowPos;
            if (Bias == ArmBias.Left)
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, currentJoints[JointType.HandLeft].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowLeft].Position.Y + 0.5f, currentJoints[JointType.ElbowLeft].Position.Z);
            }
            else
            {
                handPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, currentJoints[JointType.HandRight].Position.Z);
                elbowPos = new Vector3(2.0f * currentJoints[JointType.ElbowRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.ElbowRight].Position.Y + 0.5f, currentJoints[JointType.ElbowRight].Position.Z);
            }
            Vector2 elbowToHand = Vector2.Normalize(new Vector2(handPos.X - elbowPos.X, handPos.Y - elbowPos.Y));

            return handPos.Y > elbowPos.Y && Utilities.AngleBetween(elbowToHand, new Vector2(0,1)) < Math.PI / 8;
        }


        public static bool Pause(JointCollection currentJoints)
        {
            Vector3 handPos;
            if (Bias == ArmBias.Right)
                handPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HandLeft].Position.Z + 0.5f);
            else
                handPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HandRight].Position.Z + 0.5f);

            Vector3 spinePos = new Vector3(2.0f * currentJoints[JointType.Spine].Position.X + 0.5f, -2.0f * currentJoints[JointType.Spine].Position.Y + 0.5f, 2.0f * currentJoints[JointType.Spine].Position.Z + 0.5f);
            Vector3 hipLeftPos = new Vector3(2.0f * currentJoints[JointType.HipLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HipLeft].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HipLeft].Position.Z + 0.5f);
            Vector3 hipRightPos = new Vector3(2.0f * currentJoints[JointType.HipRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HipRight].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HipRight].Position.Z + 0.5f);
            Vector3 headPos = new Vector3(2.0f * currentJoints[JointType.Head].Position.X + 0.5f, -2.0f * currentJoints[JointType.Head].Position.Y + 0.5f, 2.0f * currentJoints[JointType.Head].Position.Z + 0.5f);


            return handPos.X > hipLeftPos.X && handPos.X < hipRightPos.X && handPos.Y > headPos.Y;
        }
        public static bool Resume(JointCollection currentJoints)
        {
            Vector3 handPos;
            if (Bias == ArmBias.Right)
                handPos = new Vector3(2.0f * currentJoints[JointType.HandLeft].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandLeft].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HandLeft].Position.Z + 0.5f);
            else
                handPos = new Vector3(2.0f * currentJoints[JointType.HandRight].Position.X + 0.5f, -2.0f * currentJoints[JointType.HandRight].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HandRight].Position.Z + 0.5f);
            Vector3 headPos = new Vector3(2.0f * currentJoints[JointType.Head].Position.X + 0.5f, -2.0f * currentJoints[JointType.Head].Position.Y + 0.5f, 2.0f * currentJoints[JointType.HandLeft].Position.Z + 0.5f);

            return handPos.Y < headPos.Y;
        }
    }
}
