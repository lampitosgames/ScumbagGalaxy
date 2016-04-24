using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UIMovesLeft : UIBox {
        //Save the unit this health bar represents
        public UIUnit reps;

        //The actions remaining sprites
        public static Texture2D twoMovesLeft;
        public static Texture2D oneMovesLeft;
        public static Texture2D zeroMovesLeft;

        public UIMovesLeft(GameBoard game, UIUnit belongsTo, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.reps = belongsTo;
        }

        public static void Load(ContentManager content) {
            twoMovesLeft = content.Load<Texture2D>(@"Images/CharacterInfoElements/twoMovesLeft");
            oneMovesLeft = content.Load<Texture2D>(@"Images/CharacterInfoElements/oneMovesLeft");
            zeroMovesLeft = content.Load<Texture2D>(@"Images/CharacterInfoElements/zeroMovesLeft");
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {//Create a new rectangle for the draw position, taking into account the view.
            Rectangle drawBounds = new Rectangle((int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)(this.boundaries.Width * gameBoard.viewScale), (int)(this.boundaries.Height * gameBoard.viewScale));

            //Switch to determine which sprite to draw
            switch (this.reps.representing.MovesLeft) {
                case 2:
                    spriteBatch.Draw(twoMovesLeft, drawBounds, Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(oneMovesLeft, drawBounds, Color.White);
                    break;
                case 0:
                    spriteBatch.Draw(zeroMovesLeft, drawBounds, Color.White);
                    break;
            }
        }
    }
}
