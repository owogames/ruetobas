using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
        public static Texture2D errorBackground;
        public static Texture2D errorWindow;
        public static Texture2D errorButton;
        public static Texture2D discardTexture;
        public static Texture2D emptyTextbox;
        public static Texture2D readyTexture;
        public static Texture2D notReadyTexture;
        public static Texture2D semiTransparentTexture;
        public static Texture2D transparentTexture;
        public static Texture2D settingsTexture;
        public static Texture2D unTickedTexture;
        public static Texture2D tickedTexture;
        public static Texture2D[] buffTexture = new Texture2D[3];
        public static Texture2D[] cardTexture = new Texture2D[73];
        public static Texture2D[] mapCardTexture = new Texture2D[3];
        public static SpriteFont font;

        public static float volume = 1.0f;
        public static SoundEffect bubbles;
        public static SoundEffect boop;
        public static SoundEffect bye;
        public static SoundEffect stal;

        public static DisplayMode[] displayModes;
        public static Point maxResDefault;
        public static bool only16to9 = true;

        public static PlacedCard[,] map;

        public static List<Card> cards;

        public static string[] windowNames = { "A tunnel placing card game.", "Boi I love this game.", "Let us all block Paweł.", "Just please don't destroy this tunnel!", "Made with love.", "Gosh I hate geologists!", "", "Ruetobas: Ruetobas: Ruetobas: Ruetobas: Ruetobas: Ruetobas...", "Now with Battle Royale!!" };
        public static int selectedCard = -1;
        public static int selectedRot = 0;
        public static int[] cardHand = new int[6];
        public static string playerTurn = ""; //Nazwa gracza, który ma teraz turę. Kompletnie nie mam pojęcia jak ją nazwać xD
        //Ogłaszam konkurs. Ten kto wymyśli najlepszą nazwę zmiennej ^ dostaje 1% przychodu

        public const int port = 2137;

        public static string IP = "192.168.0.157";
        public static string username = "No Elo";
        public static int yourPlayerId = -1;

        public static List<Player> players = new List<Player>();

        public static void Init(Game game)
        {
            Logic.game = game;

            game.Window.Title = "Ruetobas: " + windowNames[rand.Next(windowNames.Length)];
            maxResDefault = game.GetCurrentDeviceResolution();
            displayModes = game.GetDisplayModes();
            Array.Sort(displayModes, SortDisplay);
            foreach(DisplayMode dp in displayModes)
            {
                Console.WriteLine("Dp = {0} : {1}, aspekt = {2}", dp.Width, dp.Height, dp.AspectRatio);
            }
            
            buttons = new Dictionary<string, Button>();
            textBoxes = new Dictionary<string, TextBox>();
            inputBoxes = new Dictionary<string, InputBox>();
            grids = new Dictionary<string, Grid>();

            errorBackground = game.Content.Load<Texture2D>("errorBackground");
            errorWindow = game.Content.Load<Texture2D>("tekstura");
            emptyTextbox = game.Content.Load<Texture2D>("empty");
            errorButton = game.Content.Load<Texture2D>("errorButton");
            unTickedTexture = game.Content.Load<Texture2D>("CheckBoxUnTicked");
            tickedTexture = game.Content.Load<Texture2D>("CheckBoxTicked");
            chatTexture = game.Content.Load<Texture2D>("tekstura");
            chatInputTexture = game.Content.Load<Texture2D>("tekstura2");
            chatSendTexture = game.Content.Load<Texture2D>("tekstura3");
            skurwielTexture = game.Content.Load<Texture2D>("zoltyskurwiel");
            readyTexture = game.Content.Load<Texture2D>("ReadyButton");
            notReadyTexture = game.Content.Load<Texture2D>("NotReadyButton");
            semiTransparentTexture = game.Content.Load<Texture2D>("SemiTransparent");
            transparentTexture = game.Content.Load<Texture2D>("Transparent");
            settingsTexture = game.Content.Load<Texture2D>("SettingsButton");
            bubbles = game.Content.Load<SoundEffect>("SoundFX/menuback2");
            boop = game.Content.Load<SoundEffect>("SoundFX/normal-hitwhistle");
            bye = game.Content.Load<SoundEffect>("SoundFX/seeya");
            for (int i = 0; i <= 72; i++)
                cardTexture[i] = game.Content.Load<Texture2D>("cards\\card" + i.ToString());
            buffTexture[0] = game.Content.Load<Texture2D>("buffpickaxe");
            buffTexture[1] = game.Content.Load<Texture2D>("bufflantern");
            buffTexture[2] = game.Content.Load<Texture2D>("buffcart");
            for (int i = 0; i < 3; i++)
                mapCardTexture[i] = game.Content.Load<Texture2D>("cards\\mapcard" + (i + 42).ToString());
            discardTexture = game.Content.Load<Texture2D>("buttondiscard");

            font = game.Content.Load<SpriteFont>("comic");

            map = new PlacedCard[19, 15];
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 15; j++)
                    map[i, j] = new PlacedCard(0, 0);

            // grids["TESTGRID"] = new Grid(game, chatTexture, chatTexture, 30, 30, new Vector2(75, 75), new Rectangle(140, 100, 1000, 500), 10, BuchnijLolka);

            inputBoxes["ip"] = new InputBox(chatInputTexture, 10, font, new Rectangle(210, 450, 1500, 75), Color.White, Color.LightGray, "Enter server IP");
            inputBoxes["nick"] = new InputBox(chatInputTexture, 10, font, new Rectangle(210, 600, 1500, 75), Color.White, Color.LightGray, "Enter username", 32);
            buttons["connect"] = new Button(chatSendTexture, new Rectangle(210, 750, 1500, 75), LoadGameScreen);
            buttons["menubutton"] = new Button(settingsTexture, new Rectangle(10, 10, 40, 40), OpenGameMenu);
        }
        
        public static void Update()
        {
            //Wychodzenie z gry
            if (game.keyboardState.IsKeyDown(Keys.Escape) && game.keyboardBeforeState.IsKeyUp(Keys.Escape) && game.IsActive)
            {
                if (game.tcpThread != null)
                {
                    game.TCPSend("QUIT");
                    game.tcpThread.Abort();
                }
                game.Exit();
            }

            //Żeby Playerlist się za bardzo nie przesuwał
            if (grids.ContainsKey("ZPLAYERLIST"))
            {
                grids["ZPLAYERLIST"].offset.Y = Math.Max(grids["ZPLAYERLIST"].offset.Y, grids["ZPLAYERLIST"].location.Height / 2 - grids["ZPLAYERLIST"].margin);
                grids["ZPLAYERLIST"].offset.X = grids["ZPLAYERLIST"].location.Width / 2 - grids["ZPLAYERLIST"].margin;
                grids["ZPLAYERLIST"].zoom = 1.0f;
            }

            //Żeby karty się nie przesuwały
            if (grids.ContainsKey("CARDS"))
            {
                grids["CARDS"].zoom = 1.0f;
                grids["CARDS"].offset = new Vector2(grids["CARDS"].location.Width / 2 - grids["CARDS"].margin, grids["CARDS"].location.Height / 2 - grids["CARDS"].margin);
            }

            //Ikonki buffów
            if (yourPlayerId >= 0)
            {
                for (int i = 0; i < 8; i++)
                    if (buttons.ContainsKey("ZBUFFICON" + i.ToString()))
                        buttons.Remove("ZBUFFICON" + i.ToString());
                for (int i = 0; i < players[yourPlayerId].buffs.Count; i++)
                {
                    buttons["ZBUFFICON" + i.ToString()] = new Button(buffTexture[(int)players[yourPlayerId].buffs[i] - 1], new Rectangle(0, 78 * i, 72, 72), null);
                    buttons["ZBUFFICON" + i.ToString()].registerClicks = false;
                }
            }

            if (game.IsActive && game.keyboardState.IsKeyDown(Keys.Enter) && game.keyboardBeforeState.IsKeyUp(Keys.Enter) && inputBoxes.ContainsKey("CHATINPUT") && inputBoxes["CHATINPUT"].active)
                SendChatMessage();
        }

        public static void TCPRecieved(string message)
        {
            string[] submessages = message.Split('\n');
            foreach (string sub in submessages)
            {
                Console.WriteLine(sub);
                string[] data = sub.Split(' ');
                if (data[0] == "CHAT")
                    textBoxes["CHAT"].AppendAndWrap(sub.Substring(5).Trim());
                if (data[0] == "JOIN")
                {
                    textBoxes["CHAT"].AppendAndWrap(sub.Substring(5).Trim() + " has joined the game.");
                    players.Add(new Player(0, sub.Substring(5).Trim()));
                    SortPlayers();
                }
                if (data[0] == "BYE")
                {
                    PlaySound(bye, volume);
                    textBoxes["CHAT"].AppendAndWrap(sub.Substring(4).Trim() + " has left the game.");
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].username == sub.Substring(4).Trim())
                        {
                            players.RemoveAt(i);
                            i = players.Count;
                        }
                    }
                    SortPlayers();
                }
                if (data[0] == "ERROR")
                {
                    AnnounceError(sub.Substring(6).Trim());
                }
                if (data[0] == "PLACE")
                {
                    int ID = int.Parse(data[1]);
                    int x = int.Parse(data[2]);
                    int y = int.Parse(data[3]);
                    int orientation = int.Parse(data[4]);
                    map[x, y] = new PlacedCard(ID, orientation);
                    grids["BOARD"].fieldTexture[x, y] = cardTexture[ID];
                }
                if (data[0] == "MAP")
                {
                    int ID = int.Parse(data[1]) - 42;
                    int x = int.Parse(data[2]);
                    int y = int.Parse(data[3]);
                    grids["BOARD"].fieldTexture[x, y] = mapCardTexture[ID];
                }
                if (data[0] == "START")
                {
                    for (int i = 0; i < 19; i++)
                        for (int j = 0; j < 15; j++)
                            map[i, j] = new PlacedCard(0, 0);
                    map[5, 7] = new PlacedCard(1, 0);
                    map[13, 5] = new PlacedCard(45, 0);
                    map[13, 7] = new PlacedCard(45, 0);
                    map[13, 9] = new PlacedCard(45, 0);

                    for (int i = 1; i < 18; i++)
                        for (int j = 1; j < 14; j++)
                            grids["BOARD"].fieldTexture[i, j] = cardTexture[0];
                    grids["BOARD"].fieldTexture[5, 7] = cardTexture[1];
                    grids["BOARD"].fieldTexture[13, 5] = cardTexture[45];
                    grids["BOARD"].fieldTexture[13, 7] = cardTexture[45];
                    grids["BOARD"].fieldTexture[13, 9] = cardTexture[45];
                    grids["BOARD"].enabled = true;
                    buttons["READY"].enabled = false;
                    for (int i = 0; i < data.Count() - 2; i++)
                        cardHand[i] = int.Parse(data[i + 1]);

                    for (int i = 0; i < players.Count; i++)
                        players[i].playerClass = PlayerClass.Unknown;
                    for (int i = 0; i < players.Count; i++)
                        players[i].buffs.Clear();

                    players[yourPlayerId].playerClass = (PlayerClass)int.Parse(data[data.Count() - 1]);
                    textBoxes["CHAT"].Append("You Are:");
                    textBoxes["CHAT"].Append(players[yourPlayerId].playerClass.ToString());
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
                    if (playerTurn == username)
                    {
                        textBoxes["HELP"].lines[0] = "Select card from your hand";
                        PlaySound(bubbles, volume);
                    }
                    else
                    {
                        PlaySound(boop, volume * 0.3f);
                    }
                        
                }
                if (data[0] == "END")
                {
                    textBoxes["CHAT"].Append("Team " + ((PlayerClass)int.Parse(data[1])).ToString() + " wins!");
                    textBoxes["HELP"].lines[0] = "Team " + ((PlayerClass)int.Parse(data[1])).ToString() + " wins!";
                    for (int i = 0; i < players.Count; i++)
                    {
                        string playername = data[3 * i + 2];
                        for (int j = 0; j < players.Count; j++)
                            if (players[j].username == playername)
                            {
                                players[j].score = int.Parse(data[3 * i + 3]);
                                players[j].playerClass = (PlayerClass)int.Parse(data[3 * i + 4]);
                            }
                    }
                    grids["BOARD"].enabled = false;
                    buttons["READY"].enabled = true;
                    buttons["READY"].texture = notReadyTexture;
                }
                if (data[0] == "BUFF")
                {
                    Player.FindByName(data[2].Trim()).AddBuff((Buff)int.Parse(data[3]));
                }
                if (data[0] == "DEBUFF")
                {
                    Player.FindByName(data[2].Trim()).RemoveBuff((Buff)int.Parse(data[3]));
                }
                if (data[0] == "OK")
                {
                    if (data[1] == "READY")
                    {
                        buttons["READY"].texture = readyTexture;
                    }
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
            cards = new List<Card> { };
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
            cards[0].cardType = CardType.Empty;
        }

        public static void SortPlayers()
        {
            players.Sort(Player.CompareByName);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].ID = i;
                if (players[i].username == username)
                    yourPlayerId = i;
            }
        }

        public static void LoadGameScreen()
        {
            IP = inputBoxes["ip"].text;
            username = inputBoxes["nick"].text;

            if (game.TCPConnect(IP, port))
            {
                inputBoxes.Clear();
                buttons.Clear();
                textBoxes.Clear();

                ReadCards();
                textBoxes["CHAT"] = new TextBox(chatTexture, 10, Alignment.Left, font, new Rectangle(1380, 50, 540, 670), "You joined the game");
                inputBoxes["CHATINPUT"] = new InputBox(chatInputTexture, 10, font, new Rectangle(1380, 720, 490, 60), Color.White, Color.LightGray, "Enter message...", 120);
                buttons["SEND"] = new Button(chatSendTexture, new Rectangle(1870, 720, 50, 60), SendChatMessage);
                textBoxes["HELP"] = new TextBox(errorBackground, 5, Alignment.Centered, font, new Rectangle(0, 720, 1380, 60), "");
                textBoxes["HELP"].canScroll = false;
                //tekstury dla HD - ["READY"] = new Button(notReadyTexture, new Rectangle(280, 190, 360, 140), Ready); //sam guzik = Rectangle(280, 190, 360, 140) guzik z tlem = new Rectangle(0, 0, 1380, 780)
                buttons["READY"] = new Button(notReadyTexture, new Rectangle(420, 285, 540, 210), Ready);
                buttons["CHARACTER"] = new Button(chatTexture, new Rectangle(0, 780, 180, 300), null);
                textBoxes["ACTUALPLAYER"] = new TextBox(errorButton, 5, Alignment.Left, font, new Rectangle(1380, 0, 290, 50));
                buttons["PLAYERLISTSET"] = new Button(errorButton, new Rectangle(1670, 0, 250, 50), ShowPlayerList);
                buttons["DISCARD"] = new Button(discardTexture, new Rectangle(1380, 780, 540, 75), DiscardCard);
                buttons["REMOVE"] = new Button(errorButton, new Rectangle(1380, 855, 540, 75), null);
                buttons["MENU"] = new Button(settingsTexture, new Rectangle(1380, 930, 540, 75), OpenGameMenu);
                buttons["EXIT"] = new Button(errorButton, new Rectangle(1380, 1005, 540, 75), null);
                grids["CARDS"] = new Grid(game, chatTexture, chatTexture, 6, 1, new Vector2(200, 300), new Rectangle(180, 780, 1200, 300), 0, HandClick, HandDraw);
                grids["BOARD"] = new Grid(game, chatTexture, chatTexture, 19, 15, new Vector2(100, 150), new Rectangle(0, 0, 1380, 720), 10, BoardClick, BoardDraw);
                grids["BOARD"].enabled = false;
            }
            else
            {
                AnnounceError("Connection error, try again");
            }
        }

        public static void ShowPlayerList()
        {
            buttons["PLAYERLISTSET"] = new Button(chatSendTexture, new Rectangle(1670, 0, 250, 50), HidePlayerList);
            grids["ZPLAYERLIST"] = new Grid(game, chatTexture, chatTexture, 1, 10, new Vector2(250, 150), new Rectangle(1670, 50, 250, 1030), 1, PlayerListClick, PlayerListDraw);
            grids["ZPLAYERLIST"].offset = new Vector2(grids["ZPLAYERLIST"].location.Width / 2 - grids["ZPLAYERLIST"].margin, grids["ZPLAYERLIST"].location.Height / 2 - grids["ZPLAYERLIST"].margin);
        }

        public static void HidePlayerList()
        {
            buttons["PLAYERLISTSET"] = new Button(errorButton, new Rectangle(1670, 0, 250, 50), ShowPlayerList);
            grids.Remove("ZPLAYERLIST");
        }

        public static int CheckCardPlacement(int x, int y, int ID, int rot)
        {
            if (cards[ID].cardType != CardType.Tunnel)
                return 4;
            if (x < 1 || x > 17 || y < 1 || y > 13)
                return -1;
            if (cards[map[x, y].ID].cardType != CardType.Empty)
                return 3;

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
                    
                    Tunnel currentTL = (Tunnel)cards[current.ID];
                    if(center.GetEntrance(i + rot * 2) != currentTL.GetEntrance(i + (current.rotation - 1) * 2))
                    {
                        return 2;
                    }
                    else if(center.GetEntrance(i + rot * 2))
                        any_valid_card = true;
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

        public static void DiscardCard()
        {
            if (playerTurn != username)
            {
                textBoxes["HELP"].lines[0] = "You can only play cards during your turn";
                return;
            }

            if (selectedCard == -1 || cardHand[selectedCard] == 0)
            {
                textBoxes["HELP"].lines[0] = "Select a card you would like to discard first";
                return;
            }

            int id = cardHand[selectedCard];
            RemoveSelectedCard();
            game.TCPSend("DISCARD " + id.ToString());
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
            if (selectedRot == 0 || x != selectedCard)
                spriteBatch.Draw(cards[cardHand[x]].texture, location, c);
            else
            {
                location.X += location.Width;
                location.Y += location.Height;
                spriteBatch.Draw(cards[cardHand[x]].texture, location, null, c, (float)Math.PI, Vector2.Zero, SpriteEffects.None, 0);
            }
        }

        public static void PlayerListDraw(SpriteBatch spriteBatch, Rectangle location, int x, int y)
        {
            if (y >= players.Count)
                return;

            Color c = playerTurn == players[y].username ? Color.LightYellow : Color.White;
            spriteBatch.Draw(chatTexture, location, c);
            spriteBatch.DrawString(font, players[y].username, new Vector2(location.X + 5, location.Y + 5), Color.White);
            spriteBatch.DrawString(font, "Score: " + players[y].score.ToString(), new Vector2(location.X + 5, location.Y + 5 + font.LineSpacing), Color.White);
            spriteBatch.DrawString(font, "Role: " + players[y].playerClass.ToString(), new Vector2(location.X + 5, location.Y + 5 + 2 * font.LineSpacing), Color.White);

            int buffCount = 0;
            foreach (Buff buff in players[y].buffs)
            {
                spriteBatch.Draw(buffTexture[(int)buff - 1], new Rectangle(location.X + 10 + 48 * buffCount, location.Y + 5 + 3 * font.LineSpacing, 48, 48), Color.White);
                buffCount++;
            }
        }


        public static void HandClick(int x, int y)
        {
            if (x == selectedCard && (cards[cardHand[x]].cardType == CardType.Tunnel ||
                (cards[cardHand[x]].cardType == CardType.Debuff && ((DebuffCard)cards[cardHand[x]]).buffType2 != Buff.None)))
            {
                selectedRot = 1 - selectedRot;
            }
            else
            {
                selectedRot = 0;
                selectedCard = x;

                CardType cardType = cards[cardHand[selectedCard]].cardType;
                if (cardType == CardType.Tunnel)
                    textBoxes["HELP"].lines[0] = "Place card on board or rotate it by clicking on it again";
                else if (cardType == CardType.Map)
                    textBoxes["HELP"].lines[0] = "Select treasure card you would like to uncover";
                else if (cardType == CardType.Remove)
                    textBoxes["HELP"].lines[0] = "Select placed tunnel you would like to demolish";
                else if (cardType == CardType.Buff)
                    textBoxes["HELP"].lines[0] = "Select player from player list to inflict this effect on him";
                else if (cardType == CardType.Debuff)
                    textBoxes["HELP"].lines[0] = "Select player from player list to remove this effect from him";
            }
        }

        public static void BoardClick(int x, int y)
        {
            if (selectedCard == -1)
                return;

            if (playerTurn != username)
            {
                textBoxes["HELP"].lines[0] = "You can only play cards during your turn";
                return;
            }

            if (cards[cardHand[selectedCard]].cardType == CardType.Tunnel)
            {
                if (players[yourPlayerId].buffs.Contains(Buff.Cart))
                {
                    textBoxes["HELP"].lines[0] = "You cannot play tunnel cards unless you repair your cart";
                    return;
                }

                if (players[yourPlayerId].buffs.Contains(Buff.Lantern))
                {
                    textBoxes["HELP"].lines[0] = "You cannot play tunnel cards unless you repair your lantern";
                    return;
                }

                if (players[yourPlayerId].buffs.Contains(Buff.Pickaxe))
                {
                    textBoxes["HELP"].lines[0] = "You cannot play tunnel cards unless you repair your pickaxe";
                    return;
                }

                int result = CheckCardPlacement(x, y, cardHand[selectedCard], selectedRot);
                if (result == 0)
                {
                    string line = "PLACE " + cardHand[selectedCard].ToString() + " " + x.ToString() + " " + y.ToString() + " " + selectedRot.ToString();
                    RemoveSelectedCard();
                    game.TCPSend(line);
                }
                else if (result == 1)
                {
                    textBoxes["HELP"].lines[0] = "You can only place card next to other card";
                }
                else if (result == 2)
                {
                    textBoxes["HELP"].lines[0] = "Played card must match to its neighbours";
                }
                else if (result == 3)
                {
                    textBoxes["HELP"].lines[0] = "You cannot place card on occupied spot";
                }
                else if (result == 4)
                {
                    textBoxes["HELP"].lines[0] = "You can only place tunnel cards";
                }
            }
            else if (cards[cardHand[selectedCard]].cardType == CardType.Remove)
            {
                if (cards[map[x, y].ID].cardType == CardType.Tunnel)
                {
                    if (map[x, y].ID == 1)
                    {
                        textBoxes["HELP"].lines[0] = "You can't demolish starting point";
                    }
                    else if (map[x, y].ID == 42 || map[x, y].ID == 43 || map[x, y].ID == 44 || map[x, y].ID == 45)
                    {
                        textBoxes["HELP"].lines[0] = "You can't demolish treasure card";
                    }
                    else
                    {
                        string line = "USE " + cardHand[selectedCard].ToString() + " " + x.ToString() + " " + y.ToString();
                        RemoveSelectedCard();
                        game.TCPSend(line);
                    }
                }
                else
                {
                    textBoxes["HELP"].lines[0] = "This card must be used on placed tunnel";
                }
            }
            else if (cards[cardHand[selectedCard]].cardType == CardType.Map)
            {
                if (map[x, y].ID == 45)
                {
                    string line = "USE " + cardHand[selectedCard].ToString() + " " + x.ToString() + " " + y.ToString();
                    RemoveSelectedCard();
                    game.TCPSend(line);
                }
                else
                {
                    textBoxes["HELP"].lines[0] = "This card must be used on uncovered treasure card";
                }
            }
        }

        public static void PlayerListClick(int x, int y)
        {
            if (selectedCard == -1)
                return;

            if (playerTurn != username)
            {
                textBoxes["HELP"].lines[0] = "You can only play cards during your turn";
                return;
            }

            int id = cardHand[selectedCard];
            if (cards[id].cardType == CardType.Buff)
            {
                if (players[y].buffs.Contains(((BuffCard)cards[id]).buffType))
                    textBoxes["HELP"].lines[0] = "This player already has this effect applied";
                else
                {
                    string line = "USE " + id.ToString() + " " + players[y].username + " 0";
                    RemoveSelectedCard();
                    game.TCPSend(line);
                }
            }

            if (cards[id].cardType == CardType.Debuff)
            {
                Buff buff = ((DebuffCard)cards[id]).buffType;
                if (selectedRot == 1 && ((DebuffCard)cards[id]).buffType2 != Buff.None)
                    buff = ((DebuffCard)cards[id]).buffType2;

                if (!players[y].buffs.Contains(buff))
                    textBoxes["HELP"].lines[0] = "This player doesn't have this effect applied";
                else
                {
                    string line = "USE " + id.ToString() + " " + players[y].username + " " + selectedRot.ToString();
                    RemoveSelectedCard();
                    game.TCPSend(line);
                }
            }
        }


        public static void AnnounceError(string error)
        {
            buttons["ZZZBACKGROUND"] = new Button(errorBackground, new Rectangle(0, 0, 1920, 1080), null);
            textBoxes["ZZZERRORBOX"] = new TextBox(emptyTextbox, 15, Alignment.Centered, font, new Rectangle(560, 375, 800, 220), error);
            buttons["ZZZBUTTON"] = new Button(errorWindow, new Rectangle(560, 300, 800, 330), null);
            buttons["ZZZZBUTTON"] = new Button(errorButton, new Rectangle(885, 550, 150, 50), CloseError);
        }

        public static void RemoveSelectedCard()
        {
            for (int i = selectedCard; i < 5; i++)
                cardHand[i] = cardHand[i + 1];
            selectedCard = -1;
            cardHand[5] = 0;
            selectedRot = 0;
        }

        public static void CloseError()
        {
            buttons.Remove("ZZZBACKGROUND");
            buttons.Remove("ZZZBUTTON");
            buttons.Remove("ZZZZBUTTON");
            textBoxes.Remove("ZZZERRORBOX");
        }

        public static void Ready()
        {
            game.TCPSend("READY");          
        }

        public static void OpenGameMenu()
        {
            buttons["ZZZBackground"] = new Button(semiTransparentTexture, new Rectangle(0, 0, 1920, 1080), null);
            textBoxes["ZZZZFullscreentext"] = new TextBox(chatInputTexture, 8, Alignment.Centered, font, new Rectangle(10, 10, 200, 50), "Fullscreen");
            textBoxes["ZZZZ16to9text"] = new TextBox(chatInputTexture, 8, Alignment.Centered, font, new Rectangle(10, 120, 200, 50), "Allow only 16:9 res");
            buttons["ZZZZFullscreen"] = new Button(tickedTexture, new Rectangle(220, 10, 50, 50), ChangeFullscreen);
            buttons["ZZZZ16to9"] = new Button(tickedTexture, new Rectangle(220, 120, 50, 50), Change16to9Mode);
            if (Game.isFullscreen == false)
                buttons["ZZZZFullscreen"].texture = unTickedTexture;
            //textBoxes["ZZZZFullscreentext"] = new TextBox(chatInputTexture, 8, Alignment.Left, font, new Rectangle(10, 230, 110, 40));
            //textBoxes["ZZZZFullscreentext"].Append("Fullscreen");
            inputBoxes["ZZZZVolume"] = new InputBox(chatInputTexture, 8, font, new Rectangle(10, 230, 200, 100), Color.Aquamarine, Color.BlueViolet, "Volume", 3);
            buttons["ZZZZTestSound"] = new Button(errorButton, new Rectangle(220, 230, 50, 50), () => PlaySound(bubbles, volume));
            buttons["ZZZZdone"] = new Button(readyTexture, new Rectangle(10, 345, 140, 80), CloseGameMenu);
            buttons["ZZZZQuit"] = new Button(errorButton, new Rectangle(1700, 940, 140, 80), game.Exit);
            int height = 20;
            foreach (DisplayMode dp in displayModes)
            {
                if (only16to9 == false || dp.AspectRatio == (16.0f / 9.0f))
                {
                    string name = "ZZZZ" + dp.Width.ToString() + ":" + dp.Height.ToString();
                    buttons["Z" + name] = new Button(transparentTexture, new Rectangle(1000, height, 200, 50), () => game.ChangeResolution(dp.Width, dp.Height));
                    textBoxes[name] = new TextBox(chatInputTexture, 8, Alignment.Centered, font, new Rectangle(1000, height, 200, 50));
                    textBoxes[name].Append(dp.Width.ToString() + ":" + dp.Height.ToString());
                    height += 60;
                }
            }
        }

        public static void ChangeFullscreen()
        {
            game.ChangeFullscreen();
            if (Game.isFullscreen)
                buttons["ZZZZFullscreen"].texture = tickedTexture;
            else
                buttons["ZZZZFullscreen"].texture = unTickedTexture;
        }

        public static void CloseGameMenu()
        {
            //int newX, newY;
            float new_volume;
            /*
            if (int.TryParse(inputBoxes["ZZZZResolutionX"].text, out newX) &&
                int.TryParse(inputBoxes["ZZZZResolutionY"].text, out newY) && 
                newX > 0 && newY > 0)
                game.ChangeResolution(newX, newY);
            */
            if (float.TryParse(inputBoxes["ZZZZVolume"].text, out new_volume) &&
                new_volume < 100 && new_volume >= 0)
                volume = new_volume * 0.01f;
            textBoxes.Remove("ZZZZFullscreentext");
            textBoxes.Remove("ZZZZ16to9text");
            buttons.Remove("ZZZBackground");
            buttons.Remove("ZZZZdone");
            buttons.Remove("ZZZZFullscreen");
            buttons.Remove("ZZZZ16to9");
            inputBoxes.Remove("ZZZZVolume");
            buttons.Remove("ZZZZTestSound");
            foreach (DisplayMode dp in displayModes)
            {
                string name = "ZZZZ" + dp.Width.ToString() + ":" + dp.Height.ToString();
                if (only16to9 == false || dp.AspectRatio == (16.0f / 9.0f))
                {
                    buttons.Remove("Z" + name);
                    textBoxes.Remove(name);
                }
                    
            }
            //textBoxes.Remove("ZZZZFullscreentext");
            buttons.Remove("ZZZZQuit");
            return;
        }

        public static void PlaySound(SoundEffect soundEffect, float playVolume)
        {
            soundEffect.Play(playVolume, 0.0f, 0.0f);
        }

        public static int SortDisplay(DisplayMode a, DisplayMode b)
        {
            if(a.Width > maxResDefault.X || a.Height > maxResDefault.Y)
            {
                if(b.Width > maxResDefault.X || b.Height > maxResDefault.Y)
                {
                    if (a.Width > b.Width)
                        return -1;
                    else if (a.Width < b.Width)
                        return 1;
                    else
                    {
                        if (a.Height > b.Height)
                            return -1;
                        else if (a.Height < b.Height)
                            return 1;
                        else
                            return 0;
                    }
                }
                else
                {
                    return 1;
                }
            }
            else if(b.Width > maxResDefault.X || b.Height > maxResDefault.Y){
                return -1;
            }
            else
            {
                if (a.Width > b.Width)
                    return -1;
                else if (a.Width < b.Width)
                    return 1;
                else
                {
                    if (a.Height > b.Height)
                        return -1;
                    else if (a.Height < b.Height)
                        return 1;
                    else
                        return 0;
                }
            }
        }
        
        public static void Change16to9Mode()
        {

        }

    }
}
