using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ScumbagGalaxy.UI {
    public class UIBox {
        //Mouse buttons
        public enum Mouse { Left, Right, Middle };

        //Game Board this object belongs to
        GameBoard thisGameBoard;

        //Layer this ui element is drawn on
        public int layer;
        //The boundaries of this ui element from the perspective of the entire game board
        public Rectangle boundaries;
        //Does this UI element move with the view and stay on screen (true), or is it static with respect to the game board (false)
        public bool viewSnap;
        //Ratio of size change when the view zooms (0.0f is static size, 1.0f is moves with the view, and there can be any number of in-betweens)
        public float viewScale;
        //Does this object capture mouse events
        public bool mouseEvents;
        //Is this object visible
        public bool visible;
        //Can this object move the view
        public bool viewMove;

        //Graphics
        //This object's texture that gets drawn in the Draw() method
        public Texture2D thisSprite;

        /// <summary>
        /// The UIBox constructor
        /// </summary>
        /// <param name="game">GameBoard this UIBox belongs to and is referenced from</param>
        /// <param name="xPos">X position of this UIBox in the GameBoard</param>
        /// <param name="yPos">Y position of this UIBox in the GameBoard</param>
        /// <param name="width">Width of this UIBox</param>
        /// <param name="height">Height of this UIBox</param>
        public UIBox(GameBoard game, int xPos, int yPos, int width, int height) {
            //Create a new bounding rectangle for the UIBox
            this.boundaries = new Rectangle(xPos, yPos, width, height);
            //Place this object into the gameboard
            game.PlaceUIBox(this);

            this.thisGameBoard = game;

            //This object captures mouse events by default
            this.mouseEvents = true;
            //This object can move the view by default
            this.viewMove = true;
        }

        /// <summary>
        /// Delete this object from its old location, change its rectangle, and place it back in the gameboard
        /// </summary>
        /// <param name="xPos">X position to move to</param>
        /// <param name="yPos">Y position to move to</param>
        public virtual void MoveTo(int xPos, int yPos) {
            GameBoard.game.RemoveUIBox(this);
            Rectangle oldRect = this.boundaries;
            this.boundaries = new Rectangle(xPos, yPos, oldRect.Width, oldRect.Height);
            GameBoard.game.PlaceUIBox(this);
        }
        
        /// <summary>
        /// Draw this UIBox
        /// </summary>
        /// <param name="gameBoard">Reference to the gameboard so the object can take into account the view scaling</param>
        /// <param name="spriteBatch">The SpriteBatch to be used for drawing</param>
        public virtual void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            //Create a new rectangle for the draw position, taking into account the view.
            Rectangle drawBounds = new Rectangle((int)(this.boundaries.X*gameBoard.viewScale) - (int)(gameBoard.viewX*gameBoard.viewScale), (int)(this.boundaries.Y*gameBoard.viewScale) - (int)(gameBoard.viewY*gameBoard.viewScale), (int)(this.boundaries.Width*gameBoard.viewScale), (int)(this.boundaries.Height*gameBoard.viewScale));
            
            spriteBatch.Draw(thisSprite, drawBounds, Color.White);
        }

        /*
            Mouse Events
        */
        //When the mouse enters the boundaries of this object
        public virtual void MouseEnter() {

        }

        //When the mouse leaves the boundaries of this object
        public virtual void MouseLeave() {

        }

        //When the button provided is pressed
        public virtual void MouseDown(Mouse button) {

        }

        //When the button provided is lifted
        public virtual void MouseUp(Mouse button) {

        }

        //When the mouse is scrolled. Delta is negative when scrolling up, positive when scrolling down
        public virtual void MouseScroll(int delta) {
            //By default, allow the view to be zoomed on mouse scroll from any UIBox
            if (this.viewMove) {
                if (delta > 0) {
                    this.thisGameBoard.ScaleView(this.thisGameBoard.viewScale + 0.1);
                } else {
                    this.thisGameBoard.ScaleView(this.thisGameBoard.viewScale - 0.1);
                }
            }
        }

        //When the mouse moves while inside this object
        public virtual void MouseMove(int prevX, int curX, int prevY, int curY, MouseState state) {
            //Allow the view to be dragged by right clicking and dragging from any UIBox unless this event is overriden
            //If the right mouse button is down
            if (state.RightButton == ButtonState.Pressed && this.viewMove) {
                //Move the view
                this.thisGameBoard.MoveView(this.thisGameBoard.viewX + (prevX - curX), this.thisGameBoard.viewY + (prevY - curY));
            }
        }
    }
}
