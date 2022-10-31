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
    /// Purpose: Creates magic spell objects based on GameObject parent class
    /// Errors: None
    /// </summary>
    
    // Enum of possible magic attacks
    enum AttackType
    {
        Fire,
        Energy,
        Wind,
        Earth,
        Void
    }

    class Magic : GameObject
    {
        // Fields
        private int mouseX;
        private int mouseY;
        private int xNum;
        private int yNum;
        private int windowWidth;
        private int windowHeight;
        private bool isBool;
        private int damage;
        private double cooldown;
        private AttackType myAttack;
        private Rectangle initialPosition;
        private Vector2 direction;
        private float speed;

        // Proprties
        public int Damage
        {
            get { return damage; }
        }

        public double Cooldown
        {
            get { return cooldown; }
        }

        public AttackType MyAttack
        {
            get { return myAttack; }
        }

        public bool IsBool
        {
            get { return isBool; }
            set { isBool = value; }
        }
        
        public int XNum
        {
            get { return xNum; }
            set { xNum = value; }
        }

        public int YNum
        {
            get { return yNum; }
        }

        // Magic constructor
        public Magic(Texture2D asset, Rectangle position, int x, int y, bool check, AttackType myAttack) : base(asset, position)
        {
            this.asset = asset;
            this.position = position;
            this.myAttack = myAttack;

            // Switch for types of attacks
            switch (myAttack)
            {
                case AttackType.Fire:
                    damage = 4;
                    mouseX = x;
                    mouseY = y;
                    initialPosition = position;   
                    speed = 3;
                    break;

                case AttackType.Energy:
                    damage = 8;
                    xNum = x;
                    yNum = y;
                    isBool = check;
                    break;

                case AttackType.Wind:
                    damage = 2;
                    windowWidth = x;
                    windowHeight = y;
                    isBool = check;
                    speed = 3;
                    break;

                case AttackType.Earth:
                    damage = 1;
                    windowWidth = x;
                    windowHeight = y;
                    speed = 1;
                    break;

                case AttackType.Void:
                    xNum = x;
                    yNum = y;
                    break;

                default:
                    break;
            }
        }

        // Methods
        public override void Update(GameTime gameTime, Rectangle playerLoc)
        {
            // Create new Rectangle to update struct
            Rectangle loc = position;

            // Switch for types of attacks
            switch (myAttack)
            {
                case AttackType.Fire:
                    loc.Y += (int)speed;
                    break;

                case AttackType.Energy:
                    loc = playerLoc;
                    break;

                case AttackType.Wind:
                    if (position.Y < windowHeight - asset.Height/3)
                    {
                        // Gravity
                        loc.Y += 5;
                    }
                    if (isBool)
                    {
                        loc.X -= (int)speed;
                    }
                    else
                    {
                        loc.X += (int)speed;
                    }
                    break;

                case AttackType.Earth:
                    break;

                case AttackType.Void:
                    break;

                default:
                    break;
            }

            // Update Position after kbState logic is complete
            position = loc;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (xNum > 200 && (myAttack == AttackType.Energy || myAttack == AttackType.Earth))
            {
                sb.Draw(asset, position, Color.Red);
            }
            else if (myAttack == AttackType.Void)
            {
                sb.Draw(asset, position, Color.White * 0.7f);
            }
            else
            {
                sb.Draw(asset, position, Color.White);
            }
            
        }
    }
}
