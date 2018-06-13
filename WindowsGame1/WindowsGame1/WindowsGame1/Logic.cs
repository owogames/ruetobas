using Microsoft.Xna.Framework;
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
        public static Dictionary<string, Grid> grids;

        public static Game game;
        public static Texture2D chatTexture;
        public static Texture2D chatInputTexture;
        public static Texture2D chatSendTexture;
        public static Texture2D skurwielTexture;
        public static Texture2D[] cardTexture = new Texture2D[46];
        public static SpriteFont font;

        public static PlacedCard[,] map;

        public const int port = 2137;

        public static string IP = "192.168.0.157";
        public static string username = "No Elo";

        public static List<Player> players = new List<Player>();

        public static void Init(Game game)
        {
            Logic.game = game;
            buttons = new Dictionary<string, Button>();
            textBoxes = new Dictionary<string, TextBox>();
            inputBoxes = new Dictionary<string, InputBox>();
            grids = new Dictionary<string, Grid>();

            chatTexture = game.Content.Load<Texture2D>("tekstura");
            chatInputTexture = game.Content.Load<Texture2D>("tekstura2");
            chatSendTexture = game.Content.Load<Texture2D>("tekstura3");
            skurwielTexture = game.Content.Load<Texture2D>("zoltyskurwiel");
            for (int i = 0; i <= 45; i++)
                cardTexture[i] = game.Content.Load<Texture2D>("cards\\card" + i.ToString());

            font = game.Content.Load<SpriteFont>("font");

            map = new PlacedCard[40, 40];
            for (int i = 0; i < 40; i++)
                for (int j = 0; j < 40; j++)
                    map[i, j] = new PlacedCard(0, 0);

            // grids["TESTGRID"] = new Grid(game, chatTexture, chatTexture, 30, 30, new Vector2(75, 75), new Rectangle(140, 100, 1000, 500), 10, BuchnijLolka);

            inputBoxes["ip"] = new InputBox(chatInputTexture, 10, font, new Rectangle(140, 300, 1000, 50), Color.White, Color.LightGray, "Enter server IP");
            inputBoxes["nick"] = new InputBox(chatInputTexture, 10, font, new Rectangle(140, 400, 1000, 50), Color.White, Color.LightGray, "Enter username");
            buttons["connect"] = new Button(chatSendTexture, new Rectangle(140, 500, 1000, 50), LoadGameScreen);
            textBoxes["errorbox"] = new TextBox(chatTexture, 10, Alignment.Centered, font, new Rectangle(140, 650, 1000, 50));
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
            string[] data = message.Split(' ');
            if (data[0] == "CHAT")
                textBoxes["CHAT"].Append(message.Substring(5).Trim());
            if (data[0] == "JOIN")
                textBoxes["CHAT"].Append(message.Substring(5).Trim() + " has joined the chat.");
            if (data[0] == "BYE")
                textBoxes["CHAT"].Append(message.Substring(4).Trim() + " has left the chat.");
            if (data[0] == "ERROR")
            {
                textBoxes["CHAT"].Append(message.Substring(6).Trim());
                game.tcpThread.Abort();
            }
            if (data[0] == "PLACE")
            {
                int x = int.Parse(data[1]);
                int y = int.Parse(data[2]);
                int ID = int.Parse(data[3]);
                int orientation = int.Parse(data[4]);
                map[x, y] = new PlacedCard(ID, orientation);
                grids["BOARD"].fieldTexture[x, y] = cardTexture[ID];
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
                buttons.Clear();
                textBoxes["errorbox"].lines.Clear();
                textBoxes["errorbox"].Append("Successfully connected");
                textBoxes.Clear();

                textBoxes["CHAT"] = new TextBox(chatTexture, 10, Alignment.Left, font, new Rectangle(920, 0, 200, 470));
                inputBoxes["CHATINPUT"] = new InputBox(chatInputTexture, 10, font, new Rectangle(920, 470, 160, 50), Color.White, Color.LightGray, "Enter message...");
                buttons["SEND"] = new Button(chatSendTexture, new Rectangle(1080, 470, 40, 50), SendChatMessage);
                grids["BOARD"] = new Grid(game, chatTexture, cardTexture[0], 30, 30, new Vector2(75, 75), new Rectangle(0, 0, 920, 520), 10, BuchnijLolka);
                grids["CHARACTER"] = new Grid(game, chatTexture, chatTexture, 1, 1, new Vector2(80, 200), new Rectangle(0, 520, 80, 200), 0, BuchnijLolka);
                grids["CARDS"] = new Grid(game, chatTexture, chatTexture, 6, 1, new Vector2(140, 200), new Rectangle(80, 520, 840, 200), 0, BuchnijLolka);
                grids["BUTTONS"] = new Grid(game, chatTexture, chatTexture, 1, 3, new Vector2(200, 64), new Rectangle(920, 520, 200, 200), 1, BuchnijLolka);
                grids["MENU"] = new Grid(game, chatTexture, chatTexture, 3, 1, new Vector2(53, 40), new Rectangle(1120, 0, 160, 40), 1, BuchnijLolka);
                grids["USERS"] = new Grid(game, chatTexture, chatTexture, 1, 1, new Vector2(160, 680), new Rectangle(1120, 40, 160, 680), 0, BuchnijLolka);
            }
            else
            {
                textBoxes["errorbox"].lines.Clear();
                textBoxes["errorbox"].Append("Error, try again");
            }
        }

        public static int CheckCardPlacement(int x, int y, int ID, int rot)
        {
            //1 - karta musi przylegać do innej karty (pamiętać, żeby nie brać karty 45 pod uwagę)
            //2 - tunele wychodzące z karty muszą pasować do sąsiednich kart
            //3 - karta musi być położona na pustym polu
            return 0; //OK
        }



        public static void BuchnijLolka(int x, int y)
        {
            int papiez = 1;
        }
    }
}
