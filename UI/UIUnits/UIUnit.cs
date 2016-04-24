using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UIUnit : UIBox {
        public LiveUnit representing;
        public int xInd, yInd;

        //This unit's UI elements
        public UIHealthBar healthBar;
        public UIMovesLeft movesLeft;
        //This unit's active UI elements
        public UIStatBackground activeWindowWrapper;

        public UIUnit(GameBoard game, LiveUnit rep, int xIndex, int yIndex, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            //Save the reference to the object being represented
            this.representing = rep;

            //Save this object's index in the UIGrid
            this.xInd = xIndex;
            this.yInd = yIndex;

            //Create UI elements for this unit

            if (!rep.ContainsBuffType(BuffType.IsUnselectable))
            {
                //Healthbar
                this.healthBar = new UIHealthBar(game, this, xPos - 4, yPos, 59, 2);
                this.healthBar.visible = true;
                this.healthBar.layer = 97;
                this.healthBar.mouseEvents = false;
                //Actions Indicator
                this.movesLeft = new UIMovesLeft(game, this, xPos + 42, yPos + 8, 15, 12);
                this.movesLeft.visible = true;
                this.movesLeft.layer = 97;
                this.movesLeft.mouseEvents = false;
                //Create the active display elements
                this.activeWindowWrapper = new UIStatBackground(game, this.representing, xPos - 22, yPos - 40, 95, 35);
                this.activeWindowWrapper.visible = true;
                this.activeWindowWrapper.layer = 96;
                this.activeWindowWrapper.mouseEvents = false;
            }
        }

        public void Load(ContentManager content) {
            //Load this unit's sprite based on the LiveUnit filepath
            Console.WriteLine(this.representing.SpritePath);
            thisSprite = content.Load<Texture2D>(this.representing.SpritePath);
        }

        public void MoveSubUI() {
            this.healthBar.MoveTo(this.boundaries.X - 4, this.boundaries.Y);
            this.movesLeft.MoveTo(this.boundaries.X + 42, this.boundaries.Y + 8);
            this.activeWindowWrapper.MoveTo(this.boundaries.X - 22, this.boundaries.Y - 40);
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            base.Draw(gameBoard, spriteBatch);
        }
    }
}
