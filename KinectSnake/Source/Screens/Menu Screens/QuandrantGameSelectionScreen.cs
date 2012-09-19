#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ecslent2D.Input;
using Ecslent2D.Graphics;
using Ecslent2D.ScreenManagement.Screens;
using Ecslent2D.Storage;
#endregion

namespace KinectSnake.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    /// 

    public enum QuadrantExcercises
    {
        NumbersOne,
        NumbersTwo,
        NumbersThree,
        PolarityOne,
        PolarityTwo,
        PolarityThree
    }

    [Serializable()]
    public class QuadrantSaveInfo
    {
        public bool IsUnlocked {get;set;}
        public int TopScore{get;set;}

        public QuadrantSaveInfo(bool unlocked, int topScore = 0)
        {
            IsUnlocked = unlocked;
            TopScore = topScore;
        }
    }

    class QuadrantGameSelectionScreen : MenuScreen
    {
        #region Initialization
        SerializableDictionary<QuadrantExcercises, QuadrantSaveInfo> quadrantProfile;
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public QuadrantGameSelectionScreen()
            : base("Select Quadrant Excercise", Color.Coral)
        {
            quadrantProfile = new SerializableDictionary<QuadrantExcercises, QuadrantSaveInfo>();
            quadrantProfile[QuadrantExcercises.NumbersOne] = new QuadrantSaveInfo(false, 0);
            quadrantProfile[QuadrantExcercises.NumbersTwo] = new QuadrantSaveInfo(false, 0);
            quadrantProfile[QuadrantExcercises.NumbersThree] = new QuadrantSaveInfo(false, 0);
            quadrantProfile[QuadrantExcercises.PolarityOne] = new QuadrantSaveInfo(false, 0);
            quadrantProfile[QuadrantExcercises.PolarityTwo] = new QuadrantSaveInfo(false, 0);
            quadrantProfile[QuadrantExcercises.PolarityThree] = new QuadrantSaveInfo(false, 0);

        }

        public override void Activate(InputState input)
        {
            base.Activate(input);
            AddMenuEntry("Quadrant Numbers I", ExcerciseOneSelected, quadrantProfile[QuadrantExcercises.NumbersOne].IsUnlocked);
            AddMenuEntry("Quadrant Numbers II", ExcerciseTwoSelected, quadrantProfile[QuadrantExcercises.NumbersTwo].IsUnlocked);
            AddMenuEntry("Quadrant Numbers III", ExcerciseThreeSelected, quadrantProfile[QuadrantExcercises.NumbersThree].IsUnlocked);
            AddMenuEntry("Positive/Negative I ", ExcerciseFourSelected, quadrantProfile[QuadrantExcercises.PolarityOne].IsUnlocked);
            AddMenuEntry("Positive/Negative II ", ExcerciseFiveSelected, quadrantProfile[QuadrantExcercises.PolarityTwo].IsUnlocked);
            AddMenuEntry("Positive/Negative III ", ExcerciseSixSelected, quadrantProfile[QuadrantExcercises.PolarityThree].IsUnlocked);
            SetBackground(new DrawableAsset<Texture2D>(Main.windowSize * 0.5f, new Vector2(0, -1), Main.windowSize, Main.sprites["menuBack"]));
            SetHandCursor(Main.sprites["handCursor"]);
            SetSelectionSprite(Main.animatedSprites["load"]);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void ExcerciseOneSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameScreen(QuadrantExcercises.NumbersOne));
        }
        void ExcerciseTwoSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameScreen(QuadrantExcercises.NumbersTwo));
        }
        void ExcerciseThreeSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameScreen(QuadrantExcercises.NumbersThree));
        }
        void ExcerciseFourSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameScreen(QuadrantExcercises.PolarityOne));
        }
        void ExcerciseFiveSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameScreen(QuadrantExcercises.PolarityTwo));
        }


        void ExcerciseSixSelected(PlayerIndexEventArgs e)
        {
            ScreenManager.TransitionTo(new QuadrantGameScreen(QuadrantExcercises.PolarityThree));
        }

        #endregion
        public override void Draw()
        {
            base.Draw();

            // Darken down any other screens that were drawn beneath the popup.
            if (ScreenState == ScreenState.TransitionOff)
                ScreenManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
        }
    }
}
