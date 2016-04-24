using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScumbagGalaxy.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using ScumbagGalaxy.Managers;
using ScumbagGalaxy.UI.UIUnits;
using ScumbagGalaxy.UI.UIStartScreen;

namespace ScumbagGalaxy
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        //Save the mouse state in a variable
        MouseState mousePrevState;
        MouseState mouseState;

        //Create a game window
        GameWindow gameWindow;

        //Size (in pixels) of the game
        int gameWidth;
        int gameHeight;

        //Size (in pixels) of the game window
        int windowWidth;
        int windowHeight;
        
        //Default UIBox texture
        Texture2D UIBoxSprite;
        
        //Lists of players
        List<Player> playList;

        //UIGrid
        UIGrid UIGrid;


        //TODO: Have a rockManager or something
        TokenUnit rock;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Mouse Visibility
            this.IsMouseVisible = true;

            //Set the game size
            this.gameWidth = 2080;
            this.gameHeight = 1198;

            //Set the window size
            this.windowWidth = 1280;
            this.windowHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {

            //Create a game board for each game state (gameMenu, teamSelect, and game)
            GameBoard.game = new GameBoard(this.gameWidth, this.gameHeight, 8);
            GameBoard.teamSelect = new GameBoard(this.windowWidth, this.windowHeight, 8);
            GameBoard.startMenu = new GameBoard(this.windowWidth, this.windowHeight, 8);
            GameBoard.endMenu = new GameBoard(this.windowWidth, this.windowHeight, 8);

            //Save the monitor size
            int monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //Calculate new window size
            this.windowHeight = (int)(monitorHeight / 1.2);
            this.windowWidth = (int)(this.windowHeight * 1.7777778);
            //Set the screen location
            //Window.IsBorderless = true; //Make the window borderless
            Window.Position = new Point((monitorWidth - this.windowWidth) / 2, (monitorHeight - this.windowHeight) / 2);
            //Set the view size to be the window size
            GameBoard.game.viewWidth = this.windowWidth;
            GameBoard.game.viewHeight = this.windowHeight;
            GameBoard.teamSelect.viewWidth = this.windowWidth;
            GameBoard.teamSelect.viewHeight = this.windowHeight;
            GameBoard.startMenu.viewWidth = this.windowWidth;
            GameBoard.startMenu.viewHeight = this.windowHeight;
            GameBoard.endMenu.viewWidth = this.windowWidth;
            GameBoard.endMenu.viewHeight = this.windowHeight;

            //Set the active game state to the game menu
            GameBoard.activeState = 1;

            //Grab the mouse state
            mouseState = Mouse.GetState();

            //Set the screen size
            graphics.PreferredBackBufferWidth = GameBoard.game.viewWidth;
            graphics.PreferredBackBufferHeight = GameBoard.game.viewHeight;
            graphics.ApplyChanges();


            //Create the game grid
            Grid.mainGrid = new Grid(30, 16);

            //Create two players and put them in the playList
            playList = new List<Player>();
            playList.Add(new Player(OwnerPlayer.One));
            playList.Add(new Player(OwnerPlayer.Two));

            //Import all units
            ImportUnit(playList[0], "Shiv.bin");
            ImportUnit(playList[0], "Berserker.bin");
            ImportUnit(playList[0], "Bomblobber.bin");
            ImportUnit(playList[0], "Portmage.bin");
            ImportUnit(playList[0], "Optimist.bin");
            ImportUnit(playList[0], "Shade.bin");
            ImportUnit(playList[0], "Hauler.bin");
            ImportUnit(playList[0], "Stinger.bin");
            ImportUnit(playList[0], "Blunderstriker.bin");
            ImportUnit(playList[0], "Shiv.bin");

            ImportUnit(playList[1], "Shiv.bin");
            ImportUnit(playList[1], "Paladin.bin");
            ImportUnit(playList[1], "Telepriest.bin");
            ImportUnit(playList[1], "Portmage.bin");
            ImportUnit(playList[1], "Shiv.bin");
            ImportUnit(playList[1], "Optimist.bin");
            ImportUnit(playList[1], "Hauler.bin");
            ImportUnit(playList[1], "Bomblobber.bin");
            ImportUnit(playList[1], "Paladin.bin");
            ImportUnit(playList[1], "Shade.bin");

            Grid practiceGrid = new Grid(4, 6);
            practiceGrid.Set(1, 0, playList[0].Units[0]);
            practiceGrid.Set(2, 1, playList[0].Units[1]);
            practiceGrid.Set(0, 2, playList[0].Units[2]);
            practiceGrid.Set(1, 2, playList[0].Units[3]);
            practiceGrid.Set(3, 2, playList[0].Units[4]);
            practiceGrid.Set(0, 3, playList[0].Units[5]);
            practiceGrid.Set(1, 3, playList[0].Units[6]);
            practiceGrid.Set(3, 3, playList[0].Units[7]);
            practiceGrid.Set(2, 4, playList[0].Units[8]);
            practiceGrid.Set(1, 5, playList[0].Units[9]);
            Grid.mainGrid.SetRegion(5, 6, practiceGrid);

            practiceGrid.Clear();
            practiceGrid.Set(2, 0, playList[1].Units[0]);
            practiceGrid.Set(1, 1, playList[1].Units[1]);
            practiceGrid.Set(0, 2, playList[1].Units[2]);
            practiceGrid.Set(2, 2, playList[1].Units[3]);
            practiceGrid.Set(3, 2, playList[1].Units[4]);
            practiceGrid.Set(0, 3, playList[1].Units[5]);
            practiceGrid.Set(2, 3, playList[1].Units[6]);
            practiceGrid.Set(3, 3, playList[1].Units[7]);
            practiceGrid.Set(1, 4, playList[1].Units[8]);
            practiceGrid.Set(2, 5, playList[1].Units[9]);
            Grid.mainGrid.SetRegion(21, 6, practiceGrid);


            //adding some obstacles
            //rock = new TokenUnit(null, OwnerPlayer.None);
            //rock.SpritePath = "Images/CharacterArt/hauler";
            //rock.StackBuff(new Buff(BuffType.IsImmortal));//as they are not owned by any player, this buff will not decrement
            //rock.StackBuff(new Buff(BuffType.IsUnselectable));
            //Grid.mainGrid.Set(8, 8, rock);
            //Grid.mainGrid.Set(9, 9, rock);

            //Create instances of the player manager and the unit manager
            PlayerManager.Manager = new PlayerManager(playList);
            UnitManager.Manager = new UnitManager();

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

            /*
                Load content for the "game" state
            */
            //Load the background
            UIBox background = new UIBox(GameBoard.game, 0, 0, this.gameWidth, this.gameHeight);
            background.thisSprite = Content.Load<Texture2D>(@"Images/background");
            background.layer = 100;
            background.visible = true;
            background.mouseEvents = false;

            //Create a new UIGrid
            this.UIGrid = new UIGrid(Grid.mainGrid);
            //Pass that grid to the UnitManager object
            UnitManager.Manager.UIGrid = this.UIGrid;
            //Load grid textures
            UIGridCell.Load(Content);
            //Load health bar textures
            UIHealthBar.Load(Content);
            //Load moves left textures
            UIMovesLeft.Load(Content);
            //Load the active unit UI textures
            UIStatBackground.Load(Content);
            UITextBox.Load(Content);
            UISkillButton.Load(Content);
            UIDefaultActionButton.Load(Content);
            //Load UIUnit textures
            for (int p = 0; p < playList.Count; p++) {
                for (int i = 0; i < playList[p].Units.Count; i++) {
                    playList[p].Units[i].sprite.Load(Content);
                }
            }
            
            //rock.sprite.Load(Content);

            /*
                Load content for the "teamSelect" state
            */


            /*
                Load content for the "startMenu" state
            */
            //Load the background
            UIBox startMenuBackground = new UIBox(GameBoard.startMenu, 0, 0, this.windowWidth, this.windowHeight);
            startMenuBackground.thisSprite = Content.Load<Texture2D>(@"Images/TitleScreen/titleScreenBackground");
            startMenuBackground.layer = 100;
            startMenuBackground.visible = true;
            startMenuBackground.mouseEvents = false;
            startMenuBackground.viewMove = false;
            //Load the start button
            UIStartButton startButton = new UIStartButton(GameBoard.startMenu, (this.windowWidth - 672)/2, (this.windowHeight - 217)/2, 672, 217);
            UIStartButton.Load(Content);
            startButton.layer = 99;
            startButton.visible = true;
            startButton.viewMove = false;

            /*
                Load content for the 'endMenu' state
            */
            UIBox endMenuBackground = new UIBox(GameBoard.endMenu, 0, 0, this.windowWidth, this.windowHeight);
            endMenuBackground.thisSprite = Content.Load<Texture2D>(@"Images/EndScreen/endScreenBackground");
            endMenuBackground.layer = 100;
            endMenuBackground.visible = true;
            endMenuBackground.mouseEvents = false;
            endMenuBackground.viewMove = false;
            //Load the exit button
            UIExitButton exitButton = new UIExitButton(GameBoard.endMenu, (this.windowWidth - 672) / 2, ((this.windowHeight - 217) / 2) + 150, 672, 217);
            UIExitButton.Load(Content);
            exitButton.layer = 99;
            exitButton.visible = true;
            exitButton.viewMove = false;

            //Load other content here
            //Load fonts
            GameBoard.font = Content.Load<SpriteFont>("Fonts/dosis");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //UPDATE GAME BOARD
            //Set the previous state to the old game state
            mousePrevState = mouseState;
            //Get the new mouse state
            mouseState = Mouse.GetState();

            //Switch to determine game state
            switch (GameBoard.activeState) {
                case 1:
                    //Pass the old and new mouse state variables to the "update" function of the menu game board
                    GameBoard.startMenu.Update(mouseState, mousePrevState);
                    break;
                case 2:

                    break;
                case 3:
                    //Pass the old and new mouse state variables to the "update" function of the game board
                    GameBoard.game.Update(mouseState, mousePrevState);

                    //UPDATE ALL GAME STATE MANAGERS
                    PlayerManager.Manager.Update();
                    break;
                case 4:
                    GameBoard.endMenu.Update(mouseState, mousePrevState);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            
            //Draw the active game state
            //Switch to determine which game state is active
            switch (GameBoard.activeState) {
                case 1:
                    GameBoard.startMenu.Draw(spriteBatch);
                    break;
                case 2:
                    break;
                case 3:
                    GameBoard.game.Draw(spriteBatch);
                    break;
                case 4:
                    GameBoard.endMenu.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ImportUnit(Player whichPlayer, string whichfile)
        {
            BinaryReader myBR = new BinaryReader(File.Open(whichfile, FileMode.Open));
            String defaultAttackFileName = "";
            String ability1FileName = "";
            String ability2FileName = "";
            String ability3FileName = "";
            String ability4FileName = "";
            try {
                char[] delims = { ',' };
                String dataString =  System.Text.Encoding.ASCII.GetString(myBR.ReadBytes((int)myBR.BaseStream.Length));
                String[] dataStringArray = dataString.Split(delims);
                Console.WriteLine(dataStringArray[0]);
                if (playList[0] == whichPlayer) {
                    whichPlayer.Units.Add(new LiveUnit(int.Parse(dataStringArray[3]), OwnerPlayer.One));
                } else {
                    whichPlayer.Units.Add(new LiveUnit(int.Parse(dataStringArray[3]), OwnerPlayer.Two));
                }
                whichPlayer.Units[whichPlayer.Units.Count - 1].name = dataStringArray[0];
                whichPlayer.Units[whichPlayer.Units.Count - 1].description = dataStringArray[1];
                whichPlayer.Units[whichPlayer.Units.Count - 1].unitClass = dataStringArray[2];
                whichPlayer.Units[whichPlayer.Units.Count - 1].speed = int.Parse(dataStringArray[4]);
                whichPlayer.Units[whichPlayer.Units.Count - 1].critChance = int.Parse(dataStringArray[5]);
                whichPlayer.Units[whichPlayer.Units.Count - 1].critDamage = int.Parse(dataStringArray[6]);
                whichPlayer.Units[whichPlayer.Units.Count - 1].attackRange = int.Parse(dataStringArray[7]);
                whichPlayer.Units[whichPlayer.Units.Count - 1].attackDamage = int.Parse(dataStringArray[8]);
                whichPlayer.Units[whichPlayer.Units.Count - 1].baseHealsTaken = int.Parse(dataStringArray[9]);
                defaultAttackFileName = dataStringArray[10];
                ability1FileName = dataStringArray[11];
                ability2FileName = dataStringArray[12];
                ability3FileName = dataStringArray[13];
                ability4FileName = dataStringArray[14];
                Console.WriteLine("Problem free 1");
                try {
                    ImportAbility(defaultAttackFileName, whichPlayer);
                    Console.WriteLine("Problem free 2");
                } catch {
                    Console.WriteLine("Default attack not loaded");
                }
                try {
                    ImportAbility(ability1FileName, whichPlayer);
                    Console.WriteLine("Problem free 3");
                } catch {
                    Console.WriteLine("Ability 1 not loaded");
                }
                try {
                    ImportAbility(ability2FileName, whichPlayer);
                    Console.WriteLine("Problem Free 4");
                } catch {
                    Console.WriteLine("Ability 2 not loaded");
                }
                try {
                    ImportAbility(ability3FileName, whichPlayer);
                    Console.WriteLine("Problem Free 5");
                } catch {
                    Console.WriteLine("Ability 3 not loaded");
                }
                try {
                    ImportAbility(ability4FileName, whichPlayer);
                    Console.WriteLine("Problem Free 6");
                } catch {
                    Console.WriteLine("Ability 4 not loaded");
                }
                whichPlayer.Units[whichPlayer.Units.Count - 1].PortraitPath = dataStringArray[15];
                Console.WriteLine("Problem Free 7");
                whichPlayer.Units[whichPlayer.Units.Count - 1].SpritePath = dataStringArray[16];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Something went wrong.  Your character was not fully loaded.");
            }
            finally
            {
                myBR.Close();
                Console.WriteLine("Closed.");
            }
            
        }
        private void ImportAbility(string AbilityFileName, Player whichPlayer)
        {
            BinaryReader myBR = null;
            try
            {
                myBR = new BinaryReader(File.Open(AbilityFileName, FileMode.Open));
                char[] delims = { ',' };
                String dataString = System.Text.Encoding.ASCII.GetString(myBR.ReadBytes((int)myBR.BaseStream.Length));
                String[] dataStringArray = dataString.Split(delims);
                int whattype = int.Parse(dataStringArray[7]);//What Type of Ability
                //Attack Ability
                if (whattype == 1)
                {
                    //Check for Buff
                    if (File.Exists(dataStringArray[5]))
                    {
                        BinaryReader newBR = new BinaryReader(File.Open(dataStringArray[5], FileMode.Open));
                        String secondString = System.Text.Encoding.ASCII.GetString(newBR.ReadBytes((int)newBR.BaseStream.Length));
                        String[] secondDataStringArray = secondString.Split(delims);
                        String[] thirdDataStringArray = secondDataStringArray;
                        String[] fourthDataStringArray = secondDataStringArray;
                        BuffType thistype = (BuffType)int.Parse(secondDataStringArray[2]);
                        List<Buff> BuffList = new List<Buff>();
                        BuffList.Add(new Buff(thistype, int.Parse(secondDataStringArray[3]), bool.Parse(secondDataStringArray[5]), int.Parse(secondDataStringArray[4])));
                        if (File.Exists(dataStringArray[dataStringArray.Length - 2]))
                        {
                            BinaryReader secondBR = new BinaryReader(File.Open(dataStringArray[dataStringArray.Length - 2], FileMode.Open));
                            String thirdString = System.Text.Encoding.ASCII.GetString(secondBR.ReadBytes((int)secondBR.BaseStream.Length));
                            thirdDataStringArray = thirdString.Split(delims);
                            thistype = (BuffType)int.Parse(thirdDataStringArray[2]);
                            BuffList.Add(new Buff(thistype, int.Parse(thirdDataStringArray[3]), bool.Parse(thirdDataStringArray[5]), int.Parse(thirdDataStringArray[4])));
                            secondBR.Close();
                        }
                        if (File.Exists(dataStringArray[dataStringArray.Length - 1]))
                        {
                            BinaryReader thirdBR = new BinaryReader(File.Open(dataStringArray[dataStringArray.Length - 1], FileMode.Open));
                            String fourthString = System.Text.Encoding.ASCII.GetString(thirdBR.ReadBytes((int)thirdBR.BaseStream.Length));
                            fourthDataStringArray=fourthString.Split(delims);
                            thistype = (BuffType)int.Parse(fourthDataStringArray[2]);
                            BuffList.Add(new Buff(thistype, int.Parse(fourthDataStringArray[3]), bool.Parse(fourthDataStringArray[5]), int.Parse(fourthDataStringArray[4])));
                            thirdBR.Close();
                        }
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Add(new AttackAbility(int.Parse(dataStringArray[8]), int.Parse(dataStringArray[3]), int.Parse(dataStringArray[4]), int.Parse(dataStringArray[2]), BuffList));
                        Console.WriteLine("Where");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[0].Title = secondDataStringArray[0];
                        Console.WriteLine("It");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[0].Tooltip = secondDataStringArray[1];                        
                        Console.WriteLine("Is");
                        if (thirdDataStringArray.Equals(secondDataStringArray)==false)
                        {
                            Console.WriteLine("Is this happening?");
                            whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[1].Title = thirdDataStringArray[0];
                            Console.WriteLine("Yes it is.");
                            whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[1].Tooltip = thirdDataStringArray[1];
                        }
                        if (fourthDataStringArray.Equals(secondDataStringArray) == false)
                        {
                            whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[2].Title = fourthDataStringArray[0];
                            whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[2].Tooltip = fourthDataStringArray[1];
                        }
                        Console.WriteLine("On");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Title = dataStringArray[0];
                        Console.WriteLine("Going");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Tooltip = dataStringArray[1];
                        newBR.Close();
                    }
                    else
                    {
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Add(new AttackAbility(int.Parse(dataStringArray[8]), int.Parse(dataStringArray[3]), int.Parse(dataStringArray[4]), int.Parse(dataStringArray[2])));
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Title = dataStringArray[0];
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Tooltip = dataStringArray[1];
                    }
                }
                //Passive Ability
                else if (whattype == 2)
                {
                    if (File.Exists(dataStringArray[5]))
                    {
                        BinaryReader newBR = new BinaryReader(File.Open(dataStringArray[5], FileMode.Open));
                        String secondString = System.Text.Encoding.ASCII.GetString(newBR.ReadBytes((int)newBR.BaseStream.Length));
                        String[] secondDataStringArray = secondString.Split(delims);
                        BuffType thistype = (BuffType)int.Parse(secondDataStringArray[2]);
                        Console.WriteLine("Is");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Add(new PassiveAbility(new Buff(thistype, int.Parse(secondDataStringArray[3]), bool.Parse(secondDataStringArray[5]), int.Parse(secondDataStringArray[4])), int.Parse(dataStringArray[4])));
                        Console.WriteLine("This");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[0].Title = secondDataStringArray[0];
                        Console.WriteLine("It?");
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AppliedBuffs[0].Tooltip = secondDataStringArray[1];
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Title = dataStringArray[0];
                        whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Tooltip = dataStringArray[1];
                        newBR.Close();
                    }
                }
                //Teleport Ability
                else if (whattype == 3)
                {

                    whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Add(new TeleportAbility(int.Parse(dataStringArray[3]), int.Parse(dataStringArray[4]), int.Parse(dataStringArray[8]), int.Parse(dataStringArray[2])));
                    whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Title = dataStringArray[0];
                    whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].Tooltip = dataStringArray[1];
                }
                //Adds Picture
                if (File.Exists(dataStringArray[6]))
                {
                    whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities[whichPlayer.Units[whichPlayer.Units.Count - 1].Abilities.Count - 1].AbTexturePath = dataStringArray[6];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Something went wrong.  Your ability is not fully loaded.");
            }
            finally
            {
                myBR.Close();
            }
        }
    }
}
