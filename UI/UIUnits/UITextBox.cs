using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UITextBox : UIBox {
        public static Texture2D textBack;
        List<string> writeLines;

        public UITextBox(GameBoard game, string text, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.layer = 90;
            this.mouseEvents = false;
            this.writeLines = new List<string>();
            ChangeString(text);
        }

        public static void Load(ContentManager content) {
            textBack = content.Load<Texture2D>(@"Images/CharacterInfoElements/textBoxBackground");
        }

        public void ChangeString(string newText) {
            //Split the string into words
            string[] words = newText.Split(' ');
            int[] lengths = new int[words.Length];
            //Determine word lengths (in characters)
            for (int i=0; i<words.Length; i++) {
                lengths[i] = words[i].Length;
            }

            //Get the maximum width of a line (there is a 5 pixel margin)
            int lw = this.boundaries.Width - 10;

            //The string for this line
            string line = words[0];
            //For all words
            for (int w=1; w<words.Length; w++) {
                //If adding this word would make the current line wider than it can be
                if ((line.Length+2.5+lengths[w])*2.5 > lw) {
                    //Add this line to the line list and then start a new line
                    this.writeLines.Add(line);
                    line = words[w];
                //Else, add this word preceded by a space to the current line
                } else {
                    line += " " + words[w];
                }
            }
            this.writeLines.Add(line);

            //Resize the rectangle to reflect the draw height.
            int newH = 5 + (9*this.writeLines.Count);
            this.boundaries = new Rectangle(this.boundaries.X, this.boundaries.Y - newH, this.boundaries.Width, newH);
        }
        
        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            //Create a new rectangle for the draw position, taking into account the view.
            Rectangle drawBounds = new Rectangle((int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)(this.boundaries.Width * gameBoard.viewScale), (int)(this.boundaries.Height * gameBoard.viewScale));
            //Draw the rectangle
            spriteBatch.Draw(textBack, drawBounds, Color.White);

            //Draw each line of text below the last
            for (int i=0; i<this.writeLines.Count; i++) {
                //Draw the text
                spriteBatch.DrawString(GameBoard.font, writeLines[i],
                    new Vector2(
                        (int)((this.boundaries.X + 5) * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale),
                        (int)((5 + this.boundaries.Y + (i*8)) * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale)
                        ),
                    new Color(228, 220, 209), 0f, new Vector2(0, 0), (float)gameBoard.viewScale / 3f, SpriteEffects.None, 0);
            }
        }
        
    }
}
