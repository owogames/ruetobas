﻿using Microsoft.Xna.Framework;
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

        public static SortedDictionary<string, Button> buttons;
        public static SortedDictionary<string, TextBox> textBoxes;
        public static SortedDictionary<string, InputBox> inputBoxes;
        public static SortedDictionary<string, Grid> grids;
        public static SortedDictionary<string, RawImage> images;
        public static List<Animation> animations;

        public static Game game;
        public static SpriteBatch spriteBatch;
        public static Texture2D logo;
        public static Texture2D chatTexture, boardTexture;
        public static Texture2D chatInputTexture;
        public static Texture2D chatSendTexture;
        public static Texture2D skurwielTexture;
        public static Texture2D errorBackground;
        public static Texture2D errorWindow;
        public static Texture2D errorButton;
        public static Texture2D emptyTextbox;
        public static Texture2D readyTexture;
        public static Texture2D notReadyTexture;
        public static Texture2D semiTransparentTexture;
        public static Texture2D transparentTexture;
        public static Texture2D solidTexture, solidBlack, solidGray;
        public static Texture2D settingsTexture;
        public static Texture2D unTickedTexture;
        public static Texture2D tickedTexture;
        public static Texture2D noButton;
        public static Texture2D yesButton;
        public static Texture2D menuBackground;
        public static Texture2D optionsWindow, windowTexture, connectWindow;
        public static Texture2D inputboxTexture, inputboxSmallTexture, inputboxLongTexture;
        public static Texture2D dropdownTexture;
        public static Texture2D dropdownMenuTexture;
        public static Texture2D fullscreenTexture, resolutionTexture, volumeTexture, doneTexture, exitTexture, joinTexture, optionsTexture, quitTexture, connectTexture, cancelTexture;
        public static Texture2D[] buffTexture = new Texture2D[3];
        public static Texture2D[] cardTexture = new Texture2D[73];
        public static Texture2D[] mapCardTexture = new Texture2D[3];
        public static Texture2D tileGrass, tileDirt, tileGrid;
        public static Texture2D[] tileTunnel = new Texture2D[46];
        public static Texture2D gameChatTexture, gameBackgroundTexture;
        public static SpriteFont font, guifont;
        public static Effect maskEffect;

        public static SoundEffectInstance menuMusicInstance;
        public static SoundEffect menuMusic;
        public static SoundEffect bubbles;
        public static SoundEffect boop;
        public static SoundEffect bye;
        public static SoundEffect stal;

        public static DisplayMode[] displayModes;
        public static Point maxResDefault;
        public static bool onlyNativeRes = true;

        public static PlacedCard[,] map;

        public static List<Card> cards;

        public static string[] windowNames = {
            "A tunnel placing card game.",
            "Boi I love this game.",
            "Let us all block Paweł.",
            "Just please don't destroy this tunnel!",
            "Made with love.",
            "Gosh I hate geologists!",
            "",
            "Ruetobas: Ruetobas: Ruetobas: Ruetobas: Ruetobas: Ruetobas...",
            "Now with Battle Royale!!"
        };
        public static string optionsNamespace = "ZZZZOPTIONS"; // Taki prefix mają elementy z menu.
        public static string menuNamespace = "MENU";
        public static string gameNamespace = "GAME";

        public static int selectedCard = -1;
        public static int selectedRot = 0;
        public static int[] cardHand = new int[6];
        public static string playerTurn = ""; //Nazwa gracza, który ma teraz turę. Kompletnie nie mam pojęcia jak ją nazwać xD
        //Ogłaszam konkurs. Ten kto wymyśli najlepszą nazwę zmiennej ^ dostaje 1% przychodu

        public const int port = 2137;

        public static string IP = "192.168.0.157";
        public static string username = "No Elo";
        public static int yourPlayerId = -1;
        public static bool adminMode = false;
        public static bool gameInProgress = false;

        public static List<Player> players = new List<Player>();

        public static void Init(Game game)
        {
            Logic.game = game;

            game.Window.Title = "Ruetobas: " + windowNames[rand.Next(windowNames.Length)];
            maxResDefault = game.GetCurrentDeviceResolution();
            displayModes = game.GetDisplayModes();
            Array.Sort(displayModes, SortDisplay);

            
            buttons = new SortedDictionary<string, Button>();
            textBoxes = new SortedDictionary<string, TextBox>();
            inputBoxes = new SortedDictionary<string, InputBox>();
            grids = new SortedDictionary<string, Grid>();
            images = new SortedDictionary<string, RawImage>();
            animations = new List<Animation>();
        }

        public static void LoadContent()
        {
            game.loadingString = "Loading textures...";
            maskEffect = game.Content.Load<Effect>("shaders\\mask");
            logo = game.Content.Load<Texture2D>("gui\\logo");
            errorBackground = game.Content.Load<Texture2D>("errorBackground");
            errorWindow = game.Content.Load<Texture2D>("tekstura");
            emptyTextbox = game.Content.Load<Texture2D>("empty");
            noButton = game.Content.Load<Texture2D>("NO");
            yesButton = game.Content.Load<Texture2D>("YES");
            errorButton = game.Content.Load<Texture2D>("errorButton");
            unTickedTexture = game.Content.Load<Texture2D>("gui\\toggleboxoff");
            tickedTexture = game.Content.Load<Texture2D>("gui\\toggleboxon");
            chatTexture = game.Content.Load<Texture2D>("gamegui\\chattexture");
            boardTexture = game.Content.Load<Texture2D>("gamegui\\boardtexture");
            chatInputTexture = game.Content.Load<Texture2D>("gamegui\\chatinput");
            chatSendTexture = game.Content.Load<Texture2D>("gamegui\\chatsend");
            skurwielTexture = game.Content.Load<Texture2D>("zoltyskurwiel");
            readyTexture = game.Content.Load<Texture2D>("ReadyButton");
            notReadyTexture = game.Content.Load<Texture2D>("NotReadyButton");
            semiTransparentTexture = game.Content.Load<Texture2D>("SemiTransparent");
            transparentTexture = game.Content.Load<Texture2D>("Transparent");
            solidTexture = game.Content.Load<Texture2D>("white");
            solidBlack = game.Content.Load<Texture2D>("black");
            solidGray = game.Content.Load<Texture2D>("gray");
            settingsTexture = game.Content.Load<Texture2D>("SettingsButton");
            menuBackground = game.Content.Load<Texture2D>("gui\\menubackground");
            optionsWindow = game.Content.Load<Texture2D>("gui\\optionswindow");
            connectWindow = game.Content.Load<Texture2D>("gui\\connectwindow");
            windowTexture = game.Content.Load<Texture2D>("gui\\window");
            inputboxTexture = game.Content.Load<Texture2D>("gui\\inputbox");
            inputboxSmallTexture = game.Content.Load<Texture2D>("gui\\inputboxsmall");
            inputboxLongTexture = game.Content.Load<Texture2D>("gui\\inputboxlong");
            dropdownTexture = game.Content.Load<Texture2D>("gui\\dropdown");
            dropdownMenuTexture = game.Content.Load<Texture2D>("gui\\dropdownmenu");
            fullscreenTexture = game.Content.Load<Texture2D>("gui\\fullscreen");
            resolutionTexture = game.Content.Load<Texture2D>("gui\\resolution");
            volumeTexture = game.Content.Load<Texture2D>("gui\\volume");
            doneTexture = game.Content.Load<Texture2D>("gui\\done");
            exitTexture = game.Content.Load<Texture2D>("gui\\exit");
            joinTexture = game.Content.Load<Texture2D>("gui\\join");
            optionsTexture = game.Content.Load<Texture2D>("gui\\options");
            quitTexture = game.Content.Load<Texture2D>("gui\\quit");
            cancelTexture = game.Content.Load<Texture2D>("gui\\cancel");
            connectTexture = game.Content.Load<Texture2D>("gui\\connect");
            bubbles = game.Content.Load<SoundEffect>("SoundFX/menuback2");
            boop = game.Content.Load<SoundEffect>("SoundFX/normal-hitwhistle");
            bye = game.Content.Load<SoundEffect>("SoundFX/seeya");
            menuMusic = game.Content.Load<SoundEffect>("SoundFX/ruetobastitle");
            for (int i = 0; i <= 72; i++)
                cardTexture[i] = game.Content.Load<Texture2D>("cards\\card" + i.ToString());
            tileGrass = game.Content.Load<Texture2D>("tileGrass");
            tileDirt = game.Content.Load<Texture2D>("tileDirt");
            tileGrid = game.Content.Load<Texture2D>("tileGrid");
            buffTexture[0] = game.Content.Load<Texture2D>("buffpickaxe");
            buffTexture[1] = game.Content.Load<Texture2D>("bufflantern");
            buffTexture[2] = game.Content.Load<Texture2D>("buffcart");
            for (int i = 0; i < 3; i++)
                mapCardTexture[i] = game.Content.Load<Texture2D>("cards\\mapcard" + (i + 42).ToString());
            gameChatTexture = game.Content.Load<Texture2D>("gamegui\\chatgroup");
            gameBackgroundTexture = game.Content.Load<Texture2D>("gamegui\\gamebackground");

            game.loadingString = "Loading cards...";
            ReadCards();

            for (int i = 0; i <= 45; i++)
            {
                game.loadingString = "Generating tunnel textures... (" + i.ToString() + "/45)";
                tileTunnel[i] = Render.RenderTunnel(game, spriteBatch, i);
            }

            game.loadingString = "Reseting variables...";
            map = new PlacedCard[19, 15];
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 15; j++)
                    map[i, j] = new PlacedCard(0, 0);
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
            if (grids.ContainsKey(gameNamespace + "ZPLAYERLIST"))
            {
                grids[gameNamespace + "ZPLAYERLIST"].offset.Y = Math.Max(grids[gameNamespace + "ZPLAYERLIST"].offset.Y, grids[gameNamespace + "ZPLAYERLIST"].location.Height / 2 - grids[gameNamespace + "ZPLAYERLIST"].margin);
                grids[gameNamespace + "ZPLAYERLIST"].offset.X = grids[gameNamespace + "ZPLAYERLIST"].location.Width / 2 - grids[gameNamespace + "ZPLAYERLIST"].margin;
                grids[gameNamespace + "ZPLAYERLIST"].zoom = 1.0f;
            }

            //Usuwanie hover textu
            if (textBoxes.ContainsKey(gameNamespace + "ZZZHOVER"))
            {
                textBoxes.Remove(gameNamespace + "ZZZHOVER");
                images.Remove(gameNamespace + "ZZHOVER");
            }

            //Żeby karty się nie przesuwały
            if (grids.ContainsKey(gameNamespace + "CARDS"))
            {
                Grid cardsgrid = grids[gameNamespace + "CARDS"];
                cardsgrid.zoom = 1.0f;
                cardsgrid.offset = new Vector2(cardsgrid.location.Width / 2 - cardsgrid.margin, cardsgrid.location.Height / 2 - cardsgrid.margin);
                
                //Rysowanie hover textu
                if (cardsgrid.enabled && Geo.Scale(Geo.Shrink(cardsgrid.location, cardsgrid.margin)).Contains(new Point(game.mouseState.X, game.mouseState.Y)))
                {
                    int id = (game.mouseState.X - cardsgrid.location.X - cardsgrid.margin) / (int)cardsgrid.fieldSize.X;
                    Card card = cards[id];
                    string text = "ELO\nTestowy hover xDDDD";
                    Vector2 size = font.MeasureString(text);
                    Rectangle rect = Geo.Shrink(new Rectangle((int)(game.mouseState.X / Game.scale.X), (int)(game.mouseState.Y / Game.scale.Y) - (int)size.Y, (int)size.X, (int)size.Y), -3);
                    images[gameNamespace + "ZZHOVER"] = new RawImage(solidBlack, Geo.Shrink(rect, -2));
                    textBoxes[gameNamespace + "ZZZHOVER"] = new TextBox(solidGray, 3, Alignment.Left, font, rect, text)
                    {
                        registerClicks = false
                    };
                }
            }

            //Ikonki buffów
            if (yourPlayerId >= 0)
            {
                for (int i = 0; i < 8; i++)
                    if (buttons.ContainsKey(gameNamespace + "ZBUFFICON" + i.ToString()))
                        buttons.Remove(gameNamespace + "ZBUFFICON" + i.ToString());
                for (int i = 0; i < players[yourPlayerId].buffs.Count; i++)
                {
                    buttons[gameNamespace + "ZBUFFICON" + i.ToString()] = new Button(buffTexture[(int)players[yourPlayerId].buffs[i] - 1], new Rectangle(0, 78 * i, 72, 72), null);
                    buttons[gameNamespace + "ZBUFFICON" + i.ToString()].registerClicks = false;
                }
            }

            if (game.IsActive && game.keyboardState.IsKeyDown(Keys.Enter) && game.keyboardBeforeState.IsKeyUp(Keys.Enter) && inputBoxes.ContainsKey(gameNamespace + "CHATINPUT") && inputBoxes[gameNamespace + "CHATINPUT"].active)
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
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap(sub.Substring(5).Trim(), Color.Black);
                if (data[0] == "JOIN")
                {
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap(sub.Substring(5).Trim() + " has joined the game.", Color.DarkGoldenrod);
                    players.Add(new Player(0, sub.Substring(5).Trim()));
                    SortPlayers();
                }
                if (data[0] == "BYE")
                {
                    PlaySound(bye, 1.0f);
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap(sub.Substring(4).Trim() + " has left the game.", Color.DarkGoldenrod);
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
                if (data[0] == "READY")
                {
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap(data[1] + " is ready", Color.LawnGreen);
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
                    grids[gameNamespace + "BOARD"].fieldTexture[x, y] = cardTexture[ID];
                    Card.RecalculateReach();
                }
                if (data[0] == "MAP")
                {
                    int ID = int.Parse(data[1]) - 42;
                    int x = int.Parse(data[2]);
                    int y = int.Parse(data[3]);
                    grids[gameNamespace + "BOARD"].fieldTexture[x, y] = mapCardTexture[ID];
                }
                if (data[0] == "START")
                {
                    gameInProgress = true;
                    for (int i = 0; i < 19; i++)
                        for (int j = 0; j < 15; j++)
                            map[i, j] = new PlacedCard(0, 0);
                    map[5, 7] = new PlacedCard(1, 0);
                    map[13, 5] = new PlacedCard(45, 0);
                    map[13, 7] = new PlacedCard(45, 0);
                    map[13, 9] = new PlacedCard(45, 0);

                    for (int i = 1; i < 18; i++)
                        for (int j = 1; j < 14; j++)
                            grids[gameNamespace + "BOARD"].fieldTexture[i, j] = cardTexture[0];
                    grids[gameNamespace + "BOARD"].fieldTexture[5, 7] = cardTexture[1];
                    grids[gameNamespace + "BOARD"].fieldTexture[13, 5] = cardTexture[45];
                    grids[gameNamespace + "BOARD"].fieldTexture[13, 7] = cardTexture[45];
                    grids[gameNamespace + "BOARD"].fieldTexture[13, 9] = cardTexture[45];
                    buttons[gameNamespace + "READY"].enabled = false;
                    for (int i = 0; i < data.Count() - 2; i++)
                        cardHand[i] = int.Parse(data[i + 1]);
                    selectedCard = -1;

                    for (int i = 0; i < players.Count; i++)
                        players[i].playerClass = PlayerClass.Unknown;
                    for (int i = 0; i < players.Count; i++)
                        players[i].buffs.Clear();

                    players[yourPlayerId].playerClass = (PlayerClass)int.Parse(data[data.Count() - 1]);
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("The game has started", Color.LawnGreen);
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You are: " + players[yourPlayerId].playerClass.ToString(), Color.LawnGreen);
                    Card.RecalculateReach();
                }
                if (data[0] == "GIB")
                {
                    for (int j = 1; j < data.Count(); j++)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (cardHand[i] == 0)
                            {
                                cardHand[i] = int.Parse(data[j]);
                                i = 6;
                            }
                        }
                    }
                    selectedCard = -1;
                    selectedRot = 0;
                }
                if (data[0] == "TAEK")
                {
                    for (int j = 1; j < data.Count(); j++)
                    {
                        for (int i = 5; i >= 0; i--)
                        {
                            if (cardHand[i] == int.Parse(data[j]))
                            {
                                for (int ij = i; ij < 5; ij++)
                                    cardHand[ij] = cardHand[ij + 1];
                                cardHand[5] = 0;
                            }
                        }
                    }
                    selectedCard = -1;
                    selectedRot = 0;
                }
                if (data[0] == "TURN")
                {
                    if (data.Count() > 2)
                    {
                        string player = playerTurn;
                        if (playerTurn == username)
                            player = "You";
                        string line = "";
                        if (data[2] == "PLACE")
                            line = playerTurn + " placed a tunnel card";
                        if (data[2] == "CRUSH")
                            line = playerTurn + " destroyed a tunnel";
                        if (data[2] == "MAP")
                        {
                            int pos = int.Parse(data[5]);
                            string posline = pos == 5 ? "top" : (pos == 7 ? "middle" : "bottom");
                            line = playerTurn + " used map on " + posline + " treasure card";
                        }
                        if (data[2] == "BUFF")
                        {
                            string buffname = ((BuffCard)cards[int.Parse(data[3])]).buffType.ToString();
                            line = playerTurn + " gave a " + buffname + " buff to " + data[4].Trim();
                        }
                        if (data[2] == "DEBUFF")
                        {
                            DebuffCard card = (DebuffCard)cards[int.Parse(data[3])];
                            string buffname = data[5] == "0" ? card.buffType.ToString() : card.buffType2.ToString();
                            line = playerTurn + " removed a " + buffname + " buff from " + data[4].Trim();
                        }
                        if (data[2] == "DISCARD")
                            line = playerTurn + " discarded " + int.Parse(data[3]) + " card" + (int.Parse(data[3]) > 1 ? "s" : "");
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap(line, Color.Purple);
                    }

                    playerTurn = data[1].Trim();
                    textBoxes[gameNamespace + "PLAYERTURNTEXT"].lines[0] = playerTurn + "'s turn";
                    if (playerTurn == username)
                    {
                        textBoxes[gameNamespace + "HELP"].lines[0] = "Select card from your hand";
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("It's your turn", Color.Purple);
                        PlaySound(bubbles, 1.0f);
                    }
                    else
                    {
                        PlaySound(boop, 0.3f);
                    }
                }
                if (data[0] == "END")
                {
                    gameInProgress = false;
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("Team " + ((PlayerClass)int.Parse(data[1])).ToString() + " wins!", Color.LawnGreen);
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Team " + ((PlayerClass)int.Parse(data[1])).ToString() + " wins!";
                    for (int i = 0; i < players.Count; i++)
                    {
                        string playername = data[3 * i + 2];
                        for (int j = 0; j < players.Count; j++)
                            if (players[j].username == playername)
                            {
                                if (playername == username)
                                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You got " + (int.Parse(data[3 * i + 3]) - players[j].score).ToString() + " points!", Color.LawnGreen);
                                players[j].score = int.Parse(data[3 * i + 3]);
                                players[j].playerClass = (PlayerClass)int.Parse(data[3 * i + 4]);
                            }
                    }
                    //grids[gameNamespace + "BOARD"].enabled = false;
                    buttons[gameNamespace + "READY"].enabled = true;
                    buttons[gameNamespace + "READY"].texture = notReadyTexture;
                }
                if (data[0] == "BUFF")
                {
                    Player.FindByName(data[2].Trim()).AddBuff((Buff)int.Parse(data[3]));
                }
                if (data[0] == "DEBUFF")
                {
                    Player.FindByName(data[2].Trim()).RemoveBuff((Buff)int.Parse(data[3]));
                }
                if (data[0] == "DISCONNECT")
                {
                    Disconnect(sub.Substring(11));
                }
                if (data[0] == "OK")
                {
                    if (data[1] == "READY")
                    {
                        buttons[gameNamespace + "READY"].texture = readyTexture;
                    }
                    if (data[1] == "ADMIN")
                    {
                        adminMode = true;
                    }
                }
            }
        }

        public static void SendChatMessage()
        {
            string input = inputBoxes[gameNamespace + "CHATINPUT"].text.Trim();
            if (input.Length == 0)
                return;
            if (input[0] == '/')
            {
                string[] data = input.Substring(1).Split(' ');
                if (data[0] == "admin")
                {
                    if (data.Count() == 1)
                        game.TCPSend("ADMIN");
                    else
                        game.TCPSend("ADMIN " + input.Substring(7));
                }
                else if (data[0] == "kick")
                {
                    if (!adminMode)
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You are not allowed to use this command", Color.OrangeRed);
                    else if (data.Count() == 1)
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("Format: /kick <username>");
                    else
                        game.TCPSend("KICK " + data[1]);
                }
                else if (data[0] == "forcestart")
                {
                    if (adminMode)
                    {
                        if (!gameInProgress)
                            game.TCPSend("FORCESTART");
                        else
                            textBoxes[gameNamespace + "CHAT"].AppendAndWrap("The game is already running", Color.OrangeRed);
                    }
                    else
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You are not allowed to use this command", Color.OrangeRed);
                }
                else if (data[0] == "forceskip")
                {
                    if (adminMode)
                    {
                        if (gameInProgress)
                            game.TCPSend("FORCESKIP");
                        else
                            textBoxes[gameNamespace + "CHAT"].AppendAndWrap("The game is not running", Color.OrangeRed);
                    }
                    else
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You are not allowed to use this command", Color.OrangeRed);
                }
                else if (data[0] == "say")
                {
                    if (!adminMode)
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You are not allowed to use this command", Color.OrangeRed);
                    else if (data.Count() == 1)
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("Format: /say <message>");
                    else
                        game.TCPSend("SAY " + input.Substring(5));
                }
                else if (data[0] == "help")
                {
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("List of commands:");
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("/admin <password>");
                    if (adminMode)
                    {
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("/kick <username>");
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("/say <message>");
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("/forcestart");
                        textBoxes[gameNamespace + "CHAT"].AppendAndWrap("/forceskip");
                    }
                }
                else
                    textBoxes[gameNamespace + "CHAT"].AppendAndWrap("Unknown command. Type /help for list of commands", Color.OrangeRed);
            }
            else
                game.TCPSend("CHAT " + inputBoxes[gameNamespace + "CHATINPUT"].text);
            inputBoxes[gameNamespace + "CHATINPUT"].text = "";
        }

        public static void ReadCards()
        {
            cards = new List<Card>();
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
            IP = inputBoxes[menuNamespace + "Zip"].text;
            username = inputBoxes[menuNamespace + "Znick"].text.Trim();

            if (game.TCPConnect(IP, port))
            {
                UI.DisableGroup(optionsNamespace);
                UI.DisableGroup(menuNamespace);
                UI.EnableGroup(gameNamespace);
                HidePlayerList();
                menuMusicInstance.Stop();
                for (int i = 0; i < 6; i++)
                    cardHand[i] = 0;
                for (int i = 0; i < 19; i++)
                    for (int j = 0; j < 15; j++)
                        map[i, j] = new PlacedCard(0, 0);
                textBoxes[gameNamespace + "CHAT"].Reset();
                textBoxes[gameNamespace + "CHAT"].AppendAndWrap("You joined the game.", Color.DarkGoldenrod);
                inputBoxes[gameNamespace + "CHATINPUT"].text = "";
                buttons[gameNamespace + "READY"].texture = notReadyTexture;
            }
            else
            {
                AnnounceError("Connection error, try again");
            }
        }

        public static void Disconnect(string error)
        {
            yourPlayerId = -1;
            players.Clear();
            for (int i = 0; i < 6; i++)
                cardHand[i] = 0;
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 15; j++)
                    map[i, j] = new PlacedCard(0, 0);
            textBoxes[gameNamespace + "CHAT"].Reset();
            UI.DisableGroup(gameNamespace);
            UI.DisableGroup(optionsNamespace);
            UI.EnableGroup(menuNamespace);
            if (error != "")
                AnnounceError(error);
            adminMode = false;
            gameInProgress = false;
            game.tcpThread.Abort();
        }

        public static void ShowPlayerList()
        {
            buttons[gameNamespace + "PLAYERLISTTOGGLE"].clickEvent = HidePlayerList;
            grids[gameNamespace + "ZPLAYERLIST"].enabled = true;
            textBoxes[gameNamespace + "PLAYERLISTTOGGLETEXT"].lines[0] = "Hide player list";
        }

        public static void HidePlayerList()
        {
            buttons[gameNamespace + "PLAYERLISTTOGGLE"].clickEvent = ShowPlayerList;
            grids[gameNamespace + "ZPLAYERLIST"].enabled = false;
            textBoxes[gameNamespace + "PLAYERLISTTOGGLETEXT"].lines[0] = "Show player list";
        }

        public static void DiscardCard()
        {
            if (playerTurn != username)
            {
                textBoxes[gameNamespace + "HELP"].lines[0] = "You can only play cards during your turn";
                return;
            }

            if (selectedCard == -1 || cardHand[selectedCard] == 0)
            {
                textBoxes[gameNamespace + "HELP"].lines[0] = "Select a card you would like to discard first";
                return;
            }

            int id = cardHand[selectedCard];
            game.TCPSend("DISCARD " + id.ToString());
        }


        public static void BoardDraw(SpriteBatch spriteBatch, Rectangle location, int x, int y)
        {
            if (map[x, y].ID == 45)
            {
                spriteBatch.Draw(grids[gameNamespace + "BOARD"].fieldTexture[x, y], location, Color.White);
            }
            else
            {
                spriteBatch.Draw(tileGrass, location, Color.White);
                float rot = map[x, y].rotation == 1 ? (float)Math.PI : 0.0f;
                if (map[x, y].rotation == 1)
                {
                    location.X += location.Width;
                    location.Y += location.Height;
                }
                spriteBatch.Draw(tileTunnel[map[x, y].ID], location, null, Color.White, rot, Vector2.Zero, SpriteEffects.None, 0);
                if (map[x, y].rotation == 1)
                {
                    location.X -= location.Width;
                    location.Y -= location.Height;
                }
            }
            spriteBatch.Draw(tileGrid, location, Color.White);
        }

        public static void HandDraw(SpriteBatch spriteBatch, Rectangle location, int x, int y)
        {
            location = Geo.Shrink(location, 20);
            if (selectedRot == 0 || x != selectedCard)
            {
                if (cards[cardHand[x]].cardType == CardType.Tunnel)
                {
                    spriteBatch.Draw(tileGrass, location, Color.White);
                    spriteBatch.Draw(tileTunnel[cardHand[x]], location, Color.White);
                }
                else
                    spriteBatch.Draw(cards[cardHand[x]].texture, location, Color.White);

                if (selectedCard == x)
                    spriteBatch.Draw(solidTexture, location, Color.Yellow * 0.3f);
            }
            else
            {
                location.X += location.Width;
                location.Y += location.Height;
                if (cards[cardHand[x]].cardType == CardType.Tunnel)
                {
                    spriteBatch.Draw(tileGrass, location, null, Color.White, (float)Math.PI, Vector2.Zero, SpriteEffects.None, 0);
                    spriteBatch.Draw(tileTunnel[cardHand[x]], location, null, Color.White, (float)Math.PI, Vector2.Zero, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(cards[cardHand[x]].texture, location, null, Color.White, (float)Math.PI, Vector2.Zero, SpriteEffects.None, 0);
                }

                location.X -= location.Width;
                location.Y -= location.Height;
                if (selectedCard == x)
                    spriteBatch.Draw(solidTexture, location, Color.Yellow * 0.3f);
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
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Place card on board or rotate it by clicking on it again";
                else if (cardType == CardType.Map)
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Select treasure card you would like to uncover";
                else if (cardType == CardType.Remove)
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Select placed tunnel you would like to demolish";
                else if (cardType == CardType.Buff)
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Select player from player list to inflict this effect on him";
                else if (cardType == CardType.Debuff)
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Select player from player list to remove this effect from him";
            }
        }

        public static void BoardClick(int x, int y)
        {
            if (selectedCard == -1)
                return;

            if (playerTurn != username)
            {
                textBoxes[gameNamespace + "HELP"].lines[0] = "You can only play cards during your turn";
                return;
            }

            if (cards[cardHand[selectedCard]].cardType == CardType.Tunnel)
            {
                if (players[yourPlayerId].buffs.Contains(Buff.Cart))
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "You cannot play tunnel cards unless you repair your cart";
                    return;
                }

                if (players[yourPlayerId].buffs.Contains(Buff.Lantern))
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "You cannot play tunnel cards unless you repair your lantern";
                    return;
                }

                if (players[yourPlayerId].buffs.Contains(Buff.Pickaxe))
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "You cannot play tunnel cards unless you repair your pickaxe";
                    return;
                }

                int result = Card.CheckPlacement(x, y, cardHand[selectedCard], selectedRot);
                if (result == 0)
                {
                    string line = "PLACE " + cardHand[selectedCard].ToString() + " " + x.ToString() + " " + y.ToString() + " " + selectedRot.ToString();
                    game.TCPSend(line);
                }
                else if (result == 1)
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Placed tunnel must be connected to starting point";
                }
                else if (result == 2)
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "Played card must match to its neighbours";
                }
                else if (result == 3)
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "You cannot place card on occupied spot";
                }
                else if (result == 4)
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "You can only place tunnel cards";
                }
            }
            else if (cards[cardHand[selectedCard]].cardType == CardType.Remove)
            {
                if (cards[map[x, y].ID].cardType == CardType.Tunnel)
                {
                    if (map[x, y].ID == 1)
                    {
                        textBoxes[gameNamespace + "HELP"].lines[0] = "You can't demolish starting point";
                    }
                    else if (map[x, y].ID == 42 || map[x, y].ID == 43 || map[x, y].ID == 44 || map[x, y].ID == 45)
                    {
                        textBoxes[gameNamespace + "HELP"].lines[0] = "You can't demolish treasure card";
                    }
                    else
                    {
                        string line = "CRUSH " + cardHand[selectedCard].ToString() + " " + x.ToString() + " " + y.ToString();
                        game.TCPSend(line);
                    }
                }
                else
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "This card must be used on placed tunnel";
                }
            }
            else if (cards[cardHand[selectedCard]].cardType == CardType.Map)
            {
                if (map[x, y].ID == 45)
                {
                    string line = "MAP " + cardHand[selectedCard].ToString() + " " + x.ToString() + " " + y.ToString();
                    game.TCPSend(line);
                }
                else
                {
                    textBoxes[gameNamespace + "HELP"].lines[0] = "This card must be used on uncovered treasure card";
                }
            }
        }

        public static void PlayerListClick(int x, int y)
        {
            if (selectedCard == -1)
                return;

            if (playerTurn != username)
            {
                textBoxes[gameNamespace + "HELP"].lines[0] = "You can only play cards during your turn";
                return;
            }

            int id = cardHand[selectedCard];
            if (cards[id].cardType == CardType.Buff)
            {
                if (players[y].buffs.Contains(((BuffCard)cards[id]).buffType))
                    textBoxes[gameNamespace + "HELP"].lines[0] = "This player already has this effect applied";
                else
                {
                    string line = "BUFF " + id.ToString() + " " + players[y].username + " 0";
                    game.TCPSend(line);
                }
            }

            if (cards[id].cardType == CardType.Debuff)
            {
                Buff buff = ((DebuffCard)cards[id]).buffType;
                if (selectedRot == 1 && ((DebuffCard)cards[id]).buffType2 != Buff.None)
                    buff = ((DebuffCard)cards[id]).buffType2;

                if (!players[y].buffs.Contains(buff))
                    textBoxes[gameNamespace + "HELP"].lines[0] = "This player doesn't have this effect applied";
                else
                {
                    string line = "DEBUFF " + id.ToString() + " " + players[y].username + " " + selectedRot.ToString();
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

        public static void Ask(string question, Action yes, Action no)
        {
            buttons["ZZXBACKGROUND"] = new Button(errorBackground, new Rectangle(0, 0, 1920, 1080), null);
            textBoxes["ZZXQUESTIONBOX"] = new TextBox(emptyTextbox, 15, Alignment.Centered, font, new Rectangle(560, 375, 800, 220), question);
            buttons["ZZXBUTTON"] = new Button(errorWindow, new Rectangle(560, 300, 800, 330), null);
            buttons["ZZZYESBUTTON"] = new Button(yesButton, new Rectangle(760, 550, 150, 50), ()=>CloseQuestionBox(yes));
            buttons["ZZZNOBUTTON"] = new Button(noButton, new Rectangle(1010, 550, 150, 50), ()=>CloseQuestionBox(no));
        }

        public static void CloseQuestionBox(Action decision)
        {
            buttons.Remove("ZZXBACKGROUND");
            textBoxes.Remove("ZZXQUESTIONBOX");
            buttons.Remove("ZZXBUTTON");
            buttons.Remove("ZZZYESBUTTON");
            buttons.Remove("ZZZNOBUTTON");
            decision();
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

        public static void ChangeFullscreen()
        {
            if (!Settings.isFullscreen)
                buttons[optionsNamespace + "Fullscreen"].texture = tickedTexture;
            else
                buttons[optionsNamespace + "Fullscreen"].texture = unTickedTexture;
            Settings.ChangeFullscreen();
        }

        public static void DisplayOptions(bool shouldBeVisible, bool updateOptions)
        {
            if (shouldBeVisible)
            {
                UI.EnableGroup(optionsNamespace);
                inputBoxes[optionsNamespace + "Volume"].text = "";
                grids[optionsNamespace + "ZResolutions"].enabled = false;
                int gcd = GCD(Settings.resolution.X, Settings.resolution.Y);
                string line = Settings.resolution.X.ToString() + " x " + Settings.resolution.Y.ToString() + "    " +
                    (Settings.resolution.X / gcd).ToString() + ":" + (Settings.resolution.Y / gcd).ToString();
                textBoxes[optionsNamespace + "ResolutionSelected"].lines[0] = line;
                if (Settings.isFullscreen)
                    buttons[optionsNamespace + "Fullscreen"].texture = Logic.tickedTexture;
                else
                    buttons[optionsNamespace + "Fullscreen"].texture = Logic.unTickedTexture;
            }
            else
            {
                UI.DisableGroup(optionsNamespace);

                if (updateOptions == true)
                {
                    int new_volume = 0;
                    if (int.TryParse(inputBoxes[optionsNamespace + "Volume"].text, out new_volume) &&
                        new_volume <= 100 && new_volume >= 0)
                    {
                        Settings.volume = new_volume * 0.01f;
                        menuMusicInstance.Volume = Settings.volume;
                    }
                    Settings.SaveToFile();
                }
                else
                    Settings.LoadFromFile();
            }
        }

        public static void ResolutionsDraw(SpriteBatch spriteBatch, Rectangle location, int x, int y)
        {
            y = GetTheNthFittingResolution(y);
            if (y == -1) return;
            Point curRes = new Point(displayModes[y].Width, displayModes[y].Height);
            Color c = game.GetCurrentWindowResolution() == curRes ? Color.LightYellow : Color.White;
            int gcd = GCD(displayModes[y].Width, displayModes[y].Height);
            string line = displayModes[y].Width.ToString() + " x " + displayModes[y].Height.ToString() + "    " +
                (displayModes[y].Width / gcd).ToString() + ":" + (displayModes[y].Height / gcd).ToString();
            spriteBatch.DrawString(guifont, line,
                new Vector2(location.X + (location.Width - guifont.MeasureString(line).X) / 2, location.Y + (location.Height - guifont.MeasureString(line).Y) / 2), Color.White);
        }

        public static void ResolutionsClick(int x, int y)
        {
            y = GetTheNthFittingResolution(y);
            if (y == -1) return;
            int gcd = GCD(displayModes[y].Width, displayModes[y].Height);
            string line = displayModes[y].Width.ToString() + " x " + displayModes[y].Height.ToString() + "    " +
                (displayModes[y].Width / gcd).ToString() + ":" + (displayModes[y].Height / gcd).ToString();
            textBoxes[optionsNamespace + "ResolutionSelected"].lines[0] = line;
            grids[optionsNamespace + "ZResolutions"].enabled = false;
            game.ChangeResolution(displayModes[y].Width, displayModes[y].Height);
        }

        public static int GetTheNthFittingResolution(int N)
        {
            for (int i = 0; i < displayModes.Length; i++)
            {
                Point curRes = game.GetCurrentDeviceResolution();
                if (onlyNativeRes)
                {
                    if (displayModes[i].Width / (float)displayModes[i].Height == curRes.X / (float)curRes.Y)
                        N--;
                }
                else
                    N--;

                if (N == -1) return i;
            }
            return -1;
        }
        
        public static void PlaySound(SoundEffect soundEffect, float playVolume)
        {
            playVolume *= Settings.volume;
            if (playVolume < 0.0f || playVolume > 1.0f)
                return;
            soundEffect.Play(playVolume, 0.0f, 0.0f);
        }

        public static int SortDisplay(DisplayMode a, DisplayMode b)
        {
            bool aSupported = (a.Width <= maxResDefault.X && a.Height <= maxResDefault.Y);
            bool bSupported = (b.Width <= maxResDefault.X && b.Height <= maxResDefault.Y);
            if (aSupported && !bSupported)
                return -1;
            if (!aSupported && bSupported)
                return 1;

            if (a.Width > b.Width)
                return -1;
            if (a.Width < b.Width)
                return 1;
            
            return a.Height.CompareTo(b.Height);
        }

        public static void ChangeNativeResMode()
        {
            onlyNativeRes = !onlyNativeRes;
            if (onlyNativeRes)
                buttons[optionsNamespace + "nativeRes"].texture = tickedTexture;
            else
                buttons[optionsNamespace + "nativeRes"].texture = unTickedTexture;
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
