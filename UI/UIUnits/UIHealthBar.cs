using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UIHealthBar : UIBox {
        //Save the unit this health bar represents
        public UIUnit reps;

        //The health bar sprite
        public static Texture2D healthBar;

        public UIHealthBar(GameBoard game, UIUnit belongsTo, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.reps = belongsTo;
        }

        public static void Load(ContentManager content) {
            healthBar = content.Load<Texture2D>(@"Images/CharacterInfoElements/healthBar");
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            float healthPercent = (float)this.reps.representing.CurrentHealth / (float)this.reps.representing.maxHealth;
            Rectangle drawBounds = new Rectangle((int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)((float)(this.boundaries.Width * gameBoard.viewScale) * healthPercent), (int)(this.boundaries.Height * gameBoard.viewScale));

            //Draw shadow
            spriteBatch.Draw(healthBar, new Rectangle(drawBounds.X + 1, drawBounds.Y + 1, drawBounds.Width, drawBounds.Height), Color.Black * 0.3f);
            //Draw main sprite
            spriteBatch.Draw(healthBar, drawBounds, Color.White);
        }
    }
}
