using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI.UIStartScreen {
    class UIStartButton : UIBox {

        public static Texture2D button;
        public static Texture2D buttonHover;

        bool hover;
        
        public UIStartButton(GameBoard game, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
        }

        public static void Load(ContentManager content) {
            button = content.Load<Texture2D>(@"Images/TitleScreen/titleScreenStartButton");
            buttonHover = content.Load<Texture2D>(@"Images/TitleScreen/titleScreenStartButtonHover");
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            //Create a new rectangle for the draw position, taking into account the view.
            Rectangle drawBounds = new Rectangle((int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)(this.boundaries.Width * gameBoard.viewScale), (int)(this.boundaries.Height * gameBoard.viewScale));
            if (hover) {
                spriteBatch.Draw(buttonHover, drawBounds, Color.White);
            } else {
                spriteBatch.Draw(button, drawBounds, Color.White);
            }
            
        }

        public override void MouseEnter() {
            this.hover = true;
        }
        public override void MouseLeave() {
            this.hover = false;
        }
        public override void MouseUp(Mouse button) {
            GameBoard.activeState = 3;
        }
    }
}
