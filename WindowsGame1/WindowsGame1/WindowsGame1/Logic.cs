using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static Texture2D buttonTexture;
        public static SpriteFont font;

        public static void Init(Game game)
        {
            buttons = new Dictionary<string, Button>();
            textBoxes = new Dictionary<string, TextBox>();
            inputBoxes = new Dictionary<string, InputBox>();

            buttonTexture = game.Content.Load<Texture2D>("zoltyskurwiel");
            font = game.Content.Load<SpriteFont>("font");

            buttons["0"] = new Button(buttonTexture, new Rectangle(0, 0, 200, 100), () => NewButton(50, 50));

            textBoxes["TEST"] = new TextBox(buttonTexture, font, new Rectangle(100, 100, 300, 300));

            inputBoxes["TEST"] = new InputBox(buttonTexture, font, new Rectangle(500, 0, 200, 30), Color.White, Color.Red, "JESTEM PUSTY :<");
            inputBoxes["TEST"].text = "NIE JESTEM PUSTY :3";
        }

        public static int timer = 0;
        public static void Update(Game game)
        {
            
        }

        public static void TCPRecieved(string message, Game game)
        {
            Console.WriteLine(message);
        }

        public static void NewButton(int x, int y)
        {
            buttons[x.ToString()] = new Button(buttonTexture, new Rectangle(x, y, 200, 100), () => NewButton(x + 50, y + 50));
        }
    }
}
