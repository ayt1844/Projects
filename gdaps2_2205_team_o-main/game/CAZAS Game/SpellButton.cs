﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CAZAS_Game
{
    //CREDITS TO ERIKA MESH FOR THE BASE CLASS


    /// <summary>
    /// If the client wants to be notified when a button is clicked, it must
    /// implement a method matching this delegate and then tie that method to
    /// the button's "OnButtonClick" event.
    /// </summary>
    public delegate void OnButtonClickDelegate();

    /// <summary>
    /// Builds, monitors, and draws a customized Button
    /// </summary>
    public class SpellButton
    {
        // Button specific fields
        private SpriteFont font;
        private MouseState prevMState;
        private string text;
        private Rectangle position; // Button position and size
        private Vector2 textLoc;
        private Texture2D buttonImg;
        private Color textColor;
        private double cooldownRemaining;
        private Vector2 rechargeGuageLoc;
        private GraphicsDevice device;
        private int guageOutlineLength;

        /// <summary>
        /// If the client wants to be notified when a button is clicked, it must
        /// implement a method matching OnButtonClickDelegate and then tie that method to
        /// the button's "OnButtonClick" event.
        /// 
        /// The delegate will be called with a reference to the clicked button.
        /// </summary>
        public event OnButtonClickDelegate OnLeftButtonClick;

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Adds on Right Button click event
        public event OnButtonClickDelegate OnRightButtonClick;
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Create a new custom button
        /// </summary>
        /// <param name="device">The graphics device for this game - needed to create custom button textures.</param>
        /// <param name="position">Where to draw the button's top left corner</param>
        /// <param name="text">The text to draw on the button</param>
        /// <param name="font">The font to use when drawing the button text.</param>
        /// <param name="color">The color to make the button's texture.</param>
        public SpellButton(GraphicsDevice device, Rectangle position, String text, SpriteFont font, Color color, int cooldownRemaining)
        {
            // Save copies/references to the info we'll need later
            this.font = font;
            this.position = position;
            this.text = text;
            this.cooldownRemaining = cooldownRemaining;
            this.device = device;

            // Figure out where on the button to draw the text label
            Vector2 textSize = font.MeasureString(text);
            textLoc = new Vector2(
                (position.X + position.Width / 2) - textSize.X / 2,
                (position.Y + position.Height / 2) - textSize.Y / 2
            );

            //Figure out where on the button to draw the recharge guage (just under the text)
            guageOutlineLength = 70;
            rechargeGuageLoc = new Vector2((position.X + position.Width / 2) - (guageOutlineLength/2), position.Y + 2*position.Height/3+10);

            // Invert the button color for the text color (because why not)
            textColor = new Color(255 - color.R, 255 - color.G, 255 - color.B);

            // Make a custom 2d texture for the button itself
            buttonImg = new Texture2D(device, position.Width, position.Height, false, SurfaceFormat.Color);
            int[] colorData = new int[buttonImg.Width * buttonImg.Height]; // an array to hold all the pixels of the texture
            Array.Fill<int>(colorData, (int)color.PackedValue); // fill the array with all the same color
            buttonImg.SetData<Int32>(colorData, 0, colorData.Length); // update the texture's data
        }

        /// <summary>
        /// Each frame, update its status if it's been clicked.
        /// </summary>
        public void Update()
        {
            // Check/capture the mouse state regardless of whether this button
            // if active so that it's up to date next time!
            MouseState mState = Mouse.GetState();
            if (mState.LeftButton == ButtonState.Released &&
                prevMState.LeftButton == ButtonState.Pressed &&
                position.Contains(mState.Position))
            {
                if (OnLeftButtonClick != null)
                {
                    // Call ALL methods attached to this button
                    OnLeftButtonClick();
                }
            }

            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // Checks if the right mouse button has been pressed
            if (mState.RightButton == ButtonState.Released &&
                prevMState.RightButton == ButtonState.Pressed &&
                position.Contains(mState.Position))
            {
                if (OnRightButtonClick != null)
                {
                    // Call ALL methods attached to this button
                    OnRightButtonClick();
                }
            }
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            prevMState = mState;
        }

        //Scrolls the list of names across the buttons
        public void UpdateButtonInfo(string text, double cooldownRemaining)
        {
            //Update text of the button
            this.text = text;

            //Update the cooldown remaining
            this.cooldownRemaining = cooldownRemaining;
        }

        /// <summary>
        /// Override the GameObject Draw() to draw the button and then
        /// overlay it with text.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch on which to draw this button. The button 
        /// assumes that Begin() has already been called and End() will be called later.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the button itself
            spriteBatch.Draw(buttonImg, position, Color.White);

            // Draw button text over the button
            spriteBatch.DrawString(font, text, textLoc, textColor);

        }

        public void DrawShapes(double cooldownRemaining)
        {
            //cooldownRemaining = 50;
            Color guageColor = Color.Green;

            //Convert the given cooldown (out of 100) to how long the guage should be (out of 70)
            
            if (cooldownRemaining > 70)
            {
                cooldownRemaining = 70;
                guageColor = Color.Red;
            }
            else if (cooldownRemaining < -20)
            {
                cooldownRemaining = 70;
                guageColor = Color.BlueViolet;
            }
            
            
            ShapeBatch.Box(rechargeGuageLoc.X, rechargeGuageLoc.Y, (float)cooldownRemaining, 10, guageColor);

            //Draw the outline of the recharge guage 
            ShapeBatch.BoxOutline(rechargeGuageLoc.X, rechargeGuageLoc.Y, guageOutlineLength, 10, Color.Black);

        }

    }
}
