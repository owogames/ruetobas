using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public enum Alignment { Left, Centered };

    public class Button
    {
        public Texture2D texture;
        public Rectangle location;
        public Action clickEvent;
        
        public bool enabled = true;
        public bool registerClicks = true;

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
        public int margin;
        public Alignment align;

        public bool canScroll = true;

        public bool enabled = true;

        public TextBox(Texture2D texture, int margin, Alignment align, SpriteFont font, Rectangle location)
        {
            this.texture = texture;
            this.margin = margin;
            this.align = align;
            this.font = font;
            this.location = location;
            lines = new List<string>();
            lineCount = (location.Height - 2 * margin) / font.LineSpacing;
        }
        public TextBox(Texture2D texture, int margin, Alignment align, SpriteFont font, Rectangle location, string line)
        {
            this.texture = texture;
            this.margin = margin;
            this.align = align;
            this.font = font;
            this.location = location;
            lines = new List<string>();
            lines.Add(line);
            lineCount = (location.Height - 2 * margin) / font.LineSpacing;
        }

        public void Append(string line)
        {
            lines.Add(line);
            if (lines.Count - scroll > lineCount)
                scroll = lines.Count - lineCount;
        }

        public void AppendAndWrap(string line)
        {
            string[] words = line.Split(' ');
            string actLine = words[0];
            int lenX = (int)font.MeasureString(actLine).X;
            for (int i = 1; i < words.Count(); i++)
            {
                if (lenX + (int)font.MeasureString(" " + words[i]).X <= location.Width - 2 * margin)
                {
                    actLine += " " + words[i];
                    lenX += (int)font.MeasureString(" " + words[i]).X;
                }
                else
                {
                    Append(actLine);
                    if (i < words.Count())
                    {
                        actLine = words[i];
                        lenX = (int)font.MeasureString(actLine).X;
                    }
                    else
                    {
                        actLine = "";
                        lenX = 0;
                    }
                }
            }
            if (lenX > 0)
               Append(actLine);
        }

        public int scroll = 0;
        public int lineCount;
    }

    public class InputBox
    { 
        public Texture2D texture;
        public int margin;
        public SpriteFont font;
        public Rectangle location;
        public Color color;
        public Color emptyColor;
        public bool active;
        public string text;
        public string emptyText;
        public int charLimit;

        public bool enabled = true;

        public InputBox(Texture2D texture, int margin, SpriteFont font, Rectangle location, Color color, Color emptyColor, string emptyText, int charLimit = 512)
        {
            this.texture = texture;
            this.margin = margin;
            this.font = font;
            this.location = location;
            this.color = color;
            this.emptyColor = emptyColor;
            this.emptyText = emptyText;
            this.charLimit = charLimit;
            text = "";
            active = false;
        }

        public void Clear()
        {
            text = "";
        }

        public void Append(string new_text)
        {
            char enter = '\n';
            new_text = new_text.Replace(enter.ToString(), " ");
            for(int i = 0; text.Length < charLimit && i < new_text.Length; i++)
            {
                text += new_text[i];
            }
        }

        public void Append(char new_text)
        {
            Append(new_text.ToString());
        }

        public string GetText()
        {
            string new_text = "";
            
            for(int i = 1; i <= text.Length; i++)
            {
                if (font.MeasureString(text.Substring(text.Length - i, i)).X <= location.Width - 2 * margin)
                    new_text = text.Substring(text.Length - i, i);
                else
                    break;
            }
            return new_text;
        }
    }

    public class Grid
    {
        public Texture2D boxTexture; //Tekstura tła (ramki)
        public Texture2D defaultFieldTexture; //Tekstura domyślnego pola
        public Texture2D[,] fieldTexture; //Tablica tekstur wszystkich pól (można przypisywać w kodzie)

        public int sizeX, sizeY; //Liczba pól siatki
        public Vector2 fieldSize; //Rozmiar jednego pola w pikselach
        public Rectangle location; //Pozycja + wielkość obszaru na, którym ma się rysować (łącznie z marginesem)
        public int margin; //Wielkość marginesu w pikselach

        public Action<int, int> clickEvent; //Funkcja kliknięcia, powinna przyjmować dwa parametry (x, y) -> numer klikniętego pola (numerowane od 0)
        public Action<SpriteBatch, Rectangle, int, int> drawEvent; //funkcja rysująca

        //Techniczne zmienne - nie przejmowac sie xD
        public Vector2 offset; // o ile przeciągneliśmy grida myszką
        public float zoom; //Przybliżenie
        public RenderTarget2D renderTarget; // super inba XNA elo

        public bool enabled = true;

        public Grid(Game game, Texture2D boxTexture, Texture2D defaultFieldTexture, int sizeX, int sizeY, Vector2 fieldSize, Rectangle location, int margin, Action<int, int> clickEvent)
        {
            this.boxTexture = boxTexture;
            this.defaultFieldTexture = defaultFieldTexture;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fieldSize = fieldSize;
            this.location = location;
            this.margin = margin;
            this.clickEvent = clickEvent;
            drawEvent = DefaultDraw;
            offset = new Vector2(sizeX * fieldSize.X / 2, sizeY * fieldSize.Y / 2);
            zoom = 1.0f;

            renderTarget = new RenderTarget2D(game.GraphicsDevice, (int)((location.Width - 2 * margin) * Game.scale.X), (int)((location.Height - 2 * margin) * Game.scale.Y));

            fieldTexture = new Texture2D[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    fieldTexture[x, y] = defaultFieldTexture;
        }

        public Grid(Game game, Texture2D boxTexture, Texture2D defaultFieldTexture, int sizeX, int sizeY, Vector2 fieldSize, Rectangle location, int margin, Action<int, int> clickEvent, Action<SpriteBatch, Rectangle, int, int> drawEvent)
            :this(game, boxTexture, defaultFieldTexture, sizeX, sizeY, fieldSize, location, margin, clickEvent)
        {
            this.drawEvent = drawEvent;
        }

        public void DefaultDraw(SpriteBatch spriteBatch, Rectangle location, int gridX, int gridY)
        {
            spriteBatch.Draw(fieldTexture[gridX, gridY], location, Color.White);
        }
    }
}
