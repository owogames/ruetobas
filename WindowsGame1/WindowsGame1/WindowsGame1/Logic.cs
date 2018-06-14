using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public static class Logic
    {
        public static Random rand = new Random();

        public static Dictionary<string, Button> buttons;
        public static Dictionary<string, TextBox> textBoxes;
        public static Dictionary<string, InputBox> inputBoxes;
        public static Dictionary<string, Grid> grids;

        public static Game game;
        public static Texture2D chatTexture;
        public static Texture2D chatInputTexture;
        public static Texture2D chatSendTexture;
        public static Texture2D skurwielTexture;
        public static Texture2D ReadyTexture;
        public static Texture2D NotReadyTexture;
        public static Texture2D[] cardTexture = new Texture2D[46];
        public static SpriteFont font;

        public static PlacedCard[,] map;

        public static List<Card> cards;

        public static int selectedCard = -1;
        public static int selectedRot = 0;
        public static int[] cardHand = new int[6];
        public static string playerTurn = ""; //Nazwa gracza, który ma teraz turę. Kompletnie nie mam pojęcia jak ją nazwać xD
        //Ogłaszam konkurs. Ten kto wymyśli najlepszą nazwę zmiennej ^ dostaje 1% przychodu

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
            ReadyTexture = game.Content.Load<Texture2D>("ReadyButton");
            NotReadyTexture = game.Content.Load<Texture2D>("NotReadyButton");
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

            if (textBoxes.ContainsKey("PLAYERLIST"))
            {
                TextBox playerTextBox = textBoxes["PLAYERLIST"];
                playerTextBox.lines.Clear();
                for (int i = 0; i < players.Count; i++)
                {
                    string line = players[i].username;
                    if (players[i].username == playerTurn)
                        line += " - currently playing";
                    playerTextBox.Append(line);
                }
            }

            if (game.keyboardState.IsKeyDown(Keys.Enter) && game.keyboardBeforeState.IsKeyUp(Keys.Enter) && inputBoxes.ContainsKey("CHATINPUT") && inputBoxes["CHATINPUT"].active)
                SendChatMessage();
        }

        public static void TCPRecieved(string message)
        {
            Console.WriteLine(message);
            string[] data = message.Split(' ');
            if (data[0] == "CHAT")
                textBoxes["CHAT"].Append(message.Substring(5).Trim());
            if (data[0] == "JOIN")
            {
                textBoxes["CHAT"].Append(message.Substring(5).Trim() + " has joined the game.");
                players.Add(new Player(0, message.Substring(5).Trim()));
                SortPlayers();
            }
            if (data[0] == "BYE")
            {
                textBoxes["CHAT"].Append(message.Substring(4).Trim() + " has left the game.");
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].username == message.Substring(4).Trim())
                    {
                        players.RemoveAt(i);
                        i = players.Count;
                    }
                }
                SortPlayers();
            }
            if (data[0] == "ERROR")
            {
                textBoxes["CHAT"].Append(message.Substring(6).Trim());
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
            if (data[0] == "START")
            {
                buttons.Remove("READY");
                textBoxes["CHAT"].Append("You Are:");
                if (rand.Next(1, 2) == 1)
                    textBoxes["CHAT"].Append("N00b digger");
                else textBoxes["CHAT"].Append("Reutobas bitcher");
                grids["BOARD"] = new Grid(game, chatTexture, cardTexture[0], 17, 13, new Vector2(105, 150), new Rectangle(0, 0, 920, 520), 10, BoardClick, BoardDraw);
                grids["BOARD"].offset = new Vector2(grids["BOARD"].sizeX * grids["BOARD"].fieldSize.X / 2, grids["BOARD"].sizeY * grids["BOARD"].fieldSize.Y / 2);
                map[5, 7] = new PlacedCard(1, 0);
                map[13, 5] = new PlacedCard(45, 0);
                map[13, 7] = new PlacedCard(45, 0);
                map[13, 9] = new PlacedCard(45, 0);
                grids["BOARD"].fieldTexture[5, 7] = cardTexture[1];
                grids["BOARD"].fieldTexture[13, 5] = cardTexture[45];
                grids["BOARD"].fieldTexture[13, 7] = cardTexture[45];
                grids["BOARD"].fieldTexture[13, 9] = cardTexture[45];
                for (int i = 0; i < 6; i++)
                    cardHand[i] = int.Parse(data[i + 1]);
            }
            if (data[0] == "GIB")
            {
                for (int i = 0; i < 6; i++)
                {
                    if (cardHand[i] == 0)
                    {
                        cardHand[i] = int.Parse(data[1]);
                        i = 6;
                    }
                }
            }
            if (data[0] == "TURN")
            {
                playerTurn = data[1].Trim();
            }
            if (data[0] == "OK")
            {
                if(data[1] == "READY")
                {
                    buttons["READY"].texture = ReadyTexture;
                }
            }
        }

        public static void SendChatMessage()
        {
            game.TCPSend("CHAT " + username + ": " + inputBoxes["CHATINPUT"].text);
            inputBoxes["CHATINPUT"].text = "";
        }

        public static void ReadCards()
        {
            cards = new List<Card> { Card.EmptyCard() };
            using (StreamReader sr = new StreamReader("data\\cards.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    Card card = Card.ParseString(line, cards.Count);
                    cards.Add(card);
                }
                sr.Close();
            }
        }

        public static void SortPlayers()
        {
            players.Sort(Player.CompareByName);
            for (int i = 0; i < players.Count; i++)
                players[i].ID = i;
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

                ReadCards();
                textBoxes["CHAT"] = new TextBox(chatTexture, 10, Alignment.Left, font, new Rectangle(920, 0, 200, 470));
                inputBoxes["CHATINPUT"] = new InputBox(chatInputTexture, 10, font, new Rectangle(920, 470, 160, 50), Color.White, Color.LightGray, "Enter message...");
                buttons["SEND"] = new Button(chatSendTexture, new Rectangle(1080, 470, 40, 50), SendChatMessage);
                buttons["READY"] = new Button(NotReadyTexture, new Rectangle(0, 0, 920, 520), Ready); //sam guzik = Rectangle(280, 190, 360, 140)
                grids["CHARACTER"] = new Grid(game, chatTexture, chatTexture, 1, 1, new Vector2(80, 200), new Rectangle(0, 520, 80, 200), 0, null);
                grids["CARDS"] = new Grid(game, chatTexture, chatTexture, 6, 1, new Vector2(140, 200), new Rectangle(80, 520, 840, 200), 0, HandClick, HandDraw);
                grids["BUTTONS"] = new Grid(game, chatTexture, chatTexture, 1, 3, new Vector2(200, 64), new Rectangle(920, 520, 200, 200), 1, null);
                grids["MENU"] = new Grid(game, chatTexture, chatTexture, 3, 1, new Vector2(53, 40), new Rectangle(1120, 0, 160, 40), 1, null);
                textBoxes["PLAYERLIST"] = new TextBox(chatTexture, 1, Alignment.Left, font, new Rectangle(1120, 40, 160, 680));
            }
            else
            {
                textBoxes["errorbox"].lines.Clear();
                textBoxes["errorbox"].Append("Error, try again");
            }
        }

        public static int CheckCardPlacement(int x, int y, int ID, int rot)
        {
            if (cards[map[x, y].ID].cardType != CardType.Empty)
                return 3;
            if (cards[ID].cardType != CardType.Tunnel)
                return 4;

            Vector2[] placements = { new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0)};
            bool any_valid_card = false;
            Tunnel center = (Tunnel)cards[ID];

            for (int i = 0; i < 4; i++)
            {
                PlacedCard current = map[x + (int)placements[i].X, y + (int)placements[i].Y];
                if (current.ID == 45 || current.ID == 0)
                    continue;
                else
                {
                    any_valid_card = true;
                    Tunnel currentTL = (Tunnel)cards[current.ID];
                    if(center.GetEntrance(i + rot * 2) != currentTL.GetEntrance(i + (current.rotation - 1) * 2))
                    {
                        return 2;
                    }
                }
            }
            //    0       0    
            //  3 C 1 - 3 T 1  //no rotation
            //    2       2    

            if (any_valid_card == false)
                return 1;
            return 0; 
            //0 - OK
            //1 - karta musi przylegać do innej karty (pamiętać, żeby nie brać karty 45 pod uwagę)
            //2 - tunele wychodzące z karty muszą pasować do sąsiednich kart
            //3 - karta musi być położona na pustym polu
            //4 - karta musi tunelem
        }


        public static void BoardDraw(SpriteBatch spriteBatch, Rectangle location, int x, int y)
        {
            float rot = map[x, y].rotation == 1 ? (float)Math.PI : 0.0f;
            if (map[x, y].rotation == 1)
            {
                location.X += location.Width;
                location.Y += location.Height;
            }
            spriteBatch.Draw(grids["BOARD"].fieldTexture[x, y], location, null, Color.White, rot, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void HandDraw(SpriteBatch spriteBatch, Rectangle location, int x, int y)
        {
            Color c = (selectedCard == x) ? Color.LightYellow : Color.White;
            spriteBatch.Draw(cards[cardHand[x]].texture, location, c);
        }


        public static void HandClick(int x, int y)
        {
            selectedRot = 0;
            if (x == selectedCard && cards[cardHand[x]].cardType == CardType.Tunnel)
                selectedRot = 1;
            selectedCard = x;
        }

        public static void BoardClick(int x, int y)
        {
            if (selectedCard == -1)
                return;
            if (playerTurn != username)
                return;
            int result = CheckCardPlacement(x, y, cardHand[selectedCard], selectedRot);
            if (result == 0)
            {
                for (int i = selectedCard; i < 5; i++)
                    cardHand[i] = cardHand[i + 1];
                selectedCard = -1;
                cardHand[5] = 0;
                selectedRot = 0;
                game.TCPSend("PLACE " + x.ToString() + " " + y.ToString() + " " + cardHand[selectedCard].ToString() + " " + selectedRot.ToString());
            }
        }


        public static void BuchnijLolka(int x, int y)
        {
            map[x, y].ID = rand.Next(1, 44);
            grids["BOARD"].fieldTexture[x, y] = cards[map[x, y].ID].texture;
        }

        public static void Ready()
        {
            game.TCPSend("READY");           
        }
    }
}
