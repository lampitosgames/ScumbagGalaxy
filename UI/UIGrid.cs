using Microsoft.Xna.Framework;
using ScumbagGalaxy.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI {
    public class UIGrid {

        public UIGridCell[,] grid;
        public int width;
        public int height;
        public static int margin;

        public Grid dataGrid;
        //Constructor
        public UIGrid(Grid dataGrid) {
            //Save the width and height of the grid
            this.width = dataGrid.width;
            this.height = dataGrid.height;
            //Save the grid
            this.dataGrid = dataGrid;

            //Create a grid of UIGridCells equal in size to the data grid
            this.grid = new UIGridCell[this.height, this.width];

            //Set the margin size
            margin = 95;

            //Create a UIGridCell for each position in the grid
            for (int h=0; h<this.height; h++) {
                for (int w=0; w<this.width; w++) {
                    this.grid[h, w] = new UIGridCell(GameBoard.game, this, w, h, (w * 63) + margin, (h * 63) + margin, 63, 63);
                    this.grid[h, w].layer = 99;
                    this.grid[h, w].visible = true;
                }
            }

            //Create a UIUnit object for each unit in the data grid.  Save the UI object reference in the LiveUnit.  The UIUnit can be accessed through the data grid
            for (int h=0; h<this.height; h++) {
                for (int w=0; w<this.width; w++) {
                    LiveUnit cur = dataGrid.Get(w, h);
                    if (cur != null) {
                        cur.sprite = new UIUnits.UIUnit(GameBoard.game, cur, w, h, (w * 63) + 6 + margin, (h * 63) - 21 + margin, 51, 76);
                        cur.sprite.layer = 98;
                        cur.sprite.visible = true;
                        cur.sprite.mouseEvents = false;
                    }
                }
            }
        }

        public void Set(UIGridCell.CellState state, int xInd, int yInd) {
            grid[yInd, xInd].cellState = state;
        }

        public void SetRegion(UIGridCell.CellState state, int x1, int y1, int x2, int y2) {
            int x1f = Utils.Clamp(0, this.width - 1, x1);
            int x2f = Utils.Clamp(0, this.width - 1, x2);
            int y1f = Utils.Clamp(0, this.height - 1, y1);
            int y2f = Utils.Clamp(0, this.height - 1, y2);

            for (int h = y1f; h <= y2f; h++) {
                for (int w = x1f; w <= x2f; w++) {
                    grid[h, w].cellState = state;
                }
            }
        }

        public void SetRegion(UIGridCell.CellState state, int centerX, int centerY, int radius) {
            int nx1 = Utils.Clamp(0, this.width - 1, centerX - radius + 1);
            int nx2 = Utils.Clamp(0, this.width - 1, centerX + radius - 1);
            int ny1 = Utils.Clamp(0, this.height - 1, centerY - radius + 1);
            int ny2 = Utils.Clamp(0, this.height - 1, centerY + radius - 1);

            for (int h = ny1; h <= ny2; h++) {
                for (int w = nx1; w <= nx2; w++) {
                    this.grid[h, w].cellState = state;
                }
            }
        }

        //Set all UIGridCells to have inactive states
        public void Clear() {
            this.SetRegion(UIGridCell.CellState.Inactive, 0, 0, this.width-1, this.height-1);
            for (int h=0; h<this.height; h++) {
                for (int w = 0; w < this.width; w++) {
                    this.grid[h, w].fixedState = false;
                }
            }
        }

        //Move a UI unit as well as it's corrosponding LiveUnit
        public void Move(int xStart, int yStart, int xEnd, int yEnd) {
            if (Utils.RangeContains(0, this.width - 1, xStart, xEnd) && Utils.RangeContains(0, this.height - 1, yStart, yEnd)) {
                LiveUnit cur = dataGrid.Get(xStart, yStart);
                cur.sprite.xInd = xEnd;
                cur.sprite.yInd = yEnd;
                cur.sprite.MoveTo((xEnd * 63) + 6 + margin, (yEnd * 63) - 21 + margin);
                dataGrid.Move(xStart, yStart, xEnd, yEnd);
            } else {
                throw new IndexOutOfRangeException("Provided movement coordinates Out of Bounds");
            }
        }

        //Handle an individual cell being clicked
        public void CellClick(UIGridCell.CellState state, int x, int y) {
            //Pass different parameters depending on the state of the cell clicked
            UnitManager.Manager.CellClick(state, x, y);
        }

        //Handle individual cells being entered with the mouse
        public void CellEnter(UIGridCell.CellState state, int x, int y) {
            UnitManager.Manager.CellEnter(state, x, y);
        }

        //Handle individual cells being exited with the mouse
        public void CellExit(UIGridCell.CellState state, int x, int y) {
            UnitManager.Manager.CellLeave(state, x, y);
        }
    }
}
