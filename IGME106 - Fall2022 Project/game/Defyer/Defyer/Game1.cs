using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Defyer
{
    enum GameStates
    {
        mainMenu,
        Gameplay,
        Pause,
        gameOver
    }
    public class Game1 : Game
    {
        Random rng = new Random();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player mc;
        private List<Enemy> enemies = new List<Enemy>();
        private Rectangle player;
        private Rectangle enemy;
        private Texture2D mcSprite;
        private Texture2D badGuy;
        private Texture2D background;
        private Texture2D wall;
        private int counter = 0;
        
        private int height;
        private int width;
        //fields for gamestate
        GameStates gs = new GameStates();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //initialize game state as menu
            gs = GameStates.mainMenu;
            mcSprite = Content.Load<Texture2D>("sPlayerIdle_strip1");
            badGuy = Content.Load<Texture2D>("sEnemy1");
            background = Content.Load<Texture2D>("sMap");
            wall = Content.Load<Texture2D>("sWall");
            width = 600;
            height = 600;
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            player = new Rectangle(width/2, height / 2,mcSprite.Width,mcSprite.Height);
            mc = new Player(player, mcSprite, width, height);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //fsm
            switch(gs)
            {
                case GameStates.mainMenu:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        gs = GameStates.Gameplay;
                    }
                    break;

                case GameStates.Gameplay:
                    mc.Update(gameTime, new Rectangle(mc.Position.X, mc.Position.Y,mcSprite.Width,mcSprite.Height));
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) ||
                        Keyboard.GetState().IsKeyDown(Keys.Down) ||
                        Keyboard.GetState().IsKeyDown(Keys.Left) ||
                        Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        PlayerAnimate();
                    }
                    if (counter == 300)
                    {
                        switch (rng.Next(1,3))
                        {
                            case 1:
                                enemies.Add(new Enemy(
                           new Rectangle(rng.Next(600), 0, badGuy.Width, badGuy.Height)
                           , badGuy, true));
                                break;
                            case 2:
                                enemies.Add(new Enemy(
                           new Rectangle(0, rng.Next(600), badGuy.Width, badGuy.Height)
                           , badGuy, true));
                                break;

                        }
                       
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update(gameTime, new Rectangle(mc.Position.X, mc.Position.Y,badGuy.Width,badGuy.Height));
                        EnemyAnimate(i);
                        if (enemies[i].CheckCollision(mc) && counter%10 == 0)
                        {
                            mc.Health -= 1;
                        }
                    }
                    if(mc.Health <= 0)
                    {
                        gs = GameStates.gameOver;
                    }
                    
                        break;

                case GameStates.Pause:
                    break;

                case GameStates.gameOver:
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //TODO: Change background 
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //fsm
            switch (gs)
            {
                case GameStates.mainMenu:
                    break;

                case GameStates.Gameplay:
                    _spriteBatch.Draw(background, new Rectangle(0,0,width, height),Color.White);
                    _spriteBatch.Draw(wall, new Rectangle(0, 0, width, height), Color.White);
                    mc.Draw(_spriteBatch);
                    for(int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Draw(_spriteBatch);
                    }

                    break;

                case GameStates.Pause:
                    break;

                case GameStates.gameOver:
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        public void PlayerAnimate()
        {
            if (counter % 70 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun7");
            }
            else if (counter % 60 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun6");
            }
            else if (counter % 50 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun5");
            }
            else if(counter % 40 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun4");
            }
            else if (counter % 30 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun3");
            }
            else if (counter % 20 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun2");
            }
            else if (counter % 10 == 0)
            {
                mc.Text = Content.Load<Texture2D>("sPlayerRun1");
            }
        }
        public void EnemyAnimate(int i)
        {
            if (counter % 70 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy7");
            }
            else if (counter % 60 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy6");
            }
            else if (counter % 50 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy5");
            }
            else if (counter % 40 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy4");
            }
            else if (counter % 30 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy3");
            }
            else if (counter % 20 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy2");
            }
            else if (counter % 10 == 0)
            {
                enemies[i].EnemySprite = Content.Load<Texture2D>("sEnemy1");
            }
        }
    }
}