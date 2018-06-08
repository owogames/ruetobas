using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public class Button
    {
        public Texture2D texture;
        public Rectangle location;
        public Action clickEvent;

        public Button(Texture2D texture, Rectangle location, Action clickEvent)
        {
            this.texture = texture;
            this.location = location;
            this.clickEvent = clickEvent;
        }
    }

    public class TextBox
    {
        public List<string> lines;
        public Texture2D texture;
        public SpriteFont font;
        public Rectangle location;
        public TextBox(Texture2D texture, SpriteFont font, Rectangle location)
        {
            this.texture = texture;
            this.font = font;
            this.location = location;
            lines = new List<string>();
            lineCount = location.Height / font.LineSpacing;
        }
        public TextBox(Texture2D texture, SpriteFont font, Rectangle location, string line)
        {
            this.texture = texture;
            this.font = font;
            this.location = location;
            lines = new List<string>();
            lines.Add(line);
            lineCount = location.Height / font.LineSpacing;
        }

        public void Append(string line)
        {
            lines.Add(line);
            if (lines.Count - scroll > lineCount)
                scroll = lines.Count - lineCount;
        }

        public int scroll = 0;
        public int lineCount;
    }

    public class InputBox
    { 
        public Texture2D texture;
        public SpriteFont font;
        public Rectangle location;
        public Color color;
        public Color emptyColor;
        public bool active;
        public string text;
        public string emptyText;

        public InputBox(Texture2D texture, SpriteFont font, Rectangle location, Color color, Color emptyColor, string emptyText)
        {
            this.texture = texture;
            this.font = font;
            this.location = location;
            this.color = color;
            this.emptyColor = emptyColor;
            this.emptyText = emptyText;
            text = "";
            active = false;
        }

        public void Clear()
        {
            text = "";
        }
    }
}
