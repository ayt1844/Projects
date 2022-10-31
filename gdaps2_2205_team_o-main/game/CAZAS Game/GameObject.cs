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
    /// Purpose: Abstract class used to create generic game objects
    /// Errors: None
    /// </summary>

    abstract class GameObject
    {
        // Fields
        protected Texture2D asset;
        protected Rectangle position;

        // Properties
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        // Constructor
        protected GameObject(Texture2D asset, Rectangle position)
        {
            this.asset = asset;
            this.position = position;
        }

        // Methods
        public abstract void Draw(SpriteBatch sb);

        public abstract void Update(GameTime gameTime, Rectangle playerLoc);
    }
}
