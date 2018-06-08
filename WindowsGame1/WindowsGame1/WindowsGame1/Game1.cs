using System;
using System.Net;
using System.Net.Sockets;
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
        public void TestowaFunkcja()
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect("192.168.1.197", 2137);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");
                Console.Write("Enter the string to be transmitted : ");

                String str = "Hello Uorld";
                Stream stm = tcpclnt.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);

                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                Console.WriteLine(k);
                for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));
                Console.WriteLine();
                
                tcpclnt.Close();
                
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        public static Texture2D cursorTexture;

        public Game()
        {
            //TestowaFunkcja();
            graphics = new GraphicsDeviceManager(this);
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

            Logic.Init(this);
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

        MouseState state, beforeState;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            state = Mouse.GetState();
            
            //Left click
            if (state.LeftButton == ButtonState.Pressed && beforeState.LeftButton == ButtonState.Released)
            {
                for (int i = Logic.buttons.Count - 1; i >= 0; i--)
                {
                    if (Geo.RectContains(Logic.buttons.ElementAt(i).Value.location, new Vector2(state.X, state.Y)))
                    {
                        Logic.buttons.ElementAt(i).Value.clickEvent();
                        i = -1;
                    }
                }
            }

            int scrollWheelDelta = state.ScrollWheelValue - beforeState.ScrollWheelValue;
            if (scrollWheelDelta != 0)
            {
                for (int i = Logic.textBoxes.Count - 1; i >= 0; i--)
                {
                    TextBox textBox = Logic.textBoxes.ElementAt(i).Value;
                    if (Geo.RectContains(textBox.location, new Vector2(state.X, state.Y)))
                    {
                        if (scrollWheelDelta < 0)
                            textBox.scroll++;
                        else if (textBox.scroll > 0)
                            textBox.scroll--;
                        i = -1;
                    }
                }
            }

            Logic.Update(this);

            beforeState = state;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
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

            spriteBatch.Draw(cursorTexture, new Rectangle(Mouse.GetState().X - 16, Mouse.GetState().Y - 16, 32, 32), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
