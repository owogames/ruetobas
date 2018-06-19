using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Text;

using Keys = Microsoft.Xna.Framework.Input.Keys;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using MouseState = Microsoft.Xna.Framework.Input.MouseState;

namespace Ruetobas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    
    public class Game : Microsoft.Xna.Framework.Game
    {
        public TcpClient tcpClient = new TcpClient();
        public Stream stream;
        public ASCIIEncoding asen;
        public ThreadStart tcpThreadStart;
        public Thread tcpThread;

        RenderTarget2D screen;

        public bool TCPConnect(string IP, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IP, port);
                stream = tcpClient.GetStream();
                TCPSend("LOGIN " + Logic.username);

                byte[] bytes = new byte[1000];
                int length = stream.Read(bytes, 0, 1000);
                string msg = "";
                for (int i = 0; i < length; i++)
                    msg += Convert.ToChar(bytes[i]);

                if (msg.Substring(0, 2) == "OK")
                {
                    string[] data = msg.Split(' ');
                    for (int i = 1; i < data.Length; i += 2)
                    {
                        Logic.players.Add(new Player(i, data[i].Trim()));
                        Logic.players.Last().score = int.Parse(data[i + 1].Trim());
                    }
                    Logic.players.Add(new Player(data.Length, Logic.username));
                    Logic.SortPlayers();
                    tcpThreadStart = new ThreadStart(TCPListening);
                    tcpThread = new Thread(tcpThreadStart);
                    tcpThread.Start();
                    return true;
                }
                else
                {
                    Console.WriteLine(msg);
                    tcpClient.Close();
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection Timed Out: " + e.Message);
                return false;
            }
        }

        public void TCPSend(string msg)
        {
            msg += '\n';
            byte[] bytes = asen.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
        }
        
        public void TCPListening()
        {
            byte[] bytes = new byte[100];
            while (true)
            {
                int length = stream.Read(bytes, 0, 100);
                string msg = "";
                for (int i = 0; i < length; i++)
                    msg += Convert.ToChar(bytes[i]);
                Logic.TCPRecieved(msg);
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public static Texture2D cursorTexture;

        public static float scale = 1.0f;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)(1920 / scale);
            graphics.PreferredBackBufferHeight = (int)(1080 / scale);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            asen = new ASCIIEncoding();
            screen = new RenderTarget2D(GraphicsDevice, 1920, 1080);
            Logic.Init(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursorTexture = Content.Load<Texture2D>("cursor");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //Super funkcja ifowanie klawiszy hehe
        public bool TryConvertKeys(Keys key, out char charkey, bool shift)
        {
            switch (key)
            {
                //Alphabet keys
                case Keys.A: if (shift) { charkey = 'A'; } else { charkey = 'a'; } return true;
                case Keys.B: if (shift) { charkey = 'B'; } else { charkey = 'b'; } return true;
                case Keys.C: if (shift) { charkey = 'C'; } else { charkey = 'c'; } return true;
                case Keys.D: if (shift) { charkey = 'D'; } else { charkey = 'd'; } return true;
                case Keys.E: if (shift) { charkey = 'E'; } else { charkey = 'e'; } return true;
                case Keys.F: if (shift) { charkey = 'F'; } else { charkey = 'f'; } return true;
                case Keys.G: if (shift) { charkey = 'G'; } else { charkey = 'g'; } return true;
                case Keys.H: if (shift) { charkey = 'H'; } else { charkey = 'h'; } return true;
                case Keys.I: if (shift) { charkey = 'I'; } else { charkey = 'i'; } return true;
                case Keys.J: if (shift) { charkey = 'J'; } else { charkey = 'j'; } return true;
                case Keys.K: if (shift) { charkey = 'K'; } else { charkey = 'k'; } return true;
                case Keys.L: if (shift) { charkey = 'L'; } else { charkey = 'l'; } return true;
                case Keys.M: if (shift) { charkey = 'M'; } else { charkey = 'm'; } return true;
                case Keys.N: if (shift) { charkey = 'N'; } else { charkey = 'n'; } return true;
                case Keys.O: if (shift) { charkey = 'O'; } else { charkey = 'o'; } return true;
                case Keys.P: if (shift) { charkey = 'P'; } else { charkey = 'p'; } return true;
                case Keys.Q: if (shift) { charkey = 'Q'; } else { charkey = 'q'; } return true;
                case Keys.R: if (shift) { charkey = 'R'; } else { charkey = 'r'; } return true;
                case Keys.S: if (shift) { charkey = 'S'; } else { charkey = 's'; } return true;
                case Keys.T: if (shift) { charkey = 'T'; } else { charkey = 't'; } return true;
                case Keys.U: if (shift) { charkey = 'U'; } else { charkey = 'u'; } return true;
                case Keys.V: if (shift) { charkey = 'V'; } else { charkey = 'v'; } return true;
                case Keys.W: if (shift) { charkey = 'W'; } else { charkey = 'w'; } return true;
                case Keys.X: if (shift) { charkey = 'X'; } else { charkey = 'x'; } return true;
                case Keys.Y: if (shift) { charkey = 'Y'; } else { charkey = 'y'; } return true;
                case Keys.Z: if (shift) { charkey = 'Z'; } else { charkey = 'z'; } return true;

                //Decimal keys
                case Keys.D0: if (shift) { charkey = ')'; } else { charkey = '0'; } return true;
                case Keys.D1: if (shift) { charkey = '!'; } else { charkey = '1'; } return true;
                case Keys.D2: if (shift) { charkey = '@'; } else { charkey = '2'; } return true;
                case Keys.D3: if (shift) { charkey = '#'; } else { charkey = '3'; } return true;
                case Keys.D4: if (shift) { charkey = '$'; } else { charkey = '4'; } return true;
                case Keys.D5: if (shift) { charkey = '%'; } else { charkey = '5'; } return true;
                case Keys.D6: if (shift) { charkey = '^'; } else { charkey = '6'; } return true;
                case Keys.D7: if (shift) { charkey = '&'; } else { charkey = '7'; } return true;
                case Keys.D8: if (shift) { charkey = '*'; } else { charkey = '8'; } return true;
                case Keys.D9: if (shift) { charkey = '('; } else { charkey = '9'; } return true;

                //Decimal numpad keys
                case Keys.NumPad0: charkey = '0'; return true;
                case Keys.NumPad1: charkey = '1'; return true;
                case Keys.NumPad2: charkey = '2'; return true;
                case Keys.NumPad3: charkey = '3'; return true;
                case Keys.NumPad4: charkey = '4'; return true;
                case Keys.NumPad5: charkey = '5'; return true;
                case Keys.NumPad6: charkey = '6'; return true;
                case Keys.NumPad7: charkey = '7'; return true;
                case Keys.NumPad8: charkey = '8'; return true;
                case Keys.NumPad9: charkey = '9'; return true;

                //Special keys
                case Keys.OemTilde: if (shift) { charkey = '~'; } else { charkey = '`'; } return true;
                case Keys.OemSemicolon: if (shift) { charkey = ':'; } else { charkey = ';'; } return true;
                case Keys.OemQuotes: if (shift) { charkey = '"'; } else { charkey = '\''; } return true;
                case Keys.OemQuestion: if (shift) { charkey = '?'; } else { charkey = '/'; } return true;
                case Keys.OemPlus: if (shift) { charkey = '+'; } else { charkey = '='; } return true;
                case Keys.OemPipe: if (shift) { charkey = '|'; } else { charkey = '\\'; } return true;
                case Keys.OemPeriod: if (shift) { charkey = '>'; } else { charkey = '.'; } return true;
                case Keys.OemOpenBrackets: if (shift) { charkey = '{'; } else { charkey = '['; } return true;
                case Keys.OemCloseBrackets: if (shift) { charkey = '}'; } else { charkey = ']'; } return true;
                case Keys.OemMinus: if (shift) { charkey = '_'; } else { charkey = '-'; } return true;
                case Keys.OemComma: if (shift) { charkey = '<'; } else { charkey = ','; } return true;
                case Keys.Space: charkey = ' '; return true;
            }
            charkey = (char)0;
            return false;
        }

        string clipboard = "";
        private void GetClipboardHelper()
        {
            clipboard = "";
            if (Clipboard.ContainsText())
                clipboard = Clipboard.GetText();
        }

        private string GetClipboard()
        {
            Thread thread = new Thread(GetClipboardHelper);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return clipboard;
        }

        public MouseState mouseState, mouseBeforeState;
        public KeyboardState keyboardState, keyboardBeforeState;
        public Keys[] pressedKeys;

        double backspaceTimer = 0;
        double backspaceStart = 0;
        bool backspaceHeld = false;
        public InputBox activeInputBox = null;
        Grid draggedGrid;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            pressedKeys = keyboardState.GetPressedKeys();
            Vector2 mousePos = new Vector2(mouseState.X * scale, mouseState.Y * scale);
            
            //Left click
            if (mouseState.LeftButton == ButtonState.Pressed && mouseBeforeState.LeftButton == ButtonState.Released)
            {
                int i;
                for (i = Logic.buttons.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.buttons.ElementAt(i).Value.location, mousePos) && Logic.buttons.ElementAt(i).Value.enabled)
                    {
                        Logic.buttons.ElementAt(i).Value.clickEvent();
                        i = -2;
                    }
                }

                if (i != -2)
                for (i = Logic.inputBoxes.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.inputBoxes.ElementAt(i).Value.location, mousePos) && Logic.inputBoxes.ElementAt(i).Value.enabled)
                    {
                        if (activeInputBox != null)
                            activeInputBox.active = false;
                        activeInputBox = Logic.inputBoxes.ElementAt(i).Value;
                        activeInputBox.active = true;
                        i = -2;
                    }

                    if (i == 0)
                    {
                        if (activeInputBox != null)
                        {
                            activeInputBox.active = false;
                            activeInputBox = null;
                        }
                    }
                }

                if (i != -2)
                    for (i = Logic.grids.Count - 1; i >= 0; i--)
                    {
                        Grid grid = Logic.grids.ElementAt(i).Value;
                        if (Geo.RectContains(Geo.Shrink(grid.location, grid.margin), mousePos) && grid.enabled)
                        {
                            int pressX = (int)((mousePos.X - grid.location.X - grid.margin - grid.location.Width / 2 + grid.margin) / grid.zoom + grid.offset.X);
                            int pressY = (int)((mousePos.Y - grid.location.Y - grid.margin - grid.location.Height / 2 + grid.margin) / grid.zoom + grid.offset.Y);
                            int tileX = pressX / (int)grid.fieldSize.X;
                            int tileY = pressY / (int)grid.fieldSize.Y;
                            if (grid.clickEvent != null && tileX >= 0 && tileX < grid.sizeX && tileY >= 0 && tileY < grid.sizeY)
                                grid.clickEvent(tileX, tileY);
                            i = -2;
                        }
                    }
            }

            //Right click
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (mouseBeforeState.RightButton == ButtonState.Released)
                {
                    for (int i = Logic.grids.Count - 1; i >= 0; i--)
                    {
                        Grid grid = Logic.grids.ElementAt(i).Value;
                        if (Geo.RectContains(Geo.Shrink(grid.location, grid.margin), mousePos) && grid.enabled)
                            draggedGrid = grid;
                    }
                }
                else
                {
                    if (draggedGrid != null)
                    {
                        draggedGrid.offset -= new Vector2(mouseState.X * scale - mouseBeforeState.X * scale, mouseState.Y * scale - mouseBeforeState.Y * scale) / draggedGrid.zoom;
                    }
                }
            }
            else
            {
                draggedGrid = null;
            }

            //Konwersja klawiszy do inputBoxa
            if(activeInputBox != null)
            {
                if (keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    if (keyboardState.IsKeyDown(Keys.V) && keyboardBeforeState.IsKeyUp(Keys.V))
                        activeInputBox.Append(GetClipboard());
                }
                else if (keyboardState.IsKeyDown(Keys.Delete))
                    activeInputBox.Clear();
                else if (pressedKeys.Length > 0)
                {
                    foreach (Keys key in pressedKeys)
                    {
                        char charkey;
                        if (keyboardBeforeState.IsKeyUp(key) &&
                            TryConvertKeys(key, out charkey, keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)))
                        {
                            activeInputBox.Append(charkey);
                        }
                    }
                }
            }
            

            //Backspace
            if (keyboardState.IsKeyDown(Keys.Back) && activeInputBox != null)
            {
                if(backspaceHeld == false)
                {
                    if (activeInputBox.text.Length > 0)
                        activeInputBox.text = activeInputBox.text.Remove(activeInputBox.text.Length - 1);
                    backspaceStart = gameTime.TotalGameTime.TotalMilliseconds;
                }
                if(backspaceHeld && gameTime.TotalGameTime.TotalMilliseconds - backspaceStart > 500 && gameTime.TotalGameTime.TotalMilliseconds - backspaceTimer > 25)
                {
                    backspaceTimer = gameTime.TotalGameTime.TotalMilliseconds;
                    if (activeInputBox.text.Length > 0)
                        activeInputBox.text = activeInputBox.text.Remove(activeInputBox.text.Length - 1);
                }
                backspaceHeld = true;
            }
            else
            {
                backspaceHeld = false;
            }

            //Wy��czanie nieaktywnych obiekt�w
            if (activeInputBox != null && !activeInputBox.enabled)
                activeInputBox = null;
            if (draggedGrid != null && !draggedGrid.enabled)
                draggedGrid = null;

            //Scroll
            int scrollWheelDelta = mouseState.ScrollWheelValue - mouseBeforeState.ScrollWheelValue;
            if (scrollWheelDelta != 0)
            {
                int i;
                for (i = Logic.textBoxes.Count - 1; i >= 0; i--)
                {
                    TextBox textBox = Logic.textBoxes.ElementAt(i).Value;
                    if (Geo.RectContains(textBox.location, mousePos) && textBox.enabled && textBox.canScroll)
                    {
                        if (scrollWheelDelta < 0)
                            textBox.scroll++;
                        else if (textBox.scroll > 0)
                            textBox.scroll--;
                        i = -2;
                    }
                }

                if (i != -2)
                for (i = Logic.grids.Count - 1; i >= 0; i--)
                {
                    Grid grid = Logic.grids.ElementAt(i).Value;
                    if (Geo.RectContains(grid.location, mousePos) && grid.enabled)
                    {
                        if (scrollWheelDelta < 0 && grid.zoom > 0.5f)
                            grid.zoom -= 0.1f;
                        else if (scrollWheelDelta > 0 && grid.zoom < 2.0f)
                            grid.zoom += 0.1f;
                    }
                }
            }

            //Cofanie gdy przejedziesz grida za bardzo
            foreach (KeyValuePair<string, Grid> gridpair in Logic.grids)
            {
                Grid grid = gridpair.Value;
                if (grid.offset.X < grid.location.Width / 2 - grid.margin)
                    grid.offset.X = Math.Min(grid.offset.X + 20, grid.location.Width / 2 - grid.margin);
                if (grid.offset.Y < grid.location.Height / 2 - grid.margin)
                    grid.offset.Y = Math.Min(grid.offset.Y + 20, grid.location.Height / 2 - grid.margin);
                if (grid.offset.X > grid.sizeX * grid.fieldSize.X - grid.location.Width / 2 + grid.margin)
                    grid.offset.X = Math.Max(grid.offset.X - 20, grid.sizeX * grid.fieldSize.X - grid.location.Width / 2 + grid.margin);
                if (grid.offset.Y > grid.sizeY * grid.fieldSize.Y - grid.location.Height / 2 + grid.margin)
                    grid.offset.Y = Math.Max(grid.offset.Y - 20, grid.sizeY * grid.fieldSize.Y - grid.location.Height / 2 + grid.margin);
            }

            Logic.Update();

            mouseBeforeState = mouseState;
            keyboardBeforeState = keyboardState;
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        
        protected override void Draw(GameTime gameTime)
        {
            //Rysowanie grid�w
            foreach (KeyValuePair<string, Grid> gridpair in Logic.grids)
            {
                Grid grid = gridpair.Value;
                if (grid.enabled)
                {
                    spriteBatch.Begin();
                    GraphicsDevice.SetRenderTarget(grid.renderTarget);
                    GraphicsDevice.Clear(Color.Transparent);
                    for (int x = 0; x < grid.sizeX; x++)
                    {
                        for (int y = 0; y < grid.sizeY; y++)
                        {
                            Rectangle targetRect = new Rectangle(x * (int)grid.fieldSize.X - (int)grid.offset.X, y * (int)grid.fieldSize.Y - (int)grid.offset.Y, (int)grid.fieldSize.X, (int)grid.fieldSize.Y);
                            targetRect.X = (int)(grid.zoom * targetRect.X);
                            targetRect.Y = (int)(grid.zoom * targetRect.Y);
                            targetRect.Width = (int)(grid.zoom * targetRect.Width);
                            targetRect.Height = (int)(grid.zoom * targetRect.Height);
                            targetRect.X += grid.location.Width / 2 - grid.margin;
                            targetRect.Y += grid.location.Height / 2 - grid.margin;
                            if (targetRect.Intersects(new Rectangle(0, 0, grid.location.Width, grid.location.Height)))
                            {
                                grid.drawEvent(spriteBatch, targetRect, x, y);
                            }
                        }
                    }
                    spriteBatch.End();
                }
            }

            GraphicsDevice.SetRenderTarget(screen);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            List<string> UIelements = new List<string>();

            foreach (KeyValuePair<string, Button> pair in Logic.buttons)
                if (pair.Value.enabled)
                    UIelements.Add(pair.Key);

            foreach (KeyValuePair<string, TextBox> pair in Logic.textBoxes)
                if (pair.Value.enabled)
                    UIelements.Add(pair.Key);

            foreach (KeyValuePair<string, InputBox> pair in Logic.inputBoxes)
                if (pair.Value.enabled)
                    UIelements.Add(pair.Key);

            foreach (KeyValuePair<string, Grid> pair in Logic.grids)
                if (pair.Value.enabled)
                    UIelements.Add(pair.Key);

            UIelements.Sort();

            for (int id = 0; id < UIelements.Count; id++)
            {
                string name = UIelements[id];

                //Rysowanie guzik�w
                if (Logic.buttons.ContainsKey(name))
                {
                    Button button = Logic.buttons[name];
                    spriteBatch.Draw(button.texture, button.location, Color.White);
                }

                //Rysowanie textBox�w
                if (Logic.textBoxes.ContainsKey(name))
                {
                    TextBox textBox = Logic.textBoxes[name];
                    if (textBox.enabled)
                    {
                        spriteBatch.Draw(textBox.texture, textBox.location, Color.White);
                        for (int i = textBox.scroll; i < textBox.lines.Count && i < textBox.lineCount + textBox.scroll; i++)
                        {
                            float _x = 0;

                            if (textBox.align == Alignment.Left)
                                _x = textBox.location.X + textBox.margin;
                            else if (textBox.align == Alignment.Centered)
                                _x = textBox.location.X + (textBox.location.Width - textBox.font.MeasureString(textBox.lines[i]).X) / 2;

                            Vector2 position = new Vector2(_x, textBox.location.Y + textBox.font.LineSpacing * (i - textBox.scroll) + textBox.margin);

                            spriteBatch.DrawString(textBox.font, textBox.lines[i], position, Color.White);
                        }
                    }
                }

                //Rysowanie inputBox�w
                if (Logic.inputBoxes.ContainsKey(name))
                {
                    InputBox inputBox = Logic.inputBoxes[name];
                    spriteBatch.Draw(inputBox.texture, inputBox.location, inputBox.active ? Color.Gray : Color.White);
                    string text = inputBox.text;
                    if (inputBox.active) text += "|";
                    Vector2 position = new Vector2(inputBox.location.X + inputBox.margin, inputBox.location.Y + inputBox.location.Height / 2 - inputBox.font.LineSpacing / 2);

                    if (inputBox.text != "")
                    {
                        spriteBatch.DrawString(inputBox.font, text, position, inputBox.color);
                    }
                    else
                    {
                        spriteBatch.DrawString(inputBox.font, inputBox.emptyText, position, inputBox.emptyColor);
                    }
                }

                //Rysowanie grid�w
                if (Logic.grids.ContainsKey(name))
                {
                    Grid grid = Logic.grids[name];
                    spriteBatch.Draw(grid.boxTexture, grid.location, Color.White);
                    spriteBatch.Draw(grid.renderTarget, Geo.Shrink(grid.location, grid.margin), Color.White);
                }
            }

            //Rysowanie kursora
            spriteBatch.Draw(cursorTexture, new Rectangle((int)(Mouse.GetState().X * scale) - 16, (int)(Mouse.GetState().Y * scale), 32, 32), Color.White);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(screen, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
