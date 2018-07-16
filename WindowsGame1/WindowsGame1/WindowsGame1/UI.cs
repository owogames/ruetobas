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
        public bool useScrollToScroll = false;

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

    public class Timer
    {
        public float countDuration;
        public float currentTime;
        public Action countdownEvent;

        public Timer(float countDuration, Action countdownEvent)
        {
            this.countDuration = countDuration;
            this.countdownEvent = countdownEvent;
            currentTime = 0f;
        }
    }

    public class UI
    {
        public static void DisableGroup(string prefix)
        {
            foreach (KeyValuePair<string, Button> b in Logic.buttons)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = false;
            }
            foreach (KeyValuePair<string, TextBox> b in Logic.textBoxes)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = false;
            }
            foreach (KeyValuePair<string, InputBox> b in Logic.inputBoxes)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = false;
            }
            foreach (KeyValuePair<string, Grid> b in Logic.grids)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = false;
            }
        }

        public static void EnableGroup(string prefix)
        {
            foreach (KeyValuePair<string, Button> b in Logic.buttons)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = true;
            }
            foreach (KeyValuePair<string, TextBox> b in Logic.textBoxes)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = true;
            }
            foreach (KeyValuePair<string, InputBox> b in Logic.inputBoxes)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = true;
            }
            foreach (KeyValuePair<string, Grid> b in Logic.grids)
            {
                if (b.Key.StartsWith(prefix))
                    b.Value.enabled = true;
            }
        }

        public static void InitUI()
        {
            //Menu
            Logic.buttons[Logic.menuNamespace + "0Background"] = new Button(Logic.menuBackground, new Rectangle(0, 0, 1920, 1080), null);
            Logic.buttons[Logic.menuNamespace + "Join"] = new Button(Logic.joinTexture, new Rectangle(120, 620, 360, 80), () => { EnableGroup(Logic.menuNamespace + "Z"); });
            Logic.buttons[Logic.menuNamespace + "Options"] = new Button(Logic.optionsTexture, new Rectangle(120, 700, 300, 80), () => Logic.DisplayOptions(true));
            Logic.buttons[Logic.menuNamespace + "Quit"] = new Button(Logic.quitTexture, new Rectangle(120, 780, 200, 80), Logic.game.Exit);
            Logic.buttons[Logic.menuNamespace + "ZBackground"] = new Button(Logic.connectWindow, new Rectangle(0, 0, 1920, 1080), null);
            Logic.inputBoxes[Logic.menuNamespace + "Zip"] = new InputBox(Logic.inputboxTexture, 5, Logic.guifont, new Rectangle(770, 440, 380, 60), Color.White, Color.LightGray, "Enter server IP");
            Logic.inputBoxes[Logic.menuNamespace + "Znick"] = new InputBox(Logic.inputboxTexture, 5, Logic.guifont, new Rectangle(770, 560, 380, 60), Color.White, Color.LightGray, "Enter username", 32);
            Logic.buttons[Logic.menuNamespace + "Zconnect"] = new Button(Logic.connectTexture, new Rectangle(840, 680, 260, 60), Logic.LoadGameScreen);
            Logic.buttons[Logic.menuNamespace + "Zcancel"] = new Button(Logic.cancelTexture, new Rectangle(1520, 820, 200, 60), () => { DisableGroup(Logic.menuNamespace + "Z"); });
            DisableGroup(Logic.menuNamespace + "Z");

            //Options
            Logic.buttons[Logic.optionsNamespace + "0Background"] = new Button(Logic.optionsWindow, new Rectangle(0, 0, 1920, 1080), () => { Logic.grids[Logic.optionsNamespace + "ZResolutions"].enabled = false; });
            Logic.buttons[Logic.optionsNamespace + "FullscreenText"] = new Button(Logic.fullscreenTexture, new Rectangle(200, 200, 300, 60), null);
            Logic.buttons[Logic.optionsNamespace + "ResolutionText"] = new Button(Logic.resolutionTexture, new Rectangle(200, 280, 300, 60), null);
            Logic.buttons[Logic.optionsNamespace + "VolumeText"] = new Button(Logic.volumeTexture, new Rectangle(200, 360, 200, 60), null);
            Logic.buttons[Logic.optionsNamespace + "Done"] = new Button(Logic.doneTexture, new Rectangle(200, 820, 140, 60), () => Logic.DisplayOptions(false));
            Logic.buttons[Logic.optionsNamespace + "Exit"] = new Button(Logic.exitTexture, new Rectangle(1620, 820, 100, 60), Logic.game.Exit);
            Logic.textBoxes[Logic.optionsNamespace + "ResolutionSelected"] = new TextBox(Logic.inputboxTexture, 5, Alignment.Centered, Logic.guifont, new Rectangle(560, 280, 380, 60), "1280 x 720    16:9");
            Logic.textBoxes[Logic.optionsNamespace + "ResolutionSelected"].canScroll = false;
            Logic.buttons[Logic.optionsNamespace + "ResolutionButton"] = new Button(Logic.dropdownTexture, new Rectangle(960, 280, 60, 60), () => { Logic.grids[Logic.optionsNamespace + "ZResolutions"].enabled = true; });
            //Logic.textBoxes[Logic.optionsNamespace + "NativeResText"] = new TextBox(Logic.chatInputTexture, 8, Alignment.Centered, Logic.font, new Rectangle(10, 120, 200, 50), "Only native resolution");
            Logic.buttons[Logic.optionsNamespace + "Fullscreen"] = new Button(Logic.tickedTexture, new Rectangle(560, 200, 60, 60), Logic.ChangeFullscreen);
            //Logic.buttons[Logic.optionsNamespace + "nativeRes"] = new Button(Logic.onlyNativeRes ? Logic.tickedTexture : Logic.unTickedTexture, new Rectangle(220, 120, 50, 50), Logic.ChangeNativeResMode);
            if (!Settings.isFullscreen)
                Logic.buttons[Logic.optionsNamespace + "Fullscreen"].texture = Logic.unTickedTexture;
            Logic.inputBoxes[Logic.optionsNamespace + "Volume"] = new InputBox(Logic.inputboxSmallTexture, 10, Logic.guifont, new Rectangle(560, 360, 140, 60), Color.Orange, Color.White, "Enter", 3);
            //Logic.buttons[Logic.optionsNamespace + "TestSound"] = new Button(Logic.errorButton, new Rectangle(220, 230, 50, 50), () => Logic.PlaySound(Logic.bubbles, Settings.volume));
            //Logic.buttons[Logic.optionsNamespace + "done"] = new Button(Logic.readyTexture, new Rectangle(10, 345, 140, 80), () => Logic.DisplayOptions(false));
            //Logic.buttons[Logic.optionsNamespace + "Quit"] = new Button(Logic.errorButton, new Rectangle(1700, 940, 140, 80), Logic.game.Exit);
            Logic.grids[Logic.optionsNamespace + "ZResolutions"] = new Grid(Logic.game, Logic.dropdownMenuTexture, null, 1, Logic.displayModes.Length, new Vector2(380, 60), new Rectangle(560, 360, 380, 300), 5, Logic.ResolutionsClick, Logic.ResolutionsDraw);
            Logic.grids[Logic.optionsNamespace + "ZResolutions"].useScrollToScroll = true;
            Logic.grids[Logic.optionsNamespace + "ZResolutions"].offset.Y = 145;
            DisableGroup(Logic.optionsNamespace);

            //Game
            Logic.textBoxes[Logic.gameNamespace + "CHAT"] = new TextBox(Logic.chatTexture, 10, Alignment.Left, Logic.font, new Rectangle(1380, 50, 540, 670), "You joined the game");
            Logic.inputBoxes[Logic.gameNamespace + "CHATINPUT"] = new InputBox(Logic.chatInputTexture, 10, Logic.font, new Rectangle(1380, 720, 490, 60), Color.White, Color.LightGray, "Enter message...", 120);
            Logic.buttons[Logic.gameNamespace + "SEND"] = new Button(Logic.chatSendTexture, new Rectangle(1870, 720, 50, 60), Logic.SendChatMessage);
            Logic.textBoxes[Logic.gameNamespace + "HELP"] = new TextBox(Logic.errorBackground, 5, Alignment.Centered, Logic.font, new Rectangle(0, 720, 1380, 60), "");
            Logic.textBoxes[Logic.gameNamespace + "HELP"].canScroll = false;
            Logic.buttons[Logic.gameNamespace + "READY"] = new Button(Logic.notReadyTexture, new Rectangle(420, 285, 540, 210), Logic.Ready);
            Logic.buttons[Logic.gameNamespace + "CHARACTER"] = new Button(Logic.chatTexture, new Rectangle(0, 780, 180, 300), null);
            Logic.textBoxes[Logic.gameNamespace + "ACTUALPLAYER"] = new TextBox(Logic.errorButton, 5, Alignment.Left, Logic.font, new Rectangle(1380, 0, 290, 50));
            Logic.buttons[Logic.gameNamespace + "DISCARD"] = new Button(Logic.discardTexture, new Rectangle(1380, 780, 540, 75), Logic.DiscardCard);
            Logic.buttons[Logic.gameNamespace + "REMOVE"] = new Button(Logic.errorButton, new Rectangle(1380, 855, 540, 75), null);
            Logic.buttons[Logic.gameNamespace + "MENU"] = new Button(Logic.settingsTexture, new Rectangle(1380, 930, 540, 75), () => Logic.DisplayOptions(true));
            Logic.buttons[Logic.gameNamespace + "EXIT"] = new Button(Logic.errorButton, new Rectangle(1380, 1005, 540, 75), null);
            Logic.grids[Logic.gameNamespace + "CARDS"] = new Grid(Logic.game, Logic.chatTexture, Logic.chatTexture, 6, 1, new Vector2(200, 300), new Rectangle(180, 780, 1200, 300), 0, Logic.HandClick, Logic.HandDraw);
            Logic.grids[Logic.gameNamespace + "BOARD"] = new Grid(Logic.game, Logic.chatTexture, Logic.chatTexture, 19, 15, new Vector2(105, 150), new Rectangle(0, 0, 1380, 720), 10, Logic.BoardClick, Logic.BoardDraw);
            Logic.grids[Logic.gameNamespace + "BOARD"].enabled = false;
            Logic.buttons[Logic.gameNamespace + "PLAYERLISTON"] = new Button(Logic.errorButton, new Rectangle(1670, 0, 250, 50), Logic.ShowPlayerList);
            Logic.buttons[Logic.gameNamespace + "PLAYERLISTOFF"] = new Button(Logic.chatSendTexture, new Rectangle(1670, 0, 250, 50), Logic.HidePlayerList);
            Logic.buttons[Logic.gameNamespace + "PLAYERLISTOFF"].enabled = false;
            Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"] = new Grid(Logic.game, Logic.chatTexture, Logic.chatTexture, 1, 10, new Vector2(250, 150), new Rectangle(1670, 50, 250, 1030), 1, Logic.PlayerListClick, Logic.PlayerListDraw);
            Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].offset = new Vector2(Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].location.Width / 2 - Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].margin, Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].location.Height / 2 - Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].margin);
            Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].enabled = false;
            Logic.grids[Logic.gameNamespace + "ZPLAYERLIST"].useScrollToScroll = true;
            DisableGroup(Logic.gameNamespace);
        }
    }
}
