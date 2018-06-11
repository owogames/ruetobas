﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public static class Logic
    {
        public static Dictionary<string, Button> buttons;
        public static Dictionary<string, TextBox> textBoxes;
        public static Dictionary<string, InputBox> inputBoxes;

        public static Game game;
        public static Texture2D chatTexture;
        public static Texture2D chatInputTexture;
        public static Texture2D chatSendTexture;
        public static SpriteFont font;

        public const int port = 2137;

        public static string IP = "192.168.0.157";
        public static string username = "No Elo";

        public static void Init(Game game)
        {
            Logic.game = game;
            buttons = new Dictionary<string, Button>();
            textBoxes = new Dictionary<string, TextBox>();
            inputBoxes = new Dictionary<string, InputBox>();

            chatTexture = game.Content.Load<Texture2D>("tekstura");
            chatInputTexture = game.Content.Load<Texture2D>("tekstura2");
            chatSendTexture = game.Content.Load<Texture2D>("tekstura3");
            font = game.Content.Load<SpriteFont>("font");

            inputBoxes["ip"] = new InputBox(chatInputTexture, font, new Rectangle(140, 300, 1000, 50), Color.White, Color.LightGray, "Enter server IP");
            inputBoxes["nick"] = new InputBox(chatInputTexture, font, new Rectangle(140, 400, 1000, 50), Color.White, Color.LightGray, "Enter choosen username");
            buttons["connect"] = new Button(chatSendTexture, new Rectangle(500, 500, 380, 50), LoadGameScreen);
            textBoxes["errorbox"] = new TextBox(chatTexture, font, new Rectangle(140, 650, 1000, 50));
        }

        public static int timer = 0;
        public static void Update()
        {
            if (game.keyboardState.IsKeyDown(Keys.Escape) && game.keyboardBeforeState.IsKeyUp(Keys.Escape))
            {
                if (game.tcpThread != null)
                {
                    game.TCPSend("QUIT");
                    game.tcpThread.Abort();
                }
                game.Exit();
            }

            if (game.keyboardState.IsKeyDown(Keys.Enter) && game.keyboardBeforeState.IsKeyUp(Keys.Enter) && inputBoxes["CHATINPUT"].active)
                SendChatMessage();
        }

        public static void TCPRecieved(string message)
        {
            Console.WriteLine(message);
            if (message.Substring(0, 4) == "CHAT")
                textBoxes["CHAT"].Append(message.Substring(5).Trim());
            if (message.Substring(0, 4) == "JOIN")
                textBoxes["CHAT"].Append(message.Substring(5).Trim() + " has joined the chat.");
            if (message.Substring(0, 3) == "BYE")
                textBoxes["CHAT"].Append(message.Substring(4).Trim() + " has left the chat.");
            if (message.Substring(0, 5) == "ERROR")
            {
                textBoxes["CHAT"].Append(message.Substring(6).Trim());
                game.tcpThread.Abort();
            }
        }

        public static void SendChatMessage()
        {
            game.TCPSend("CHAT " + username + ": " + inputBoxes["CHATINPUT"].text);
            inputBoxes["CHATINPUT"].text = "";
        }

        public static void LoadGameScreen()
        {
            IP = inputBoxes["ip"].text;
            username = inputBoxes["nick"].text;

            if (game.TCPConnect(IP, port))
            {
                inputBoxes.Clear();
                textBoxes["errorbox"].lines.Clear();
                textBoxes["errorbox"].lines.Add("Successfully connected");
                textBoxes.Clear();
                textBoxes["CHAT"] = new TextBox(chatTexture, font, new Rectangle(140, 100, 1000, 500));
                inputBoxes["CHATINPUT"] = new InputBox(chatInputTexture, font, new Rectangle(140, 650, 800, 50), Color.White, Color.LightGray, "Enter message...");
                buttons["SEND"] = new Button(chatSendTexture, new Rectangle(990, 650, 150, 50), SendChatMessage);
            }
            else
            {
                textBoxes["errorbox"].lines.Clear();
                textBoxes["errorbox"].lines.Add("Error, try again");
            }
        }
    }
}
