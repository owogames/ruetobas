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
        public ThreadStart tcpThreadStart, loadThreadStart;
        public Thread tcpThread, loadThread;

        public bool isLoading = true;
        public string loadingString = "";

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

                    Logic.players.Clear();
                    for (int i = 0; i < 6; i++)
                        Logic.cardHand[i] = 0;
                    for (int i = 0; i < 19; i++)
                        for (int j = 0; j < 15; j++)
                            Logic.map[i, j] = new PlacedCard(0, 0);

                    for (int i = 1; i < data.Length; i += 2)
                    {
                        Logic.players.Add(new Player(i, data[i].Trim()));
                        Logic.players.Last().score = int.Parse(data[i + 1].Trim());
                    }
                    Logic.players.Add(new Player(data.Length, Logic.username));
                    Logic.SortPlayers();
                    Logic.adminMode = false;
                    Logic.gameInProgress = false;
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
                Logic.AnnounceError("Connection timed out");
                Console.WriteLine("Connection Timed Out: " + e.Message);
                return false;
            }
        }

        public void TCPSend(string msg)
        {
            try
            {
                msg += '\n';
                byte[] bytes = asen.GetBytes(msg);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logic.TCPRecieved("DISCONNECT Error sending packet to server\n");
            }
        }
        
        public void TCPListening()
        {
            byte[] bytes = new byte[100];
            while (true)
            {
                try
                {
                    int length = stream.Read(bytes, 0, 100);
                    string msg = "";
                    for (int i = 0; i < length; i++)
                        msg += Convert.ToChar(bytes[i]);
                    Logic.TCPRecieved(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Logic.TCPRecieved("DISCONNECT Error recieving packet from server\n");
                }
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D cursorTexture;

        public static Vector2 scale = Vector2.One * 1280.0f/1920.0f;
        
        public void ChangeResolution(float X, float Y)
        {
            int width = (int)X;
            int height = (int)Y;
            Settings.resolution = new Point(width, height);
            scale = new Vector2(width / 1920.0f, height / 1080.0f);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
        }

        public void ChangeResolution(Point res) => ChangeResolution(res.X, res.Y);

        public void ChangeFullscreen()
        {
            graphics.ToggleFullScreen();
            Settings.isFullscreen = !Settings.isFullscreen;
        }

        public Point GetCurrentDeviceResolution()
        {
            Point resolution;
            resolution.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            resolution.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            return resolution;
        }

        public Point GetCurrentWindowResolution() => new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

        public DisplayMode[] GetDisplayModes()
        {
            return GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToArray();
        }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Settings.resolution.X;
            graphics.PreferredBackBufferHeight = Settings.resolution.Y;
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
            base.Initialize();
            Settings.Initialize(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Logic.spriteBatch = spriteBatch;
            cursorTexture = Content.Load<Texture2D>("cursor");
            Logic.font = Content.Load<SpriteFont>("comic");
            Logic.guifont = Content.Load<SpriteFont>("guifont");
            Logic.Init(this);

            isLoading = true;
            loadThreadStart = new ThreadStart(Logic.LoadContent);
            loadThread = new Thread(loadThreadStart);
            loadThread.Start();
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
            if (isLoading)
            {
                if (!loadThread.IsAlive)
                {
                    UI.InitUI();
                    isLoading = false;
                    Logic.bye.Play();
                    Animation logoAnimation = new Animation(() =>
                    {
                        Logic.images.Remove("LOGO");
                        UI.EnableGroup(Logic.menuNamespace);
                        UI.DisableGroup(Logic.menuNamespace + "Z");
                        UI.GenerateFadeIn(1.0f);
                        Logic.menuMusicInstance = Logic.menuMusic.CreateInstance();
                        Logic.menuMusicInstance.IsLooped = true;
                        Logic.menuMusicInstance.Play();
                    });
                    logoAnimation.AddFadeInOut(2.0f, 5.0f, 2.0f, Logic.images["LOGO"]);
                    Logic.animations.Add(logoAnimation);
                }
                return;
            }
            

            if (this.IsActive)
            {
                mouseState = Mouse.GetState();
                keyboardState = Keyboard.GetState();
                pressedKeys = keyboardState.GetPressedKeys();
                Vector2 mousePos = new Vector2(mouseState.X / scale.X, mouseState.Y / scale.Y);
                
                for (int i = Logic.animations.Count - 1; i >= 0; i--)
                {
                    if (Logic.animations[i].Update((float)gameTime.ElapsedGameTime.TotalSeconds))
                        Logic.animations.RemoveAt(i);
                }

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

                //Left click
                if (mouseState.LeftButton == ButtonState.Pressed && mouseBeforeState.LeftButton == ButtonState.Released)
                {
                    int i;
                    for (i = UIelements.Count - 1; i >= 0; i--)
                    {
                        string name = UIelements[i];
                        //Klikanie buttonów
                        if (Logic.buttons.ContainsKey(name))
                        {
                            if (Geo.RectContains(Logic.buttons[name].location, mousePos) && Logic.buttons[name].enabled && Logic.buttons[name].registerClicks)
                            {
                                if (activeInputBox != null)
                                    activeInputBox.active = false;
                                activeInputBox = null;
                                if (Logic.buttons[name].clickEvent != null)
                                    Logic.buttons[name].clickEvent();
                                i = -1;
                            }
                        }

                        if (Logic.textBoxes.ContainsKey(name))
                        {
                            if (Geo.RectContains(Logic.textBoxes[name].location, mousePos) && Logic.textBoxes[name].enabled)
                            {
                                if (activeInputBox != null)
                                    activeInputBox.active = false;
                                activeInputBox = null;
                                i = -1;
                            }
                        }

                        if (Logic.inputBoxes.ContainsKey(name))
                        {
                            if (Geo.RectContains(Logic.inputBoxes[name].location, mousePos) && Logic.inputBoxes[name].enabled)
                            {
                                if (activeInputBox != null)
                                    activeInputBox.active = false;
                                activeInputBox = Logic.inputBoxes[name];
                                activeInputBox.active = true;
                                i = -2;
                            }
                        }

                        if (Logic.grids.ContainsKey(name))
                        {
                            Grid grid = Logic.grids[name];
                            if (Geo.RectContains(Geo.Shrink(grid.location, grid.margin), mousePos) && grid.enabled)
                            {
                                if (activeInputBox != null)
                                    activeInputBox.active = false;
                                activeInputBox = null;
                                int pressX = (int)((mousePos.X - grid.location.X - grid.margin - grid.location.Width / 2 + grid.margin) / grid.zoom + grid.offset.X);
                                int pressY = (int)((mousePos.Y - grid.location.Y - grid.margin - grid.location.Height / 2 + grid.margin) / grid.zoom + grid.offset.Y);
                                int tileX = pressX / (int)grid.fieldSize.X;
                                int tileY = pressY / (int)grid.fieldSize.Y;
                                if (grid.clickEvent != null && tileX >= 0 && tileX < grid.sizeX && tileY >= 0 && tileY < grid.sizeY)
                                    grid.clickEvent(tileX, tileY);
                                i = -1;
                            }
                        }
                    }

                    if (i == -1)
                    {
                        if (activeInputBox != null)
                        {
                            activeInputBox.active = false;
                            activeInputBox = null;
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
                            if (Geo.RectContains(Geo.Shrink(grid.location, grid.margin), mousePos) && grid.enabled && !grid.useScrollToScroll)
                                draggedGrid = grid;
                        }
                    }
                    else
                    {
                        if (draggedGrid != null)
                        {
                            draggedGrid.offset -= new Vector2(mouseState.X / scale.X - mouseBeforeState.X / scale.X, mouseState.Y / scale.Y - mouseBeforeState.Y / scale.Y) / draggedGrid.zoom;
                        }
                    }
                }
                else
                {
                    draggedGrid = null;
                }

                //Konwersja klawiszy do inputBoxa
                if (activeInputBox != null)
                {
                    if (keyboardState.IsKeyDown(Keys.LeftControl))
                    {
                        if (keyboardState.IsKeyDown(Keys.V) && keyboardBeforeState.IsKeyUp(Keys.V))
                        {
                            string sOut = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(GetClipboard()));
                            activeInputBox.Append(sOut);
                        }
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
                    if (backspaceHeld == false)
                    {
                        if (activeInputBox.text.Length > 0)
                            activeInputBox.text = activeInputBox.text.Remove(activeInputBox.text.Length - 1);
                        backspaceStart = gameTime.TotalGameTime.TotalMilliseconds;
                    }
                    if (backspaceHeld && gameTime.TotalGameTime.TotalMilliseconds - backspaceStart > 500 && gameTime.TotalGameTime.TotalMilliseconds - backspaceTimer > 25)
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

                //Scroll
                int scrollWheelDelta = mouseState.ScrollWheelValue - mouseBeforeState.ScrollWheelValue;
                if (scrollWheelDelta != 0)
                {
                    int i;
                    for (i = UIelements.Count - 1; i >= 0; i--)
                    {
                        string name = UIelements[i];
                        if (Logic.buttons.ContainsKey(name))
                        {
                            Button button = Logic.buttons[name];
                            if (Geo.RectContains(button.location, mousePos) && button.enabled)
                                i = -2;
                        }

                        if (Logic.inputBoxes.ContainsKey(name))
                        {
                            InputBox inputBox = Logic.inputBoxes[name];
                            if (Geo.RectContains(inputBox.location, mousePos) && inputBox.enabled)
                                i = -2;
                        }

                        if (Logic.textBoxes.ContainsKey(name))
                        {
                            TextBox textBox = Logic.textBoxes[name];
                            if (Geo.RectContains(textBox.location, mousePos) && textBox.enabled)
                            {
                                if (textBox.canScroll)
                                {
                                    if (scrollWheelDelta < 0)
                                        textBox.scroll++;
                                    else if (textBox.scroll > 0)
                                        textBox.scroll--;

                                    if (textBox.lines.Count - textBox.scroll < textBox.lineCount)
                                        textBox.scroll = textBox.lines.Count - textBox.lineCount;

                                    if (textBox.scroll < 0)
                                        textBox.scroll = 0;
                                }

                                i = -2;
                            }
                        }

                        if (Logic.grids.ContainsKey(name))
                        {
                            Grid grid = Logic.grids[name];
                            if (Geo.RectContains(grid.location, mousePos) && grid.enabled)
                            {
                                if (grid.useScrollToScroll)
                                {
                                    if (scrollWheelDelta < 0)
                                        grid.offset.Y += 25.0f;
                                    else if (scrollWheelDelta > 0)
                                        grid.offset.Y -= 25.0f;
                                }
                                else
                                {
                                    if (scrollWheelDelta < 0 && grid.zoom > 0.5f)
                                        grid.zoom -= 0.1f;
                                    else if (scrollWheelDelta > 0 && grid.zoom < 2.0f)
                                        grid.zoom += 0.1f;
                                }
                                i = -2;
                            }
                        }
                    }
                }
            }

            //Wy³¹czanie nieaktywnych obiektów
            if (activeInputBox != null && !activeInputBox.enabled)
                activeInputBox = null;
            if (draggedGrid != null && !draggedGrid.enabled)
                draggedGrid = null;

            //Cofanie gdy przejedziesz grida za bardzo
            foreach (KeyValuePair<string, Grid> gridpair in Logic.grids)
            {
                Grid grid = gridpair.Value;
                int marginX = (int)((grid.location.Width / 2 - grid.margin) / grid.zoom);
                int marginY = (int)((grid.location.Height / 2 - grid.margin) / grid.zoom);
                if (grid.offset.X < marginX)
                    grid.offset.X = Math.Min(grid.offset.X + 25, marginX);
                if (grid.offset.Y < marginY)
                    grid.offset.Y = Math.Min(grid.offset.Y + 25, marginY);
                if (grid.offset.X > grid.sizeX * grid.fieldSize.X - marginX)
                    grid.offset.X = Math.Max(grid.offset.X - 25, grid.sizeX * grid.fieldSize.X - marginX);
                if (grid.offset.Y > grid.sizeY * grid.fieldSize.Y - marginY)
                    grid.offset.Y = Math.Max(grid.offset.Y - 25, grid.sizeY * grid.fieldSize.Y - marginY);
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
            //Loading
            if (isLoading)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                spriteBatch.DrawString(Logic.guifont, loadingString, (new Vector2(Settings.resolution.X, Settings.resolution.Y) - Logic.guifont.MeasureString(loadingString)) / 2, Color.White);
                spriteBatch.End();
                return;
            }

            //Rysowanie gridów
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
                            targetRect.Width = (int)(grid.zoom * targetRect.Width) + 1;
                            targetRect.Height = (int)(grid.zoom * targetRect.Height) + 1;
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

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

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

            foreach (KeyValuePair<string, RawImage> pair in Logic.images)
                if (pair.Value.enabled)
                    UIelements.Add(pair.Key);

            UIelements.Sort();

            for (int id = 0; id < UIelements.Count; id++)
            {
                string name = UIelements[id];

                //Rysowanie guzików
                if (Logic.buttons.ContainsKey(name))
                {
                    Button button = Logic.buttons[name];
                    spriteBatch.Draw(button.texture, Geo.Scale(button.location), Color.White);
                }

                //Rysowanie textBoxów
                if (Logic.textBoxes.ContainsKey(name))
                {
                    TextBox textBox = Logic.textBoxes[name];
                    if (textBox.enabled)
                    {
                        spriteBatch.Draw(textBox.texture, Geo.Scale(textBox.location), Color.White);
                        for (int i = textBox.scroll; i < textBox.lines.Count && i < textBox.lineCount + textBox.scroll; i++)
                        {
                            float _x = 0;

                            if (textBox.align == Alignment.Left)
                                _x = textBox.location.X + textBox.margin;
                            else if (textBox.align == Alignment.Centered)
                                _x = textBox.location.X + (textBox.location.Width - textBox.font.MeasureString(textBox.lines[i]).X) / 2;
                            _x = (int)_x;

                            Vector2 position = new Vector2(_x, textBox.location.Y + textBox.font.LineSpacing * (i - textBox.scroll) + textBox.margin);

                            spriteBatch.DrawString(textBox.font, textBox.lines[i], position * scale, textBox.colors[i], 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                        }
                    }
                }

                //Rysowanie inputBoxów
                if (Logic.inputBoxes.ContainsKey(name))
                {
                    InputBox inputBox = Logic.inputBoxes[name];
                    spriteBatch.Draw(inputBox.texture, Geo.Scale(inputBox.location), inputBox.active ? Color.Gray : Color.White);
                    string text = inputBox.GetText();
                    if (inputBox.active) text += "|";
                    Vector2 position = new Vector2(inputBox.location.X + inputBox.margin, inputBox.location.Y + inputBox.location.Height / 2 - inputBox.font.LineSpacing / 2);

                    if (inputBox.text != "")
                    {
                        spriteBatch.DrawString(inputBox.font, text, position * scale, inputBox.color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.DrawString(inputBox.font, inputBox.emptyText, position * scale, inputBox.emptyColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                    }
                }

                //Rysowanie gridów
                if (Logic.grids.ContainsKey(name))
                {
                    Grid grid = Logic.grids[name];
                    spriteBatch.Draw(grid.boxTexture, Geo.Scale(grid.location), Color.White);
                    spriteBatch.Draw(grid.renderTarget, Geo.Scale(Geo.Shrink(grid.location, grid.margin)), Color.White);
                }

                //Rysowanie RawImage
                if (Logic.images.ContainsKey(name))
                {
                    RawImage image = Logic.images[name];
                    spriteBatch.Draw(image.texture, Geo.Scale(new Rectangle(image.location.X + image.location.Width / 2, image.location.Y + image.location.Height / 2, image.location.Width, image.location.Height)), null, Color.White * image.opacity, image.rotation, new Vector2(image.texture.Width, image.texture.Height) / 2, SpriteEffects.None, 0);
                }
            }

            //Rysowanie kursora
            spriteBatch.Draw(cursorTexture, new Rectangle(Mouse.GetState().X - 16, Mouse.GetState().Y, 32, 32), Color.White);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            if (tcpThread != null)
            {
                TCPSend("QUIT");
                tcpThread.Abort();
            }
            Exit();
        }
    }
}
