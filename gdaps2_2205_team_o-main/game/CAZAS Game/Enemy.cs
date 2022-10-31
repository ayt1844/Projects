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
    /// Purpose: Creates Enemy object based on GameObject parent class
    /// Errors: None
    /// </summary>
    
    class Enemy : GameObject
    {
        // Fields
        private int windowWidth;
        private int windowHeight;
        private int health;
        private int speed;
        private bool hasGravity;
        private bool canFly;

        // Proprties
        public int Health
        {
            get { return health; }
            set
            {
                if (health < 0)
                {
                    health = 0;
                }
                else
                {
                    health = value;
                }
            }
        }

        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public bool HasGravity
        {
            get { return hasGravity; }
            set { hasGravity = value; }
        }

        // Enemy constructor
        public Enemy(Texture2D asset, Rectangle position, int width, int height, int health, int speed, bool canFly) : base(asset, position)
        {
            this.asset = asset;
            this.position = position;
            windowWidth = width;
            windowHeight = height;
            this.health = health;
            this.speed = speed;
            hasGravity = true;
            this.canFly = canFly;
        }

        // Methods
        public override void Draw(SpriteBatch sb)
        {
            // Only Draw alive enemies
            if (Health > 0)
            {
                if (Health > 50)
                {
                    sb.Draw(asset, position, Color.White);
                }
                else
                {
                    sb.Draw(asset, position, Color.Red);
                }
            }
        }

        public override void Update(GameTime gameTime, Rectangle playerLoc)
        {
            // Create new Rectangle to update struct
            Rectangle loc = position;

            if (!canFly)
            {
                loc.X += speed;
                if (position.X > windowWidth)
                {
                    // Wraps from right to left
                    loc.X -= windowWidth + position.Width;
                    loc.Y -= 10;
                }
                if (position.Y < windowHeight - asset.Height / 6 && hasGravity)
                {
                    // Gravity
                    loc.Y += 5;
                }
            }
            else
            {
                loc.X += speed;
                if (Position.X > windowWidth)
                {
                    // Wraps from right to left
                    loc.X -= windowWidth + Position.Width;
                    if (position.Y < windowHeight / 2)
                    {
                        loc.Y += 50;
                    }
                    else
                    {
                        loc.Y -= 200;
                    }
                }
            }

            // Update Position after kbState logic is complete
            position = loc;
        }

        

        public bool CheckCollision(GameObject check)
        {
            // Checks if a passed object intersects an enemy
            return health > 0 && check.Position.Intersects(position);
        }
    }
}
