using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public static class Logic
    {
        public static Dictionary<string, Button> buttons;
        public static Dictionary<string, TextBox> textBoxes;
        public static Dictionary<string, InputBox> inputBoxes;

        public static Texture2D chatTexture;
        public static SpriteFont font;

        public const int port = 2137;

        public static string IP = "81.190.71.186";
        public static string username = "No Elo";

        public static void Init(Game game)
        {
            buttons = new Dictionary<string, Button>();
            textBoxes = new Dictionary<string, TextBox>();
            inputBoxes = new Dictionary<string, InputBox>();

            chatTexture = game.Content.Load<Texture2D>("tekstura");
            font = game.Content.Load<SpriteFont>("font");

            //buttons["0"] = new Button(buttonTexture, new Rectangle(0, 0, 200, 100), () => NewButton(50, 50));

            textBoxes["CHAT"] = new TextBox(chatTexture, font, new Rectangle(440, 100, 400, 400));

            //inputBoxes["TEST"] = new InputBox(buttonTexture, font, new Rectangle(500, 0, 200, 30), Color.White, Color.Red, "JESTEM PUSTY :<");
            //inputBoxes["TEST"].text = "NIE JESTEM PUSTY :3";

            game.TCPConnect(IP, port);
        }

        public static int timer = 0;
        public static void Update(Game game)
        {
            if (game.keyboardState.IsKeyDown(Keys.Space) && game.keyboardBeforeState.IsKeyUp(Keys.Space))
                game.TCPSend("CHAT " + username + ": OwO");
            if (game.keyboardState.IsKeyDown(Keys.Escape) && game.keyboardBeforeState.IsKeyUp(Keys.Escape))
            {
                if (game.tcpThread != null)
                {
                    game.TCPSend("QUIT");
                    game.tcpThread.Abort();
                }
                game.Exit();
            }
        }

        public static void TCPRecieved(string message, Game game)
        {
            Console.WriteLine(message);
            if (message.Substring(0, 4) == "CHAT")
                textBoxes["CHAT"].Append(message.Substring(5).Trim());
            if (message.Substring(0, 4) == "JOIN")
                textBoxes["CHAT"].Append(message.Substring(5).Trim() + " has joined the chat.");
            if (message.Substring(0, 3) == "BYE")
                textBoxes["CHAT"].Append(message.Substring(4).Trim() + " has left the chat.");
            if (message.Substring(0, 5) == "ERROR")
            {
                textBoxes["CHAT"].Append(message.Substring(6).Trim());
                game.tcpThread.Abort();
            }
        }
    }
}
