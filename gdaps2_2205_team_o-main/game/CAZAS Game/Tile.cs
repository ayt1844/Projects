using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAZAS_Game
{
    /// <summary>
    /// Class used in setting up levels.
    /// Levels in this game are tile-based and will be read in from text files using the format described in levelFileTemplate.txt
    /// Tiles can either be empty or platform.
    /// When the file is read in, the player/enemies will be listed as empty tiles, and the player and enemies will be generated at the position listed.
    /// </summary>
    
    class Tile : GameObject
    {
        // Fields
        protected bool isEmpty;

        /* NOTES:
         * - all tiles have a Texture2D and Rectangle because they are GameObjects
         * - however, if a tile isEmpty, it will not show up in the game, and it won't have any collision checks.
         * - 
        */

        // Properties
        public bool IsEmpty 
        {
            get { return isEmpty; }
        }

        // Constructor
        public Tile(bool isEmpty, Texture2D asset, Rectangle position) :base(asset, position)
        {
            this.isEmpty = isEmpty;
            this.asset = asset;
            this.position = position;
        }

        // Methods
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(asset, position, Color.White);
        }

        public override string ToString()
        {
            if (IsEmpty) { return "."; }
            else { return "="; }
        }

        public override void Update(GameTime gameTime, Rectangle playerLoc) { } //not sure what to put here yet.

        public bool CheckCollision(GameObject check)
        {
            // if tile isEmpty, no collisions
            /* makes check using a rectangle representing only the top portion of the tile 
               in order to avoid objects appearing "inside" a tile */
            if(!IsEmpty && check.Position.Intersects(new Rectangle(position.X, position.Y,position.Width, position.Height / 100)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
