using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class Button
    {
        public Texture2D texture;
        public Vector2 position;
        public Action clickEvent;

        public Button(Texture2D texture, Vector2 position, Action clickEvent)
        {
            this.texture = texture;
            this.position = position;
            this.clickEvent = clickEvent;
        }
    }
}
