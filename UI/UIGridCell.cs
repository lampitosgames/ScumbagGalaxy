using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI {
    public class UIGridCell : UIBox {
        //All possible textures for this object.  Loaded once for all objects
        static Texture2D gridCell;
        static Texture2D gridCellActive;
        static Texture2D gridCellPossible;
        static Texture2D gridCellAttackPossible;
        static Texture2D gridCellAttackActive;
        static Texture2D gridCellHealPossible;
        static Texture2D gridCellHealActive;
        static Texture2D gridCellMovePossible;
        static Texture2D gridCellMoveActive;
        static Texture2D gridCellTeleportPossible;
        static Texture2D gridCellTeleportActive;

        //This grid cell's position in the grid
        public int x;
        public int y;

        //Grid holding this cell
        public UIGrid grid;

        //Is the state of this cell fixed (can't be changed internally)
        public bool fixedState;

        //Hold what state the cell is in
        public enum CellState { Inactive, Active, Possible, AttackPossible, AttackActive, HealPossible, HealActive, MovePossible, MoveActive, TeleportPossible, TeleportActive };
        public CellState cellState;

        public UIGridCell(GameBoard game, UIGrid grid, int realX, int realY, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            //Set this cell's position in the cell grid
            this.x = realX;
            this.y = realY;

            //Set the grid this cell belongs to
            this.grid = grid;

            //Set the default cell state to "cell"
            this.cellState = CellState.Inactive;
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            //Create a new rectangle for the draw position of this cell, taking into account the view.
            Rectangle drawBounds = new Rectangle((int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)(this.boundaries.Width * gameBoard.viewScale), (int)(this.boundaries.Height * gameBoard.viewScale));

            //Switch to determine the cell state, and draw the corosponding texture
            switch (cellState) {
                case CellState.Inactive:
                    spriteBatch.Draw(gridCell, drawBounds, Color.White);
                    break;
                case CellState.Possible:
                    spriteBatch.Draw(gridCellPossible, drawBounds, Color.White);
                    break;
                case CellState.Active:
                    spriteBatch.Draw(gridCellActive, drawBounds, Color.White);
                    break;
                case CellState.AttackPossible:
                    spriteBatch.Draw(gridCellAttackPossible, drawBounds, Color.White);
                    break;
                case CellState.AttackActive:
                    spriteBatch.Draw(gridCellAttackActive, drawBounds, Color.White);
                    break;
                case CellState.HealPossible:
                    spriteBatch.Draw(gridCellHealPossible, drawBounds, Color.White);
                    break;
                case CellState.HealActive:
                    spriteBatch.Draw(gridCellHealActive, drawBounds, Color.White);
                    break;
                case CellState.MovePossible:
                    spriteBatch.Draw(gridCellMovePossible, drawBounds, Color.White);
                    break;
                case CellState.MoveActive:
                    spriteBatch.Draw(gridCellMoveActive, drawBounds, Color.White);
                    break;
                case CellState.TeleportPossible:
                    spriteBatch.Draw(gridCellTeleportPossible, drawBounds, Color.White);
                    break;
                case CellState.TeleportActive:
                    spriteBatch.Draw(gridCellTeleportActive, drawBounds, Color.White);
                    break;
            }
        }

        public static void Load(ContentManager content) {
            //Load default sprites for all possible grid states
            gridCell = content.Load<Texture2D>(@"Images/Grid/gridCell");
            gridCellPossible = content.Load<Texture2D>(@"Images/Grid/gridCellPossible");
            gridCellActive = content.Load<Texture2D>(@"Images/Grid/gridCellActive");
            gridCellAttackPossible = content.Load<Texture2D>(@"Images/Grid/gridCellAttackPossible");
            gridCellAttackActive = content.Load<Texture2D>(@"Images/Grid/gridCellAttackActive");
            gridCellHealPossible = content.Load<Texture2D>(@"Images/Grid/gridCellHealPossible");
            gridCellHealActive = content.Load<Texture2D>(@"Images/Grid/gridCellHealActive");
            gridCellMovePossible = content.Load<Texture2D>(@"Images/Grid/gridCellMovePossible");
            gridCellMoveActive = content.Load<Texture2D>(@"Images/Grid/gridCellMoveActive");
            gridCellTeleportPossible = content.Load<Texture2D>(@"Images/Grid/gridCellTeleportPossible");
            gridCellTeleportActive = content.Load<Texture2D>(@"Images/Grid/gridCellTeleportActive");
        }

        public override void MouseEnter() {
            //Switch to determine the cell state
            switch (cellState) {
                case CellState.Inactive:
                    if (this.fixedState == false) {
                        this.cellState = CellState.Possible;
                    }
                    break;
                case CellState.Possible:
                    //Nothing currently
                    break;
                case CellState.Active:
                    //Nothing currently
                    break;
                case CellState.AttackPossible:
                    this.grid.CellEnter(this.cellState, this.x, this.y);
                    break;
                case CellState.AttackActive:
                    this.grid.CellEnter(this.cellState, this.x, this.y);
                    break;
                case CellState.HealPossible:
                    this.grid.CellEnter(this.cellState, this.x, this.y);
                    break;
                case CellState.HealActive:
                    this.grid.CellEnter(this.cellState, this.x, this.y);
                    break;
                case CellState.MovePossible:
                    if (this.fixedState == false) {
                        this.cellState = CellState.MoveActive;
                    }
                    break;
                case CellState.MoveActive:
                    //Nothing currently
                    break;
                case CellState.TeleportPossible:
                    this.grid.CellEnter(this.cellState, this.x, this.y);
                    break;
                case CellState.TeleportActive:
                    this.grid.CellEnter(this.cellState, this.x, this.y);
                    break;
            }
        }

        public override void MouseLeave() {
            //Switch to determine the cell state
            switch (cellState) {
                case CellState.Inactive:
                    //Nothing currently
                    break;
                case CellState.Possible:
                    //If the cell state is not fixed
                    if (this.fixedState == false) {
                        this.cellState = CellState.Inactive;
                    }
                    break;
                case CellState.Active:
                    //Nothing currently
                    break;
                case CellState.AttackPossible:
                    //Nothing currently
                    break;
                case CellState.AttackActive:
                    this.grid.CellExit(this.cellState, this.x, this.y);
                    break;
                case CellState.HealPossible:
                    //Nothing currently
                    break;
                case CellState.HealActive:
                    this.grid.CellExit(this.cellState, this.x, this.y);
                    break;
                case CellState.MovePossible:
                    //Nothing currently
                    break;
                case CellState.MoveActive:
                    //If the cell state is not fixed
                    if (this.fixedState == false) {
                        this.cellState = CellState.MovePossible;
                    }
                    break;
                case CellState.TeleportPossible:
                    //Nothing currently
                    break;
                case CellState.TeleportActive:
                    this.grid.CellExit(this.cellState, this.x, this.y);
                    break;
            }
        }

        public override void MouseUp(Mouse button) {
            if (button == Mouse.Left) {
                this.grid.CellClick(this.cellState, this.x, this.y);
            }
        }
    }
}
