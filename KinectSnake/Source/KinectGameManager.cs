
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Ecslent2D;
using Ecslent2D.ScreenManagement.Screens;

namespace KinectSnake
{
    public enum GameType
    {
        Classic,
        Quadrant
    }

    public enum Difficulty
    {
        VeryEasy,
        Easy,
        Normal,
        Hard,
        Hardest
    }

    public struct SaveGameData
    {
        public string PlayerName;
        public Difficulty ClassicProgress;
        public Difficulty QuadrantProgress;
    }

    public class GameManager
    {
        private Dictionary<GameType, Dictionary<Difficulty, bool>> unlocked;

        public void LoadSettings(string user)
        {
            unlocked = new Dictionary<GameType, Dictionary<Difficulty, bool>>();
            string path = "Saves/" + user + ".usr";
            if (File.Exists(path))
            {
                /*StorageDevice device;// = new StorageDevice();
                // Open a storage container.
                IAsyncResult result =
                    device.BeginOpenContainer("StorageDemo", null, null);

                // Wait for the WaitHandle to become signaled.
                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                // Close the wait handle.
                result.AsyncWaitHandle.Close();*/
            }
            else
            {
                foreach (GameType type in Enum.GetValues(typeof(GameType)))
                {
                    unlocked[type] = new Dictionary<Difficulty, bool>();
                    bool unlock = true;
                    foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
                    {
                        unlocked[type][diff] = unlock;
                        if (type != GameType.Classic)
                            unlock = false;
                    }

                }
            }
        }
        public Screen LaunchGameScreen(GameType type, Difficulty diff) 
        {

            if (type == GameType.Classic)
                return new Screens.ClassicGameScreen(diff);

            else
                return new Screens.QuadrantGameScreen( Screens.QuadrantExcercises.NumbersOne);
        }
        public void Unlock(GameType type, Difficulty diff)
        {
            unlocked[type][diff] = true;
        }
        public void Lock(GameType type, Difficulty diff)
        {
            unlocked[type][diff] = false;
        }
        public bool IsUnlocked(GameType type, Difficulty diff)
        {
            return unlocked[type][diff];
        }
    }
}
