using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public static class Logic
    {
        public static List<Button> buttons;

        public static void Init()
        {
            buttons = new List<Button>();
            buttons.Add(new Button(Game.buttonTexture, new Vector2(0, 0), null));
            buttons.Add(new Button(Game.buttonTexture, new Vector2(100, 100), null));
        }
    }
}
