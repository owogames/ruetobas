using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ruetobas
{
    public static class Settings
    {
        public static Game game;
        public static string SaveFileString = "Settings.txt";
        private static char divisionChar = '♂';
        public static Point resolution = new Point(1280, 720);
        public static bool isFullscreen = false;
        public static bool onlyNativeRes = true;
        public static float volume = 1.0f;

        public static void Initialize(Game game)
        {
            Settings.game = game;
            int err = LoadFromFile();
            if(err != 0)
            {
                Console.WriteLine("Failed to load options. Error number = {0}", err);
                err = SaveToFile();
                if (err != 0)
                    Console.WriteLine("Failed to save options. Error number = {0}", err);
            }
        }

        public static int SaveToFile()
        {
            FileStream save = File.OpenWrite(SaveFileString);
            string output = resolution.X.ToString() + divisionChar
                + resolution.Y.ToString() + divisionChar
                + isFullscreen.ToString() + divisionChar
                + onlyNativeRes.ToString() + divisionChar
                + volume.ToString();
            byte[] b = Encoding.UTF8.GetBytes(output);
            save.Write(b, 0, b.Length);
            save.Close();
            return 0;
        }

        public static int LoadFromFile()
        {
            Point new_res = new Point();
            bool new_fullscreen;
            bool new_nativeRes;
            float new_volume;
            if (File.Exists(SaveFileString))
            {
                try
                {
                    string input = File.ReadAllText(SaveFileString);
                    string[] elements = input.Split(divisionChar);
                    int x = 0;
                    new_res.X = int.Parse(elements[x++]);
                    new_res.Y = int.Parse(elements[x++]);
                    new_fullscreen = bool.Parse(elements[x++]);
                    new_nativeRes = bool.Parse(elements[x++]);
                    new_volume = float.Parse(elements[x++]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading file!");
                    Console.WriteLine(e.Message);
                    return 2;
                }
                
                game.ChangeResolution(new_res);
                if (isFullscreen != new_fullscreen)
                    Logic.ChangeFullscreen();
                if (onlyNativeRes != new_nativeRes)
                    Logic.ChangeNativeResMode();
                volume = new_volume;
                
                return 0;
            }
            else
                return 1;
        }
        

    }
}
