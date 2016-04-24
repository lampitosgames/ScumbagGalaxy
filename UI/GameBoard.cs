using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI {
    public class GameBoard {
        //There are 3 game states, and a static gameboard variable to represent each of them
        public static GameBoard game; //state 3
        public static GameBoard teamSelect; //state 2
        public static GameBoard startMenu; //sate 1
        public static GameBoard endMenu; //state 4

        public static int activeState;

        public static SpriteFont font;

        //Size of the game board in width and height
        public int gameWidth;
        public int gameHeight;

        //Size of a single cell
        public int cellWidth;
        public int cellHeight;
        //Number of cells
        public int cellNum;

        public List<UIBox>[][] dataGrid;

        //Boundaries of the view.  X and Y represent the top-left corner of the view with respect to the game board
        public int viewX;
        public int viewY;
        public int viewWidth;
        public int viewHeight;
        //viewScale determines the percentage ratio of how much each element is zoomed.  The view width and height remain constant, but this scales things inside the view
        public double viewScale;

        /// <summary>
        /// The GameBoard constructor
        /// </summary>
        /// <param name="gameWidth">The entire width of the game (in pixels)</param>
        /// <param name="gameHeight">The entire height of the game (in pixels)</param>
        /// <param name="numOfCells">How many cells to divide the game by to make sure</param>
        public GameBoard(int gameWidth, int gameHeight, int numOfCells) {
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;
            this.cellWidth = gameWidth / numOfCells;
            this.cellHeight = gameHeight / numOfCells;
            this.cellNum = numOfCells;

            //Default values for the view
            this.viewX = 0;
            this.viewY = 0;
            this.viewHeight = 720;
            this.viewWidth = 1280;
            this.viewScale = 1.0;

            //Initialize the data grid
            dataGrid = new List<UIBox>[numOfCells][];
            for (int i=0; i<numOfCells; i++) {
                dataGrid[i] = new List<UIBox>[8];
                for (int j=0; j<numOfCells; j++) {
                    dataGrid[i][j] = new List<UIBox>();
                }
            }
        }
        
        /// <summary>
        /// Take a UIBox and place it in all grid cells intersecting with it
        /// </summary>
        /// <param name="item">UIBox to place</param>
        public void PlaceUIBox(UIBox item) {
            //Get the start and end positions 
            int startCol = Utils.Clamp(0, this.cellNum, (int)Math.Floor((double)item.boundaries.X / this.cellWidth));
            int endCol = Utils.Clamp(0, this.cellNum, (int)Math.Ceiling((double)(item.boundaries.X + item.boundaries.Width) / this.cellWidth));
            int startRow = Utils.Clamp(0, this.cellNum, (int)Math.Floor((double)item.boundaries.Y / this.cellHeight));
            int endRow = Utils.Clamp(0, this.cellNum, (int)Math.Ceiling((double)(item.boundaries.Y + item.boundaries.Height) / this.cellHeight));

            //Loop through all sections that the object intersects with.  Use a do/while loop in case the entire UIBox fits into the 0, 0 cell
            int h = startRow;
            do {
                int w = startCol;
                do {
                    //Add this item to that cell's list
                    this.dataGrid[h][w].Add(item);
                    w++;
                } while (w < endCol);

                h++;
            } while (h < endRow);
        }
        
        /// <summary>
        /// Remove a UIBox from all the cells it currently intersects.  This is the opposite of PlaceUIBox()
        /// </summary>
        /// <param name="item">UIBox reference to remove</param>
        public void RemoveUIBox(UIBox item) {
            //Get the start and end positions 
            int startCol = Utils.Clamp(0, this.cellNum, (int)Math.Floor((double)item.boundaries.X / this.cellWidth));
            int endCol = Utils.Clamp(0, this.cellNum, (int)Math.Ceiling((double)(item.boundaries.X + item.boundaries.Width) / this.cellWidth));
            int startRow = Utils.Clamp(0, this.cellNum, (int)Math.Floor((double)item.boundaries.Y / this.cellHeight));
            int endRow = Utils.Clamp(0, this.cellNum, (int)Math.Ceiling((double)(item.boundaries.Y + item.boundaries.Height) / this.cellHeight));

            //Loop through all sections that the object intersects with
            int h = startRow;
            do {
                int w = startCol;
                do {
                    //Remove this item from that cell's list
                    this.dataGrid[h][w].Remove(item);
                    w++;
                } while (w <= endCol);

                h++;
            } while (h <= endRow);
        }
        
        /// <summary>
        /// Return a sorted list of all UIBoxes that collide with the provided position
        /// </summary>
        /// <param name="xPos">X position to check</param>
        /// <param name="yPos">Y Position to check</param>
        /// <returns>Sorted list of UIBox references that are visible</returns>
        public List<UIBox> CheckPos(int xPos, int yPos) {
            //Get the dataGrid cell number estimate
            int cellX = Utils.Clamp(0, this.cellNum-1, xPos / this.cellWidth);
            int cellY = Utils.Clamp(0, this.cellNum-1, yPos / this.cellHeight);

            //Create a list to store valid UIBoxes to return
            List<UIBox> returnList = new List<UIBox>();
            //Get the reference to objects in this current cell
            List<UIBox> cellList = this.dataGrid[cellY][cellX];

            //Loop through the cellList, check if each item is valid, and then pass it into the returnList.
            for (int i=0; i<cellList.Count; i++) {
                //If the UIBox is visible
                if (cellList[i].visible == true) {
                    //If the UIBox collides with the provided point
                    if (cellList[i].boundaries.Contains(xPos, yPos)) {
                        //Add the UIBox to the return list
                        returnList.Add(cellList[i]);
                    }
                }
            }

            //Sort the return list
            Utils.SortUIBoxList(returnList);

            //Return UIBoxes
            return returnList;
        }
        
        /// <summary>
        /// Find all cells that intersect with the provided rectangle and return unique UIBoxes in those cells ordered by layer.  Quicker runtime, not exact.
        /// </summary>
        /// <param name="x1">Left side position of the rectangle</param>
        /// <param name="y1">Top side position of the rectangle</param>
        /// <param name="x2">Right side position of the rectangle</param>
        /// <param name="y2">Bottom side position of the rectangle</param>
        /// <returns>Sorted list of UIBoxes estimated to collide with the provided rectangle</returns>
        public List<UIBox> CheckRect(int x1, int y1, int x2, int y2) {
            //Get the start and end positions 
            int startCol = Utils.Clamp(0, this.cellNum, (int)Math.Floor((double)x1 / this.cellWidth));
            int endCol = Utils.Clamp(0, this.cellNum, (int)Math.Ceiling((double)x2 / this.cellWidth));
            int startRow = Utils.Clamp(0, this.cellNum, (int)Math.Floor((double)y1 / this.cellHeight));
            int endRow = Utils.Clamp(0, this.cellNum, (int)Math.Ceiling((double)y2 / this.cellHeight));

            //Start a return list
            List<UIBox> returnList = new List<UIBox>();

            //Loop through all sections that the rectangle intersects with
            for (int h=startRow; h<endRow; h++) {
                for (int w=startCol; w<endCol; w++) {
                    //Get the current cell
                    List<UIBox> cellList = this.dataGrid[h][w];
                    //Merge the current cell's list with the return list, only keeping unique values
                    returnList = returnList.Union(cellList).ToList();
                }
            }

            //Do not return objects that are not visible
            for (int i=0; i<returnList.Count; i++) {
                if (returnList[i].visible == false) {
                    returnList.Remove(returnList[i]);
                    i -= 1;
                }
            }

            //Sort the return list
            Utils.SortUIBoxList(returnList);

            //Return the completed list of unique, visible UIBoxes contained in the specefied rectangle
            return returnList;
        }

        //Same as above, but collision checks margin objects to determine if they are actually colliding with the rect, or if they are simply a part of a cell that the rect collides with.  Does not return the second type.
        /// <summary>
        /// An exact version of CheckRect()
        /// </summary>
        /// <param name="x1">Left side position of the rectangle</param>
        /// <param name="y1">Top side position of the rectangle</param>
        /// <param name="x2">Right side position of the rectangle</param>
        /// <param name="y2">Bottom side position of the rectangle</param>
        /// <returns>Sorted list of UIBoxes known to collide with the provided rectangle</returns>
        public List<UIBox> CheckRectExact(int x1, int y1, int x2, int y2) {
            //Check the rectangle
            List<UIBox> uniques = this.CheckRect(x1, y1, x2, y2);

            //Create a rectangle out of the provided coordinates
            Rectangle bounds = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            //Check each of the unique UIBoxes for collision with the provided rectangle.  If no collision is detected, remove it
            for (int i=0; i<uniques.Count; i++) {
                if (uniques[i].boundaries.Intersects(bounds) == false) {
                    uniques.Remove(uniques[i]);
                    i -= 1;
                }
            }

            //Return unique, collision checked UIBoxes that are inside or partially inside the provided rectangle
            return uniques;
        }

        //TODO :: Move the view window
        public void MoveView(int newViewX, int newViewY) {
            //Find items that snap to the view and modify their positions to reflect the new view location
            //**TODO**

            //Move the view to a new position
            this.viewX = Utils.Clamp(0, this.gameWidth - (int)(this.viewWidth / this.viewScale), newViewX);
            this.viewY = Utils.Clamp(0, this.gameHeight - (int)(this.viewHeight / this.viewScale), newViewY);
        }

        //TODO :: Scale the view window
        public void ScaleView(double scaleMultiplier) {
            //Scale all items that scale with the view
            //**TODO**

            this.viewScale = Utils.ClampDouble(1.0, 3.0, scaleMultiplier);
            MoveView(this.viewX, this.viewY);
        }

        //Take two mouse states, compare them, and pass events to objects
        //TODO :: Implement objects that do not accept mouse events
        public void Update(MouseState mouseState, MouseState mousePrevState) {
            //Get the real mouse position by removing view calculations
            int prevMouseX = (int)(mousePrevState.X / this.viewScale) + (int)(this.viewX);
            int prevMouseY = (int)(mousePrevState.Y / this.viewScale) + (int)(this.viewY);
            int curMouseX = (int)(mouseState.X / this.viewScale) + (int)(this.viewX);
            int curMouseY = (int)(mouseState.Y / this.viewScale) + (int)(this.viewY);

            //Check for objects the mouse was over in the last step
            List<UIBox> prevCollide = this.CheckPos(prevMouseX, prevMouseY);
            //Check for objects the mouse is currently over
            List<UIBox> curCollide = this.CheckPos(curMouseX, curMouseY);

            //Only keep the highest layered objects that have mouse events
            //Remove items without mouse events
            for (int m = 0; m < prevCollide.Count; m++) {
                if (prevCollide[m].mouseEvents == false) {
                    prevCollide.Remove(prevCollide[m]);
                    m -= 1;
                }
            }
            //If there are items in the prevCollide list
            if (prevCollide.Count > 0) {
                //Get the lowest layer value
                int lowLayer = prevCollide[0].layer;
                //loop through the list
                for (int m = 0; m < prevCollide.Count; m++) {
                    //If the current UIBox has a different layer from the lowest
                    if (prevCollide[m].layer != lowLayer) {
                        //Remove it from the list
                        prevCollide.Remove(prevCollide[m]);
                        m -= 1;
                    }
                }
            }

            //Remove items without mouse events
            for (int n = 0; n < curCollide.Count; n++) {
                if (curCollide[n].mouseEvents == false) {
                    curCollide.Remove(curCollide[n]);
                    n -= 1;
                }
            }
            //If there are items in the curCollide list
            if (curCollide.Count > 0) {
                //Get the lowest layer value
                int lowLayer = curCollide[0].layer;
                //Loop through the list
                for (int n = 0; n < curCollide.Count; n++) {
                    //If the current UIBox has a different layer from the lowest
                    if (curCollide[n].layer != lowLayer) {
                        //Remvoe it from the list
                        curCollide.Remove(curCollide[n]);
                        n -= 1;
                    }
                }
            }

            //Find all UIBoxes in the curCollide list that are not in prevCollide
            List<UIBox> inCurCollide = curCollide.Where(b => !prevCollide.Any(b2 => b2 == b)).ToList();
            //Find all UIBoxes in the prevCollide list that are not in curCollide
            List<UIBox> inPrevCollide = prevCollide.Where(b => !curCollide.Any(b2 => b2 == b)).ToList();

            /*
                Detect MouseEnter events
            */
            //If there are elements in the inCurCollide
            if (inCurCollide.Count > 0) {
                //Sort inCurCollide
                Utils.SortUIBoxList(inCurCollide);
                //Get the layer of the first element
                int firstLayer = inCurCollide[0].layer;
                //Loop through all inCurCollide elements on the first layer and pass the mouseenter event to all
                for (int i = 0; i < inCurCollide.Count; i++) {
                    if (inCurCollide[i].layer == firstLayer) {
                        inCurCollide[i].MouseEnter();
                    } else {
                        break;
                    }
                }
            }

            /*
                Detect MouseLeave events
            */
            //If there are elements in the inPrevCollide (elements that were in the previous mouse location that are not in the current)
            if (inPrevCollide.Count > 0) {
                //Sort inPrevCollide
                Utils.SortUIBoxList(inPrevCollide);
                //Get the layer of the first element
                int firstLayer = inPrevCollide[0].layer;
                //Loop through all inPrevCollide elements on the first layer and pass the mouseLeave event to all
                for (int i=0; i < inPrevCollide.Count; i++) {
                    if (inPrevCollide[i].layer == firstLayer) {
                        inPrevCollide[i].MouseLeave();
                    } else {
                        break;
                    }
                }
            }

            //If there are elements at the current mouse position
            if (curCollide.Count > 0) {

            /*
                Detect MouseDown events
            */
                //Check for left mouse down.  If the button is depressed in the current state and was not in the previous
                if ((mouseState.LeftButton == ButtonState.Pressed) && (mousePrevState.LeftButton == ButtonState.Released)) {
                    //Loop through all UIBoxes at the location.  (Lower layered objects were already removed)
                    for (int i=0; i<curCollide.Count; i++) {
                        //Pass the mouseDown event to the UIBox
                        curCollide[i].MouseDown(UIBox.Mouse.Left);
                    }
                }
                //Check for right mouse down, reusing the same method
                if ((mouseState.RightButton == ButtonState.Pressed) && (mousePrevState.RightButton == ButtonState.Released)) {
                    for (int i = 0; i < curCollide.Count; i++) {
                        curCollide[i].MouseDown(UIBox.Mouse.Right);
                    }
                }
                //Check for middle mouse down, reusing the same method
                if ((mouseState.MiddleButton == ButtonState.Pressed) && (mousePrevState.MiddleButton == ButtonState.Released)) {
                    for (int i = 0; i < curCollide.Count; i++) {
                        curCollide[i].MouseDown(UIBox.Mouse.Middle);
                    }
                }

            /*
                Detect MouseUp events
            */
                //Check for left mouse up.  If the button is depressed in the previous state and is not in the current
                if ((mouseState.LeftButton == ButtonState.Released) && (mousePrevState.LeftButton == ButtonState.Pressed)) {
                    //Loop through all UIBoxes at the location.  (Lower layered objects were already removed)
                    for (int i = 0; i < curCollide.Count; i++) {
                        //Pass the MouseUp event to the UIBox
                        curCollide[i].MouseUp(UIBox.Mouse.Left);
                    }
                }
                //Check for right mouse up, reusing the same method
                if ((mouseState.RightButton == ButtonState.Released) && (mousePrevState.RightButton == ButtonState.Pressed)) {
                    for (int i = 0; i < curCollide.Count; i++) {
                        curCollide[i].MouseUp(UIBox.Mouse.Right);
                    }
                }
                //Check for middle mouse down, reusing the same method
                if ((mouseState.MiddleButton == ButtonState.Released) && (mousePrevState.MiddleButton == ButtonState.Pressed)) {
                    for (int i = 0; i < curCollide.Count; i++) {
                        curCollide[i].MouseUp(UIBox.Mouse.Middle);
                    }
                }

                /*
                    Detect MouseScroll events
                */
                //Check for different mouse scroll states between the previous and current mouse states
                if (mouseState.ScrollWheelValue != mousePrevState.ScrollWheelValue) {
                    //Pass the scroll delta to the MouseScroll event function
                    for (int i=0; i<curCollide.Count; i++) {
                        curCollide[i].MouseScroll(mouseState.ScrollWheelValue - mousePrevState.ScrollWheelValue);
                    }
                }
            }

            /*
                Detect MouseMove events
            */
            //If there are objects in the prevCollide list
            if (prevCollide.Count > 0) {
                //We know that the mouse was colliding in the previous state
                //If the mouse position in the previous state is different from that in the current state
                if (prevMouseX != curMouseX || prevMouseY != curMouseY) {
                    //Even if the mouse is no longer colliding (which would trigger a MouseOut event), the mouse still moved.  This is helpful mostly for dragabble elements
                    for (int i=0; i< prevCollide.Count; i++) {
                        prevCollide[i].MouseMove(prevMouseX, curMouseX, prevMouseY, curMouseY, mouseState);
                    }
                }
            }
        }

        //TODO :: Draw everything contained inside the view
        public void Draw(SpriteBatch spriteBatch) {
            //Get a list of all objects in the view
            List<UIBox> toDraw = this.CheckRect(this.viewX, this.viewY, this.viewX + (int)(this.viewWidth/this.viewScale), this.viewY + (int)(this.viewHeight/this.viewScale));

            for (int i=toDraw.Count-1; i>=0; i--) {
                toDraw[i].Draw(this, spriteBatch);
            }
        }
    }
}