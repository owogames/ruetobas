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

        public TextBox(Texture2D texture, int margin, Alignment align, SpriteFont font, Rectangle location)
        {
            this.texture = texture;
            this.margin = margin;
            this.align = align;
            this.font = font;
            this.location = location;
            lines = new List<string>();
            lineCount = location.Height / font.LineSpacing;
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

        public InputBox(Texture2D texture, int margin, SpriteFont font, Rectangle location, Color color, Color emptyColor, string emptyText)
        {
            this.texture = texture;
            this.margin = margin;
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
        public RenderTarget2D renderTarget; // super inba XNA elo

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
            offset = new Vector2(0, 0);

            renderTarget = new RenderTarget2D(game.GraphicsDevice, location.Width - 2 * margin, location.Height - 2 * margin);

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
