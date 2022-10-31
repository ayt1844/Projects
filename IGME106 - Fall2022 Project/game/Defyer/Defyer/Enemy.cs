using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defyer
{
    internal class Enemy : GameObject
    {
        // so they have random movement
        private Texture2D enemySprite;
        private Rectangle pos;
        public Rectangle Pos
        {
            get { return pos; }
            set { pos = value; }
        }
        public Texture2D EnemySprite
        {
            get { return enemySprite; }
            set { enemySprite = value; }
        }
        // makes the enemy
        public Enemy(Rectangle pos, Texture2D text, bool active) : base(pos, text)
        {
            this.pos = pos;
            enemySprite = text;
        }
        // see if it hits the player
        public bool CheckCollision(GameObject check)
        {
            return position.Intersects(check.Position);
        }        
        // so they have random movement
        public override void Update(GameTime gameTime,Rectangle player)
        {
            if (player.X < pos.X)
            {
                pos.X = pos.X - 1;
            }
            else if(player.X > pos.X)
            {
                pos.X = pos.X + 1;
            }
            if (player.Y < pos.Y)
            {
                pos.Y = pos.Y - 1;
            }
            else if(player.Y > pos.Y)
            {
                pos.Y = pos.Y + 1;
            }
            if (pos.X > 600)
            {
                pos.X = 600;
            }
            else if (pos.X < 0)
            {
                pos.X = 0;
            }
            if (pos.Y > 600)
            {
                pos.Y = 600;
            }
            else if (pos.Y < 0)
            {
                pos.Y = 0;
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(enemySprite, pos, Color.White);
        }
    }
}
