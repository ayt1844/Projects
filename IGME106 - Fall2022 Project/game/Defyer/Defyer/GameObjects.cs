using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defyer
{
    abstract internal class GameObject
    {
        protected Rectangle position;
        protected Texture2D texture;
        // this is a abstract class that is used to help make our objects in the game like bowser and the good and bad coins
        protected GameObject(Rectangle pos, Texture2D Text)
        {
            position = pos;
            texture = Text;
        }
        public Rectangle Position
        {
            get { return position; }
        }
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }
        public abstract void Update(GameTime gameTime,Rectangle pos);
    }
}
