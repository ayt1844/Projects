using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CAZAS_Game
{
    /// <summary>
    /// Created By: Christian Koski, Zachary Rubin, Andrew Tark, Andew Broderick, and Shane Olszewski
    /// Assignment: Team_o (CAZAS) Game "Ala-CAZAS"
    /// Class: IGME 106 Professor Mesh
    /// Date: 2/15/2021
    /// Purpose: Contains methods necessary to generate a MonoGame application
    /// Errors: None
    /// TODO: Improve collision mechanics, add correct assets, add more wizard spells and display for current spell, add buttons?
    /// </summary>
    

    // ERRORS: 
    // - cooldown on buttons is not working
    // - all magic cooldowns are still linked together


    // Game State enum
    enum GameState
    {
        TitleScreen,
        MainMenu,
        Options,
        Pause,
        Game,
        GameOver,
        Win,
        BetweenLevels
    }

    public class Game1 : Game
    {
        // FIELDS

        // current game state
        private GameState currentState = GameState.TitleScreen;

        // game objects (player instance and lists for other objects) 
        private Player player;
        private List<Enemy> enemies;
        private List<Magic> allMagic;

        // kb and m states
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private MouseState mouseState;
        private MouseState prevMouseState;

        // HUD elements 
        private int currentLevel;
        private int maxLevel;
        private Random rng = new Random();
        private string text;
        private double timer;
        private SpriteFont font;
        private Color menuColor;
        private Color menuTextColor;
        private int betweenLevelTimer;
        private string[] messages;
        private int msgNum;

        // Textures
        private Texture2D background;
        private Texture2D menu;
        private Texture2D playerAsset;
        private Texture2D enemyAsset;
        //
        private Texture2D flyingEnemyAsset;
        //
        private Texture2D magicFireAsset;
        private Texture2D magicEnergyAsset;
        private Texture2D magicWindAsset;
        private Texture2D magicEarthAsset;
        private Texture2D magicVoidAsset;

        // magic elements
        private Magic fireBall;
        private Magic energyShield;
        private Magic windTornado;
        private Magic earthVines;
        private Magic voidPortal;

        // position tracking
        private int windowWidth;
        private int windowHeight;
        private int mouseX;
        private int mouseY;

        // buttons and tiles
        private Texture2D tileAsset;
        private List<SpellButton> spellWheelButtonList;
        private List<String> spellList;
        private string tempValue;
        private List<double> spellCooldownTrackerList;
        private double tempDouble;
        private Tile[,] tileSet;

        // MonoGame fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Initialize fields
            windowWidth = _graphics.GraphicsDevice.Viewport.Width;
            windowHeight = _graphics.GraphicsDevice.Viewport.Height;
            background = Content.Load<Texture2D>("Background3");
            menu = Content.Load<Texture2D>("Menu3");
            playerAsset = Content.Load<Texture2D>("Wizard_new");
            player = new Player(playerAsset, new Rectangle((windowWidth - playerAsset.Width / 4) / 2, windowHeight - playerAsset.Height / 4, playerAsset.Width / 4, playerAsset.Height / 4), windowWidth, windowHeight, 100, MovementType.FaceLeft);
            enemyAsset = Content.Load<Texture2D>("Robot_c_Placeholder");
            
            flyingEnemyAsset = Content.Load<Texture2D>("FlyingRobot_Placeholder");
            
            enemies = new List<Enemy>();
            magicFireAsset = Content.Load<Texture2D>("Fireball_Placeholder");
            magicEnergyAsset = Content.Load<Texture2D>("Energy_Placeholder");
            magicWindAsset = Content.Load<Texture2D>("Wind_Placeholder");
            magicEarthAsset = Content.Load<Texture2D>("Earth_Placeholder");
            magicVoidAsset = Content.Load<Texture2D>("Void_Placeholder");
            allMagic = new List<Magic>();
            font = Content.Load<SpriteFont>("arial12");
            menuColor = Color.Orange;
            menuTextColor = Color.White;
            tileAsset = Content.Load<Texture2D>("Platform_Final");
            
            spellWheelButtonList = new List<SpellButton>();
            spellList = new List<string>(new string[] { "Earth", "Void", "Fire", "Energy", "Wind" }); //Probabbly a better way to handle these but whatever
            spellCooldownTrackerList = new List<double>(new double[] { 0,0,0,0,0 });

            //Buttons for spell wheel
            for (int i = 0; i < 5; i++)
            {
                int squareDimension = 80;
                int verticalSpacing = 15 + (90 * i);
                int horizontalSpacing = windowWidth - 95;
                
                if (i == 2) //Make the middle square bigger to show that it is currently highlighted, eventually ill put a border around it too or something
                {
                    squareDimension = 90;
                    horizontalSpacing = windowWidth - 100;
                } 
                else if (i >= 3) //After big square we need to do spacing differently
                {
                    verticalSpacing = 25 + (90 * i);
                }

                int cooldownTime = 100; //THIS IS A TEST 

                spellWheelButtonList.Add(new SpellButton(
                        _graphics.GraphicsDevice,
                        new Rectangle(horizontalSpacing, verticalSpacing, squareDimension, squareDimension),
                        String.Format(spellList[i]),
                        font,
                        menuColor,
                        cooldownTime));
            }

            // obtain between-level messages from the text file
            messages = GetMessages("..\\..\\..\\..\\..\\data_files\\messages.txt");

            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i].Contains('_'))
                {
                    int tempLength = messages[i].IndexOf('_');
                    messages[i] = messages[i].Substring(0, tempLength) + "\n" + messages[i].Substring(tempLength + 1);  ;
                }
            }
                

            // initialize the currentLevel to 1
            currentLevel = 1;
            // initialize the maxLevel to whatever the final level's value is
            maxLevel = 5;
            // load in the first tileSet from a file (after the first level loads in, NextLevel() will take care of this)
            tileSet = LevelFromFile($"..\\..\\..\\..\\..\\data_files\\level{currentLevel}.txt");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Get Keyboard and Mouse States
            kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // Get mouse position
            mouseX = mouseState.Position.X;
            mouseY = mouseState.Position.Y;

            // Switch for current GameState
            switch (currentState)
            {
                case GameState.TitleScreen:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        // If any key is pressed -> menu
                        currentState = GameState.MainMenu;
                    }
                    break;

                case GameState.MainMenu:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        // If enter key is pressed -> game
                        ResetGame();
                        currentState = GameState.Game;
                    }
                    else if (SingleKeyPress(Keys.O))
                    {
                        // If o key is pressed -> options
                        currentState = GameState.Options;
                    }
                    break;

                case GameState.Options:
                    if (SingleKeyPress(Keys.Back))
                    {
                        // If backsapce key is pressed -> menu
                        currentState = GameState.MainMenu;
                    }
                    break;

                case GameState.Pause:
                    if (SingleKeyPress(Keys.P))
                    {
                        // If p key is pressed -> game
                        currentState = GameState.Game;
                    }
                    else if (SingleKeyPress(Keys.Q))
                    {
                        // If q key is pressed -> menu
                        currentState = GameState.MainMenu;
                    }
                    break;

                case GameState.Game:
                    // Increment timer
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (enemies.Count == 0)
                    {
                        // checks if the next level can be called
                        if(currentLevel < maxLevel)
                        {
                            // Calls next level if no enemies remain
                            NextLevel();
                        }
                        else
                        {
                            // if the final level has been cleared, move to the Win state
                            currentState = GameState.Win;
                        }
                    }

                    // Updates player
                    if (IsOnGround(tileSet, player))
                    {
                        player.HasGravity = false;
                    }
                    else
                    {
                        player.HasGravity = true;
                    }
                    player.Update(gameTime, player.Position);

                    // Updates player's spell
                    if (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue != 0)
                    {
                        //SPELL WHEEL VISUAL SCROLLING LOGIC
                        if (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue <= -25)
                        {  
                            //Reorder spell list
                            tempValue = spellList[0];
                            spellList.RemoveAt(0);
                            spellList.Add(tempValue);

                            //reorder spell cooldown list
                            tempDouble = spellCooldownTrackerList[0];
                            spellCooldownTrackerList.RemoveAt(0);
                            spellCooldownTrackerList.Insert(spellCooldownTrackerList.Count, tempDouble);

                            //Update the actual buttons
                            for (int i = 0; i < 5; i++)
                            {
                                spellWheelButtonList[i].UpdateButtonInfo(spellList[i], spellCooldownTrackerList[i]);
                            }
                          
                            //Update the current spell value
                            player.CurrentSpell++;
                            if (player.CurrentSpell > (AttackType)4)
                            {
                                player.CurrentSpell = 0;
                            }
                        }
                        if (mouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue >= 25)
                        {
                            //Reorder spell list
                            tempValue = spellList[spellList.Count - 1];
                            spellList.RemoveAt(spellList.Count - 1);
                            spellList.Insert(0, tempValue);

                            //Reorder spell cooldown list
                            tempDouble = spellCooldownTrackerList[spellCooldownTrackerList.Count - 1];
                            spellCooldownTrackerList.RemoveAt(spellCooldownTrackerList.Count - 1);
                            spellCooldownTrackerList.Insert(0, tempDouble);

                            //Update the actual buttons
                            for (int i = 0; i < 5; i++)
                            {
                                spellWheelButtonList[i].UpdateButtonInfo(spellList[i], spellCooldownTrackerList[i]);
                            }

                            //Update the current spell value
                            player.CurrentSpell--;
                            if (player.CurrentSpell < 0)
                            {
                                player.CurrentSpell = (AttackType)4;
                            }
                        }

                        if (player.CurrentSpell != AttackType.Energy)
                        {
                            allMagic.RemoveAll(eachSpell => eachSpell == energyShield);
                        }

                        if (player.CurrentSpell != AttackType.Void)
                        {
                            allMagic.RemoveAll(eachSpell => eachSpell == voidPortal);
                        }
                    }

                    const int key = 2;

                    // Will need a switch statement for each spell
                    switch (player.CurrentSpell)
                    {
                        case AttackType.Fire:
                            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && spellCooldownTrackerList[key] < 70)
                            {
                                // create fireball
                                fireBall = new Magic(magicFireAsset, new Rectangle(mouseX, 0, (playerAsset.Width / 4) + 25, playerAsset.Height / 4), mouseX, mouseY, true, AttackType.Fire);
                                allMagic.Add(fireBall);
                                spellCooldownTrackerList[key] += 35;
                            }
                            break;

                        case AttackType.Energy:
                            if (!allMagic.Contains(energyShield) && spellCooldownTrackerList[key] < 70)
                            {
                                // create energy shield
                                energyShield = new Magic(magicEnergyAsset, player.Position, 0, 300, true, AttackType.Energy);
                                allMagic.Add(energyShield);
                                spellCooldownTrackerList[key] = -30;
                            }
                            break;

                        case AttackType.Wind:
                            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && spellCooldownTrackerList[key] < 70)
                            {
                                // create wind tornado
                                windTornado = new Magic(magicWindAsset, player.Position, windowWidth, windowHeight, player.CurrentState == MovementType.FaceLeft, AttackType.Wind);
                                allMagic.Add(windTornado);
                                spellCooldownTrackerList[key] += 60;
                            }
                            break;

                        case AttackType.Earth:
                            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && !allMagic.Contains(earthVines) && spellCooldownTrackerList[key] < 70)
                            {
                                // create earth vines
                                Point loc = new Point(mouseX, mouseY);
                                earthVines = new Magic(magicEarthAsset, new Rectangle(loc.X - magicEarthAsset.Width / 4, loc.Y - magicEnergyAsset.Height / 6, magicEarthAsset.Width / 2, magicEarthAsset.Height / 6), windowWidth, windowHeight, true, AttackType.Earth);
                                foreach (Enemy enemy in enemies)
                                {
                                    if (enemy.Position.Contains(loc))
                                    {
                                        allMagic.Add(earthVines);
                                        spellCooldownTrackerList[key] += 100;
                                        enemy.Speed = 1;
                                    }
                                }
                            }
                            break;

                        case AttackType.Void:
                            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed && spellCooldownTrackerList[key] < 70)
                            {
                                // create void portal
                                if (mouseX > 50 && mouseX < windowWidth - 50 && mouseY > 50 && mouseY < windowHeight - 50)
                                {
                                    voidPortal = new Magic(magicVoidAsset, new Rectangle(mouseX, mouseY, playerAsset.Width / 4, playerAsset.Height / 4), 0, 100, false, AttackType.Void);
                                    allMagic.Add(voidPortal);
                                    voidPortal = new Magic(magicVoidAsset, new Rectangle(player.Position.X, player.Position.Y, playerAsset.Width / 4, playerAsset.Height / 4), 0, 100, false, AttackType.Void);
                                    allMagic.Add(voidPortal);
                                    Rectangle loc = player.Position;
                                    loc.X = mouseX;
                                    loc.Y = mouseY;
                                    player.Position = loc;
                                    spellCooldownTrackerList[key] += 150;
                                }
                            }
                            else if (spellCooldownTrackerList[key] < 70)
                            {
                                allMagic.RemoveAll(eachSpell => eachSpell.MyAttack == AttackType.Void);
                            }
                            break;

                        default:
                            break;
                    }

                    // testing new cooldowns

                    for (int i = 0; i < spellCooldownTrackerList.Count; i++)
                    {
                        if (spellCooldownTrackerList[i] > 0)
                        {
                            spellCooldownTrackerList[i] -= 0.3;
                        }
                    }
                   
                    foreach (Magic spell in allMagic)
                    {
                        // Updates each spell and checks for collisions with enemies
                        spell.Update(gameTime, player.Position);

                        foreach (Enemy enemy in enemies)
                        {
                            // Checks for collisions with spell
                            if (enemy.CheckCollision(spell) == true)
                            {
                                // Collision is true, enemy loses health
                                enemy.Health -= spell.Damage;

                                if (spell == energyShield)
                                {
                                    spell.XNum += spell.Damage;
                                    // Shield will break
                                    if (spell.XNum >= spell.YNum)
                                    {
                                        spell.IsBool = false;
                                        spellCooldownTrackerList[3] += 200;
                                    }
                                }

                                if (spell == windTornado)
                                {
                                    Rectangle loc = enemy.Position;
                                    if (windTornado.IsBool)
                                    {
                                        loc.X -= 2;
                                    }
                                    else
                                    {
                                        loc.X += 2;
                                    }
                                    loc.Y -= 2;
                                    enemy.Position = loc;
                                }
                                
                                if (spell == earthVines)
                                {
                                    spell.Position = enemy.Position;
                                    if (enemy.Health <= 0)
                                    {
                                        spell.XNum = 1000;
                                    }
                                }
                                
                                if (enemy.Health <= 0)
                                {
                                    player.Score += 100;
                                }
                            }
                        }
                    }
                    foreach (Enemy enemy in enemies)
                    {
                        // Updates each enemy and checks for collisions with player and tiles
                        enemy.Update(gameTime, player.Position);
                        
                        if (enemy.CheckCollision(player) && !allMagic.Contains(energyShield))
                        {
                            // Collision is true, player loses health
                            player.Health--;
                        }
                        // isn't affected by gravity if it's on a tile
                        if(IsOnGround(tileSet, enemy))
                        {
                            enemy.HasGravity = false;
                        }
                        else
                        {
                            enemy.HasGravity = true;
                        }
                    }

                    // Removes all dead enemys
                    enemies.RemoveAll(eachEnemy => eachEnemy.Health <= 0);
                    // Removes all off-screen magic attacks, disabled energy shields, and vines with no host
                    allMagic.RemoveAll(eachSpell => eachSpell.Position.X < 0 || eachSpell.Position.X > windowWidth || eachSpell.Position.Y < 0 || eachSpell.Position.Y > windowHeight);
                    allMagic.RemoveAll(eachSpell => eachSpell.MyAttack == AttackType.Energy && eachSpell.IsBool == false);
                    allMagic.RemoveAll(eachSpell => eachSpell.MyAttack == AttackType.Earth && eachSpell.XNum >= 1000);
                    if (SingleKeyPress(Keys.P))
                    {
                        // If p key is pressed -> pause
                        currentState = GameState.Pause;
                    }
                    if (enemies.Count == 0)
                    {
                        betweenLevelTimer = 240;
                        // Draws a random message from an array (this should only run once for each "between" screen, so it goes here instead of in Draw().)
                        msgNum = rng.Next(0, messages.Length);
                        currentState = GameState.BetweenLevels;
                    }
                    if (player.Health <= 0)
                    {
                        // Removes enemys and changes to game over state
                        enemies.Clear();
                        currentState = GameState.GameOver;
                    }
                    break;

                case GameState.GameOver:
                case GameState.Win:
                    if (SingleKeyPress(Keys.M))
                    {
                        // If m key is pressed -> menu
                        currentState = GameState.MainMenu;
                    }
                    break;

                case GameState.BetweenLevels:
                    // wait for a bit before moving on to the next level
                    if(betweenLevelTimer >= 0)
                    {
                        betweenLevelTimer--;
                    }
                    // once the "timer" runs out, move on to the next level
                    else
                    {
                        NextLevel();
                        currentState = GameState.Game;
                    }
                    break;

                default:
                    break;
            }

            // Update previous Keyboard and Mouse States
            prevKbState = kbState;
            prevMouseState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Begin spriteBatch
            _spriteBatch.Begin();

            _spriteBatch.Draw(background, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

            if (currentState != GameState.Game)
            {
                _spriteBatch.Draw(menu, new Rectangle((windowWidth - menu.Width)/2, (windowHeight - menu.Height)/2, menu.Width, menu.Height), Color.White);
            }

            // Switch for current GameState
            switch (currentState)
            {
                case GameState.TitleScreen:
                    // Draws title display
                    text = "Welcome to Ala-CAZAS!\nPress any key to continue...";
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X)/2, (windowHeight - font.MeasureString(text).Y)/2), menuTextColor);
                    break;

                case GameState.MainMenu:
                    // Draws menu display
                    text = "Rules: Use a variety of spells \nin addition to" +
                        "and movement options to " +
                        "\ndestroy the enemies and win.\n\nPress Enter to start the game.\n\nPress o for the Options Menu";
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X) / 2, (windowHeight - font.MeasureString(text).Y) / 2), menuTextColor);
                    break;

                case GameState.Options:
                    // Draws options display
                    text = "W - Jump" +
                        "\nA,D - Move Left & Right" +
                        "\nScroll Wheel - Rotate through spells" +
                        "\nLeft Mouse Button - Activate spell" +
                        "\nP - Pause Menu\n\nPress Backspace to return to the Main Menu";
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X) / 2, (windowHeight - font.MeasureString(text).Y) / 2), menuTextColor);
                    break;

                case GameState.Pause:
                    // Draws pause display
                    text = "GAME PAUSED\n\nPress p to Unpause.\n\nPress q to Quit";
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X) / 2, (windowHeight - font.MeasureString(text).Y) / 2), menuTextColor);
                    break;

                case GameState.Game:
                    //GAMEPLAY DRAWING
                    // Draws player, enemies, magic, score, etc.
                    foreach (Tile tile in tileSet)
                    {
                        if (!tile.IsEmpty)
                        {
                            tile.Draw(_spriteBatch);
                        }
                    }
                    player.Draw(_spriteBatch);
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(_spriteBatch);
                    }
                    foreach (Magic spell in allMagic)
                    {
                        spell.Draw(_spriteBatch);
                    }
                    

                    //HUD DRAWING 
                    _spriteBatch.DrawString(font, "Score: " + player.Score, new Vector2(10, 5), Color.White);
                    
                    _spriteBatch.DrawString(font, "Health " + player.Health, new Vector2(210, 5), Color.White);
                    
                    _spriteBatch.DrawString(font, "Time: " + string.Format("{0:0.00}", timer), new Vector2(410, 5), Color.White);
                    _spriteBatch.DrawString(font, "Level: " + currentLevel, new Vector2(610, 5), Color.White);

                    //Spell Wheel HUD Drawing
                    foreach (SpellButton b in spellWheelButtonList)
                    {
                        b.Draw(_spriteBatch);
                    }


                    break;

                case GameState.GameOver:
                    // Draws game over display
                    text = "GAME OVER\n\nPress m to return to the Main Menu";
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X) / 2, (windowHeight - font.MeasureString(text).Y) / 2), menuTextColor);
                    break;

                case GameState.Win:
                    // Draws game over display
                    text = "YOU WON!\n\nPress m to return to the Main Menu";
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X) / 2, (windowHeight - font.MeasureString(text).Y) / 2), menuTextColor);
                    break;

                case GameState.BetweenLevels:
                    text = messages[msgNum];
                    _spriteBatch.DrawString(font, text, new Vector2((windowWidth - font.MeasureString(text).X) / 2, (windowHeight - font.MeasureString(text).Y) / 2), menuTextColor);
                    break;

                default:
                    break;
            }

            // End spriteBatch
            _spriteBatch.End();


            //ShapeBatch Section, need to do this in its own switch statement I think because its weird and uses shapebatch isnstead of spritebatch
            ShapeBatch.Begin(GraphicsDevice);

            switch (currentState)
            {
                case GameState.Game:
                    //Draw cooldown guages with updated cooldown timer
                    
                    for (int i = 0; i < 5; i++)
                    {
                        
                        
                        spellWheelButtonList[i].DrawShapes(spellCooldownTrackerList[i]);
                    }
                    
                    break;

            }
            
            

            ShapeBatch.End();

            base.Draw(gameTime);
        }

        private void NextLevel()
        {
            // Moves to the next level
            currentLevel++;

            // load in the tileSet from a file
            tileSet = LevelFromFile($"..\\..\\..\\..\\..\\data_files\\level{currentLevel}.txt");

            // reset gameplay data
            player.Health = 100;
            for (int i = 0; i < spellCooldownTrackerList.Count; i++)
            {
                spellCooldownTrackerList[i] = 0;
            }
            enemies.Clear();
            for (int i = 0; i < 1 + currentLevel; i++) // Testing enemies
            {
                enemies.Add(new Enemy(enemyAsset, new Rectangle((0 - enemyAsset.Width / 6) / 2, windowHeight - 125 - enemyAsset.Height / 6, enemyAsset.Width / 6, enemyAsset.Height / 6), windowWidth, windowHeight, 200 + rng.Next(101), rng.Next(1, 6), false));
                if (i > 0)
                {
                    enemies.Add(new Enemy(flyingEnemyAsset, new Rectangle((0 - enemyAsset.Width / 6) / 2, 50, flyingEnemyAsset.Width / 6, flyingEnemyAsset.Height / 6), windowWidth, windowHeight, 100 + rng.Next(101), 4, true));
                }
            }
            allMagic.Clear();
        }

        private void ResetGame()
        {
            timer = 0;
            currentLevel = 0;
            player.Score = 0;
            player.Center();
            enemies.Clear();
            allMagic.Clear();
        }

        private bool SingleKeyPress(Keys key)
        {
            // Checks for a single key press
            if (kbState.IsKeyUp(key) && prevKbState.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Tile[,] LevelFromFile(string fileName) 
        {
            // loads in level data from a .txt file
            // (see levelFileTemplate.txt for how to format text files)
            string input = "";
            string[] widthHeight = { "", "" };
            string[,] stringLayout = new string[0,0];
            Tile[,] layout;

            StreamReader reader = null;
            try
            {
                reader = new StreamReader(fileName);
                // reads in the first line, then stores it as the width and height
                input = reader.ReadLine();
                widthHeight = input.Trim().Split(',');

                // creates a 2d string array to hold the read-in data
                stringLayout = new string[int.Parse(widthHeight[0]), int.Parse(widthHeight[1])];
                // fills in that array as it reads in the file data
                string[] data = new string[int.Parse(widthHeight[1])];
                
                // read in the rest of the file and store it in the data array,
                // where array indices correspond to line numbers, and the first line is line #0
                for (int i = 0; i < int.Parse(widthHeight[1]); i++)
                {
                    data[i] = reader.ReadLine();
                }

                // populate the string layout arraythe data array
                for (int i = 0; i < int.Parse(widthHeight[1]); i++)
                {
                    for (int j = 0; j < int.Parse(widthHeight[0]); j++)
                    {
                        stringLayout[j, i] = data[i][j].ToString();
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Issue reading the file:" + e.Message);
            }

            finally 
            {
                reader.Close();

                // makes the level layout 2D array using the width and height data
                layout = new Tile[int.Parse(widthHeight[0]), int.Parse(widthHeight[1])];
                // position variables that will update as it moves through the array
                int yPos = 0;
                int xPos = 0;
                // change dimension to update the size of the tile
                // tiles are squares, so we only need 1 variable here.
                int dimension = 40;

                // fill in the array
                for (int i = 0; i < int.Parse(widthHeight[1]); i++)
                {
                    for (int j = 0; j < int.Parse(widthHeight[0]); j++)
                    {
                        bool isEmpty = true;
                        if (stringLayout[j,i] == ".")
                        {
                            isEmpty = true;
                        }
                        else { isEmpty = false; }
                        layout[j,i] = new Tile(isEmpty, tileAsset, new Rectangle(xPos, yPos, dimension, dimension));
                        xPos += dimension;
                    }
                    xPos = 0;
                    yPos += dimension;
                }
            }

            // returns the level tile layout array
            return layout;
        }

        private bool IsOnGround(Tile[,] tileSet, GameObject thing)
        {
            // Checks if the player is standing on a tile; call whenever player is updated.
            // check for collisions with tiles
            bool isOnTile = false;
            foreach (Tile tile in tileSet)
            {
                if (tile.CheckCollision(thing))
                {
                    isOnTile = true;
                    break;
                }
            }
            return isOnTile;
        }

        private string[] GetMessages(string fileName)
        {
            // Returns an array of string messages to be displayed randomly between levels
            int size = 0;
            bool linesRemain = true;
            string line;
            // temporary list so that the file only has to be read through once and doesn't need to include a line count
            List<string> tempList = new List<string>(0);

            StreamReader reader = null;
            try
            {
                reader = new StreamReader(fileName);
                while (linesRemain != false)
                {
                    line = reader.ReadLine();
                    if(line == null)
                    {
                        linesRemain = false;
                    }
                    else
                    {
                        size++;
                        tempList.Add(line);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Issue reading the file:" + e.Message);
            }
            finally
            {
                reader.Close();

                // make and populate the array using retrieved data
                messages = new string[size];
                for(int i = 0; i < tempList.Count; i++)
                {
                    messages[i] = tempList[i];
                }
            }

            return messages;
        }
    }
}
