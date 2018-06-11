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

        public void TCPConnect(string IP, int port)
        {
            try
            {
                tcpClient.Connect(IP, port);
                stream = tcpClient.GetStream();
                TCPSend("LOGIN " + Logic.username);
                tcpThreadStart = new ThreadStart(TCPListening);
                tcpThread = new Thread(tcpThreadStart);
                tcpThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection Timed Out: " + e.Message);
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
        public bool TryConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard, out char key)
        {
            Keys[] keys = keyboard.GetPressedKeys();
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = '('; } else { key = '9'; } return true;

                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;

                    //Special keys
                    case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } return true;
                    case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } return true;
                    case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } return true;
                    case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } return true;
                    case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } return true;
                    case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } return true;
                    case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } return true;
                    case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } return true;
                    case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } return true;
                    case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return true;
                    case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; } return true;
                    case Keys.Space: key = ' '; return true;
                }
            }
            key = (char)0;
            return false;
        }

        public MouseState mouseState, mouseBeforeState;
        public KeyboardState keyboardState, keyboardBeforeState;

        string activeInputBoxName = "";
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
            
            //Left click
            if (mouseState.LeftButton == ButtonState.Pressed && mouseBeforeState.LeftButton == ButtonState.Released)
            {
                for (int i = Logic.buttons.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.buttons.ElementAt(i).Value.location, new Vector2(mouseState.X, mouseState.Y)))
                    {
                        Logic.buttons.ElementAt(i).Value.clickEvent();
                        i = -1;
                    }
                }

                for (int i = Logic.inputBoxes.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.inputBoxes.ElementAt(i).Value.location, new Vector2(mouseState.X, mouseState.Y)))
                    {
                        if (activeInputBoxName != "")
                            Logic.inputBoxes[activeInputBoxName].active = false;
                        activeInputBoxName = Logic.inputBoxes.ElementAt(i).Key;
                        Logic.inputBoxes[activeInputBoxName].active = true;
                        i = -1;
                    }

                    if (i == 0)
                    {
                        if (activeInputBoxName != "")
                        {
                            Logic.inputBoxes[activeInputBoxName].active = false;
                            activeInputBoxName = "";
                        }
                    }
                }
            }

            char key;
            if (TryConvertKeyboardInput(keyboardState, keyboardBeforeState, out key))
            {
                if (activeInputBoxName != "")
                {
                    Logic.inputBoxes[activeInputBoxName].text += key;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Back) && activeInputBoxName != "")
            {
                if (Logic.inputBoxes[activeInputBoxName].text.Length > 0)
                    Logic.inputBoxes[activeInputBoxName].text = Logic.inputBoxes[activeInputBoxName].text.Remove(Logic.inputBoxes[activeInputBoxName].text.Length - 1);
            }

            //Scroll
            int scrollWheelDelta = mouseState.ScrollWheelValue - mouseBeforeState.ScrollWheelValue;
            if (scrollWheelDelta != 0)
            {
                for (int i = Logic.textBoxes.Count - 1; i >= 0; i--)
                {
                    TextBox textBox = Logic.textBoxes.ElementAt(i).Value;
                    if (Geo.RectContains(textBox.location, new Vector2(mouseState.X, mouseState.Y)))
                    {
                        if (scrollWheelDelta < 0)
                            textBox.scroll++;
                        else if (textBox.scroll > 0)
                            textBox.scroll--;
                        i = -1;
                    }
                }
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
                        new Vector2(textBox.Value.location.X, textBox.Value.location.Y + textBox.Value.font.LineSpacing * (i - textBox.Value.scroll)), Color.White);
            }

            foreach (KeyValuePair<string, InputBox> inputBox in Logic.inputBoxes)
            {
                // InputBox IB = inputBox.Value; (referencja)

                spriteBatch.Draw(inputBox.Value.texture, inputBox.Value.location, inputBox.Value.active ? Color.Gray : Color.White);
                string text = inputBox.Value.text;
                if (inputBox.Value.active) text += "|";

                if (inputBox.Value.text != "")
                {
                    spriteBatch.DrawString(inputBox.Value.font, text, new Vector2(inputBox.Value.location.X, inputBox.Value.location.Y), inputBox.Value.color);
                }
                else
                {
                    spriteBatch.DrawString(inputBox.Value.font, inputBox.Value.emptyText, new Vector2(inputBox.Value.location.X, inputBox.Value.location.Y), inputBox.Value.emptyColor);
                }
            }

            spriteBatch.Draw(cursorTexture, new Rectangle(Mouse.GetState().X - 16, Mouse.GetState().Y, 32, 32), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
