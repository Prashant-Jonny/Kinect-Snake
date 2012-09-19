using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Speech.Recognition;

namespace Ecslent2D.Input.Kinect
{
    public delegate bool GestureTest(JointCollection currentJoints);

    public delegate void GestureHandler(PlayerIndex player);

    public class KinectInputState
    {

        private KinectSensor kinectSensor;
        private bool trackingPlayer;
        private int playerSkeletonIndex = -1;
        private Thread listener;
        private AudioCell cell;
        private AudioListener kinectSpeechListener;
        private RecognitionResult voiceCommand = null;
        private Texture2D kinectRGB, kinectDepth;
        private JointCollection joints;
        private bool startElevationSet = false;
        private Dictionary<GestureTest, GestureHandler> gestures;


        private bool colorEnabled = false, depthEnabled = false, speechEnabled = false, skeletonEnabled = false;

        public KinectInputState()
        {
            DiscoverSensor();
            gestures = new Dictionary<GestureTest, GestureHandler>();
        }


        public bool IsEnabled()
        {
            return kinectSensor != null;
        }


        protected void DiscoverSensor()
        {
            kinectSensor = null;
            trackingPlayer = false;
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    kinectSensor = sensor;
                    break;
                }
            }
            if (kinectSensor != null)
            {
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(StatusChanged);
                kinectSensor.Start();
                kinectSensor.ElevationAngle = 8;
            }
        }

        public void Update(GameTime gTime)
        {
            if (kinectSensor != null && speechEnabled)
            {
                RecognitionResult newCommand = cell.GetRecognitionResult();
                if (newCommand != null && newCommand.likelihood > 0.65)
                {
                    voiceCommand = newCommand;
                }
            }
        }



        protected void StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (kinectSensor == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected || e.Status == KinectStatus.NotPowered)
                {
                    Dispose();
                    DiscoverSensor();
                }
            }
        }

        public void Dispose()
        {
            if (IsEnabled())
            {
                if (speechEnabled)
                {
                    listener.Abort();
                    listener.Join();
                }
                kinectSensor.Stop();
                kinectSensor.Dispose();
                kinectSensor = null;
            }
        }


        #region Color Video Capture

        public void EnableColorCapture(GraphicsDevice device, ColorImageFormat format = ColorImageFormat.RgbResolution640x480Fps30)
        {
            if (kinectSensor != null)
            {
                // Skeleton Stream
                kinectSensor.ColorStream.Enable(format);
                kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(ColorFrameReady);

                if (format == ColorImageFormat.RgbResolution1280x960Fps12)
                    kinectRGB = new Texture2D(device, 1280, 960);
                else
                    kinectRGB = new Texture2D(device, 640, 480);
                colorEnabled = true;
            }
        }

        public void DisableColorCapture()
        {
            if (colorEnabled)
            {
                kinectRGB = null;
                kinectSensor.ColorStream.Disable();
                kinectSensor.ColorFrameReady -= new EventHandler<ColorImageFrameReadyEventArgs>(ColorFrameReady);
                colorEnabled = false;
            }
        }
        public bool IsColorCaptureEnabled()
        {
            return colorEnabled;
        }

        public Texture2D GetLastColorFrame()
        {
            return kinectRGB;
        }
        protected void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {

            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame != null)
                {

                    byte[] pixelsFromFrame = new byte[colorImageFrame.PixelDataLength];

                    colorImageFrame.CopyPixelDataTo(pixelsFromFrame);

                    Color[] colors = new Color[colorImageFrame.Height * colorImageFrame.Width];

                    // Go through each pixel and set the bytes correctly
                    // Remember, each pixel got a Rad, Green and Blue
                    int index = 0;
                    for (int y = 0; y < colorImageFrame.Height; y++)
                    {
                        for (int x = 0; x < colorImageFrame.Width; x++, index += 4)
                        {
                            colors[y * colorImageFrame.Width + x] = new Color(pixelsFromFrame[index + 2], pixelsFromFrame[index + 1], pixelsFromFrame[index]);
                        }
                    }

                    // Set pixeldata from the ColorImageFrame to a Texture2D
                    kinectRGB.SetData(colors);
                }
            }
        }
        #endregion

        # region Skeletal Tracking
        public void EnableSkeletalTracking()
        {
            if (kinectSensor != null)
            {
                // Skeleton Stream
                kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrameReady);
                skeletonEnabled = true;
            }
        }


        public void DisableSkeletalTracking()
        {
            if (skeletonEnabled)
            {
                kinectSensor.SkeletonStream.Disable();
                kinectSensor.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrameReady);
                skeletonEnabled = false;
            }
        }

        public bool IsSkeletalTrackingEnabled()
        {
            return skeletonEnabled;
        }
        public bool IsTrackingPlayer()
        {
            return trackingPlayer && skeletonEnabled;
        }

        public void EnableGestureTest(GestureTest test, GestureHandler callback)
        {
            gestures[test] = callback;
        }

        public void DisableGestureTest(GestureTest test)
        {
            if (gestures.ContainsKey(test))
                gestures.Remove(test);
        }

        public Vector3 GetJointPosition(JointType t)
        {
            if (joints != null)
                return new Vector3(2.0f * joints[t].Position.X + 0.5f, -2.0f * joints[t].Position.Y + 0.5f, joints[t].Position.Z);

            else
                return Vector3.Zero;

        }

        protected void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton;
                    if (playerSkeletonIndex == -1)
                    {
                        playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                        if (playerSkeleton != null)
                            playerSkeletonIndex = playerSkeleton.TrackingId;
                    }
                    else
                        playerSkeleton = (from s in skeletonData where s.TrackingState == SkeletonTrackingState.Tracked && s.TrackingId == playerSkeletonIndex select s).FirstOrDefault();
                    if (playerSkeleton != null)
                    {
                        joints = playerSkeleton.Joints;
                        List<GestureHandler> toPerform = new List<GestureHandler>();
                        foreach (KeyValuePair<GestureTest, GestureHandler> gesture in gestures)
                            if (gesture.Key(joints))
                                toPerform.Add(gesture.Value);

                        foreach (GestureHandler handler in toPerform)
                            handler(PlayerIndex.One);
                        trackingPlayer = true;

                    }
                    else
                    {
                        trackingPlayer = false;
                        playerSkeletonIndex = -1;
                    }
                }
            }
        }
        #endregion

        #region Voice Recognition


        public void EnableVoiceRecognition()
        {

            if (kinectSensor != null)
            {
                cell = new AudioCell();
                kinectSpeechListener = new AudioListener(cell, kinectSensor.AudioSource);
                listener = new Thread(new ThreadStart(kinectSpeechListener.Listen));
                listener.Start();
                speechEnabled = true;
            }
        }

        public void DisableVoiceRecognition()
        {
            if (speechEnabled)
            {
                listener.Abort();
                listener.Join();
                cell = null;
                kinectSpeechListener = null;
                listener = null;
                speechEnabled = false;
            }
        }

        public bool IsVoiceRecognitionEnabled()
        {
            return speechEnabled;
        }
        public void SetNewSpeechVocabulary(Choices newChoices)
        {
            cell.SetNewVocabulary(newChoices);
        }


        public RecognitionResult GetNewestCommand()
        {
            return voiceCommand;
        }

        #endregion

        #region Depth Video Capture

        public void EnableDepthCapture(GraphicsDevice device, DepthImageFormat format = DepthImageFormat.Resolution640x480Fps30)
        {
            if (kinectSensor != null)
            {
                // Skeleton Stream
                kinectSensor.DepthStream.Enable(format);
                kinectSensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(DepthFrameReady);
                if (format == DepthImageFormat.Resolution640x480Fps30)
                    kinectDepth = new Texture2D(device, 640, 480);
                else if (format == DepthImageFormat.Resolution320x240Fps30)
                    kinectDepth = new Texture2D(device, 320, 240);
                else if (format == DepthImageFormat.Resolution80x60Fps30)
                    kinectDepth = new Texture2D(device, 80, 60);
                depthEnabled = true;
            }
        }
        public void DisableDepthCapture()
        {
            if (depthEnabled)
            {
                kinectDepth = null;
                kinectSensor.DepthStream.Disable();
                kinectSensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(DepthFrameReady);
                depthEnabled = false;
            }
        }


        public bool IsDepthCaptureEnabled()
        {
            return depthEnabled;
        }
        protected void DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {

                    short[] pixelsFromFrame = new short[depthImageFrame.PixelDataLength];

                    depthImageFrame.CopyPixelDataTo(pixelsFromFrame);

                    Color[] colors = new Color[depthImageFrame.Height * depthImageFrame.Width];

                    // Go through each pixel and set the bytes correctly
                    // Remember, each pixel got a Rad, Green and Blue
                    int index = 0;
                    for (int y = 0; y < depthImageFrame.Height; y++)
                    {
                        for (int x = 0; x < depthImageFrame.Width; x++, index += 4)
                        {
                            colors[y * depthImageFrame.Width + x] = new Color(pixelsFromFrame[index + 2], pixelsFromFrame[index + 1], pixelsFromFrame[index]);
                        }
                    }

                    // Set pixeldata from the ColorImageFrame to a Texture2D
                    kinectDepth.SetData(colors);
                }
            }
        }
        #endregion



    }

}
