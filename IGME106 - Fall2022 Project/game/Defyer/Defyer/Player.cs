using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defyer
{
    internal class Player : GameObject
    {
        // score
        private int levelScore;
        private int totalScore = 0;
        private int health = 10;
        // what are the dimenchens
        private int width;
        private int height;
        private Texture2D text;
        // makes the player
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        public Texture2D Text
        {
            get { return Text; }
            set { text = value; }

        }
        public Player(Rectangle pos, Texture2D text, int width, int heigh) : base(pos, text)
        {
            position.X = width / 2;
            position.Y = heigh / 2;
            this.text = text;
        }
        // lets me see and set the scores when needed 
        public int LevelScore
        {
            get { return levelScore; }
            set { levelScore = value; }
        }
        public int TotalScore
        {
            get { return totalScore; }
            set { totalScore = value; }
        }
        // allows the player to move 
        public override void Update(GameTime gameTime,Rectangle player)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                position.Y = position.Y - 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                position.Y = position.Y + 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                position.X = position.X + 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                position.X = position.X - 5;
            }
            if (position.X > 600)
            {
                position.X = 580;
            }
            else if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y > 600)
            {
                position.Y = 580;
            }
            else if (position.Y < 0)
            {
                position.Y = 0;
            }
        }
        // moves them to the ceter
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(text, position, Color.White);
        }
    }
}
