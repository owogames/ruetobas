using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Text;

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

        public bool TCPConnect(string IP, int port)
        {
            try
            {
                tcpClient.Connect(IP, port);
                stream = tcpClient.GetStream();
                TCPSend("LOGIN " + Logic.username);

                byte[] bytes = new byte[100];
                int length = stream.Read(bytes, 0, 100);
                string msg = "";
                for (int i = 0; i < length; i++)
                    msg += Convert.ToChar(bytes[i]);
                if (msg.Substring(0, 2) == "OK")
                {
                    tcpThreadStart = new ThreadStart(TCPListening);
                    tcpThread = new Thread(tcpThreadStart);
                    tcpThread.Start();
                    return true;
                }
                else
                {
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

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
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

        public MouseState mouseState, mouseBeforeState;
        public KeyboardState keyboardState, keyboardBeforeState;
        public Keys[] pressedKeys;


        double backspaceTimer = 0;
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
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
            
            //Left click
            if (mouseState.LeftButton == ButtonState.Pressed && mouseBeforeState.LeftButton == ButtonState.Released)
            {
                for (int i = Logic.buttons.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.buttons.ElementAt(i).Value.location, mousePos))
                    {
                        Logic.buttons.ElementAt(i).Value.clickEvent();
                        i = -1;
                    }
                }

                for (int i = Logic.inputBoxes.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.inputBoxes.ElementAt(i).Value.location, mousePos))
                    {
                        if (activeInputBox != null)
                            activeInputBox.active = false;
                        activeInputBox = Logic.inputBoxes.ElementAt(i).Value;
                        activeInputBox.active = true;
                        i = -1;
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

                for (int i = Logic.grids.Count - 1; i >= 0; i--)
                {
                    Grid grid = Logic.grids.ElementAt(i).Value;
                    if (Geo.RectContains(Geo.Shrink(grid.location, grid.margin), mousePos))
                    {
                        int tileX = ((int)mousePos.X - grid.location.X - grid.margin + (int)grid.offset.X) / (int)grid.fieldSize.X;
                        int tileY = ((int)mousePos.Y - grid.location.Y - grid.margin + (int)grid.offset.Y) / (int)grid.fieldSize.Y;
                        grid.clickEvent(tileX, tileY);
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (mouseBeforeState.RightButton == ButtonState.Released)
                {
                    for (int i = Logic.grids.Count - 1; i >= 0; i--)
                    {
                        Grid grid = Logic.grids.ElementAt(i).Value;
                        if (Geo.RectContains(Geo.Shrink(grid.location, grid.margin), mousePos))
                            draggedGrid = grid;
                    }
                }
                else
                {
                    if (draggedGrid != null)
                    {
                        draggedGrid.offset -= new Vector2(mouseState.X - mouseBeforeState.X, mouseState.Y - mouseBeforeState.Y);
                    }
                }
            }
            else
            {
                draggedGrid = null;
            }
            
            if (pressedKeys.Length > 0 && activeInputBox != null)
            {
                foreach (Keys key in pressedKeys)
                {
                    char charkey;
                    if (keyboardBeforeState.IsKeyUp(key) &&
                        TryConvertKeys(key, out charkey, keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)))
                    {
                        if (activeInputBox.font.MeasureString(activeInputBox.text + charkey).X <= activeInputBox.location.Width - 2 * activeInputBox.margin)
                            activeInputBox.text += charkey;
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.Back) && activeInputBox != null && gameTime.TotalGameTime.TotalMilliseconds - backspaceTimer > 75)
            {
                backspaceTimer = gameTime.TotalGameTime.TotalMilliseconds;
                if (activeInputBox.text.Length > 0)
                    activeInputBox.text = activeInputBox.text.Remove(activeInputBox.text.Length - 1);
            }

            //Scroll
            int scrollWheelDelta = mouseState.ScrollWheelValue - mouseBeforeState.ScrollWheelValue;
            if (scrollWheelDelta != 0)
            {
                for (int i = Logic.textBoxes.Count - 1; i >= 0; i--)
                {
                    TextBox textBox = Logic.textBoxes.ElementAt(i).Value;
                    if (Geo.RectContains(textBox.location, mousePos))
                    {
                        if (scrollWheelDelta < 0)
                            textBox.scroll++;
                        else if (textBox.scroll > 0)
                            textBox.scroll--;
                        i = -1;
                    }
                }
            }

            foreach (KeyValuePair<string, Grid> gridpair in Logic.grids)
            {
                Grid grid = gridpair.Value;
                if (grid.offset.X < 0)
                    grid.offset.X += 10;
                if (grid.offset.Y < 0)
                    grid.offset.Y += 10;
                if (grid.offset.X > grid.sizeX * grid.fieldSize.X - grid.location.Width + 2 * grid.margin)
                    grid.offset.X -= 10;
                if (grid.offset.Y > grid.sizeY * grid.fieldSize.Y - grid.location.Height + 2 * grid.margin)
                    grid.offset.Y -= 10;
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
            foreach (KeyValuePair<string, Grid> gridpair in Logic.grids)
            {
                Grid grid = gridpair.Value;
                spriteBatch.Begin();
                GraphicsDevice.SetRenderTarget(grid.renderTarget);
                GraphicsDevice.Clear(Color.Transparent);
                for (int x = 0; x < grid.sizeX; x++)
                {
                    for (int y = 0; y < grid.sizeY; y++)
                    {
                        Rectangle targetRect = new Rectangle(x * (int)grid.fieldSize.X - (int)grid.offset.X, y * (int)grid.fieldSize.Y - (int)grid.offset.Y, (int)grid.fieldSize.X, (int)grid.fieldSize.Y);
                        if (targetRect.Intersects(new Rectangle(0, 0, grid.location.Width, grid.location.Height)))
                        {
                            spriteBatch.Draw(grid.fieldTexture[x, y], targetRect, Color.White);
                        }
                    }
                }
                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (KeyValuePair<string, Button> button in Logic.buttons)
            {
                spriteBatch.Draw(button.Value.texture, button.Value.location, Color.White);
            }

            foreach (KeyValuePair<string, TextBox> textBox in Logic.textBoxes)
            {
                spriteBatch.Draw(textBox.Value.texture, textBox.Value.location, Color.White);
                for (int i = textBox.Value.scroll; i < textBox.Value.lines.Count && i < textBox.Value.lineCount + textBox.Value.scroll; i++)
                    spriteBatch.DrawString(textBox.Value.font, textBox.Value.lines[i],
                        new Vector2(textBox.Value.location.X + textBox.Value.margin, textBox.Value.location.Y + textBox.Value.font.LineSpacing * (i - textBox.Value.scroll)), Color.White);
            }

            foreach (KeyValuePair<string, InputBox> inputBox in Logic.inputBoxes)
            {
                // InputBox IB = inputBox.Value; (referencja)

                spriteBatch.Draw(inputBox.Value.texture, inputBox.Value.location, inputBox.Value.active ? Color.Gray : Color.White);
                string text = inputBox.Value.text;
                if (inputBox.Value.active) text += "|";
                Vector2 position = new Vector2(inputBox.Value.location.X + inputBox.Value.margin, inputBox.Value.location.Y + inputBox.Value.location.Height / 2 - inputBox.Value.font.LineSpacing / 2);

                if (inputBox.Value.text != "")
                {
                    spriteBatch.DrawString(inputBox.Value.font, text, position, inputBox.Value.color);
                }
                else
                {
                    spriteBatch.DrawString(inputBox.Value.font, inputBox.Value.emptyText, position, inputBox.Value.emptyColor);
                }
            }

            foreach (KeyValuePair<string, Grid> gridpair in Logic.grids)
            {
                Grid grid = gridpair.Value;
                spriteBatch.Draw(grid.boxTexture, grid.location, Color.White);
                spriteBatch.Draw(grid.renderTarget, Geo.Shrink(grid.location, grid.margin), Color.White);
            }

            spriteBatch.Draw(cursorTexture, new Rectangle(Mouse.GetState().X - 16, Mouse.GetState().Y, 32, 32), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
