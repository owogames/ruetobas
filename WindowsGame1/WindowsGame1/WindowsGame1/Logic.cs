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
        public static List<Button> buttons;

        public static Texture2D buttonTexture;

        public static void Init(Game game)
        {
            buttons = new List<Button>();
            buttonTexture = game.Content.Load<Texture2D>("zoltyskurwiel");
            buttons.Add(new Button(buttonTexture, new Rectangle(0, 0, 200, 100), () => NewButton(50, 50)));
        }

        public static void NewButton(int x, int y)
        {
            buttons.Add(new Button(buttonTexture, new Rectangle(x, y, 200, 100), () => NewButton(x + 50, y + 50)));
        }
    }
}
