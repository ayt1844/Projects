using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CAZAS_Game
{
    /// <summary>
    /// Created By: Christian Koski, Zachary Rubin, Andrew Tark, Andew Broderick, and Shane Olszewski
    /// Assignment: Team_o (CAZAS) Game --Insert Title--
    /// Class: IGME 106 Professor Mesh
    /// Date: 2/15/2021
    /// Purpose: Creates Player object based on GameObject parent class
    /// Errors: None
    /// </summary>

    enum MovementType
    {
        FaceLeft,
        FaceRight,
    }


    class Player : GameObject
    {
        // Fields
        private int windowWidth;
        private int windowHeight;
        private int score;
        private int health;
        private AttackType currentSpell;
        private MovementType currentState;
        private bool hasGravity;
        private bool isJumping;
        private int originalHeight;

        // Proprties
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int Health
        {
            get { return health; }
            set 
            { 
                if(health < 0)
                {
                    health = 0;
                }
                else 
                { 
                    health = value; 
                }
            }
        }
        public bool HasGravity
        {
            get { return hasGravity; }
            set { hasGravity = value; }
        }

        public AttackType CurrentSpell
        {
            get { return currentSpell; }
            set { currentSpell = value; }
        }

        public MovementType CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        // Player constructor
        public Player(Texture2D asset, Rectangle position, int width, int height, int health, MovementType startingState) : base(asset, position)
        {
            this.asset = asset;
            this.position = position;
            windowWidth = width;
            windowHeight = height;
            this.health = health;
            this.currentState = startingState;
            hasGravity = true;
            isJumping = false;
        }

        // Methods
        public override void Update(GameTime gameTime, Rectangle playerLoc)
        {
            // Get kbState, create new Rectangle to struct, declare speed field
            KeyboardState kbState = Keyboard.GetState();
            Rectangle loc = position;

            if (position.Y < windowHeight - asset.Height / 4 && hasGravity)
            {
                // Gravity
                loc.Y += 5;
            }
            //if (kbState.IsKeyDown(Keys.W) && !hasGravity)
            //{
            // Jump
            //starting jump
            if (kbState.IsKeyDown(Keys.W) && !hasGravity && !isJumping)
            {
                originalHeight = loc.Y; //remember current y position
                isJumping = true; //activate jump mode
                hasGravity = false;
            }
            //If already jumping, increase more unti you're 200 units above where you started, then deactivate jump mode
            if (isJumping)
            {
                loc.Y -= 10;
                if (loc.Y <= originalHeight - 200) //if jump has reached its limit
                {
                    isJumping = false;
                    hasGravity = true;
                }
            }
               
            if (kbState.IsKeyDown(Keys.S))
            {
                // Crouch
                // Movement necessary?
            }
            if (kbState.IsKeyDown(Keys.A) && position.X > 0)
            {
                // Moving left
                currentState = MovementType.FaceLeft;
                loc.X -= 3;
            }
            if (kbState.IsKeyDown(Keys.D) && position.X < windowWidth - position.Width)
            {
                // Moving right
                currentState = MovementType.FaceRight;
                loc.X += 3;
            }

            // Update Position after kbState logic is complete
            position = loc;
        }

        public void Center()
        {
            // Resets the player's location to the center
            Rectangle loc = position;
            loc.X = (windowWidth - asset.Width / 4) / 2;
            loc.Y = (windowHeight - asset.Height/ 4) - 75;
            position = loc;
        }

        public override void Draw(SpriteBatch sb)
        {
            switch (currentState)
            {
                case MovementType.FaceLeft:
                    DrawStanding(SpriteEffects.None, sb);
                    break;

                case MovementType.FaceRight:
                    DrawStanding(SpriteEffects.FlipHorizontally, sb);
                    break;

                default:
                    break;
            }
            //sb.Draw(asset, position, Color.White);
        }

        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch sb)
        {
            sb.Draw(asset, position, new Rectangle(0, 0, asset.Width, asset.Height), Color.White, 0, Vector2.Zero, flipSprite, 0);        
        }
    }
}
