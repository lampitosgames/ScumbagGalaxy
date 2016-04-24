using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy {
    public class Grid {
        //Allowing global access to the grid instance
        public static Grid mainGrid;

        //Dimensions of the grid
        public int width;
        public int height;
        
        //2d array of Units representing the grid itself.
        private LiveUnit[,] grid;

        //readonly grid array
        public LiveUnit[,] _Grid { get { return grid; } }
        
        /// <summary>
        /// The Grid constructor
        /// </summary>
        /// <param name="w">Width of the new grid</param>
        /// <param name="h">Height of the new grid</param>
        public Grid(int w, int h) {
            this.grid = new LiveUnit[h, w];
            this.width = w;
            this.height = h;
        }
        
        /// <summary>
        /// Get the unit at the specefied location in the grid
        /// </summary>
        /// <param name="xPos">X position of the unit to return</param>
        /// <param name="yPos">Y positino of the unit to return</param>
        /// <returns>Returns a unit if a unit exists, else it returns null</returns>
        public LiveUnit Get(int xPos, int yPos) {
            if (Utils.InGrid(xPos, yPos, this.width, this.height)) {
                return grid[yPos, xPos];
            } else {
                return null;
            }
        }
        
        /// <summary>
        /// Place a unit in the grid.  Overrides other units that might be at this position.
        /// Throws IndexOutOfRangeException when the specefied location is OOB
        /// </summary>
        /// <param name="xPos">X position to insert the unit at</param>
        /// <param name="yPos">Y position to insert the unit at</param>
        /// <param name="unit">Unit to insert at this position</param>
        public void Set(int xPos, int yPos, LiveUnit unit) {
            grid[yPos, xPos] = unit;
        }

        /// <summary>
        /// Clear the grid
        /// </summary>
        public void Clear() {
            this.grid = new LiveUnit[this.height, this.width];
        }
        
        /// <summary>
        /// Returns a copy of the specefied region as a new grid using the two bounding grid corners.  If the grid region would be OOB, the returned grid is clamped to be in bounds.
        /// </summary>
        /// <param name="x1">X of the top-left corner</param>
        /// <param name="y1">Y of the top-left corner</param>
        /// <param name="x2">X of the bottom-right corner</param>
        /// <param name="y2">Y of the bottom-right corner</param>
        /// <returns>Copy grid of the specefied subsection, containing identical references</returns>
        public Grid GetRegion(int x1, int y1, int x2, int y2) {
            int nx1 = Utils.Clamp(0, this.width - 1, x1);
            int nx2 = Utils.Clamp(0, this.width - 1, x2);
            int ny1 = Utils.Clamp(0, this.height - 1, y1);
            int ny2 = Utils.Clamp(0, this.height - 1, y2);

            Grid newGrid = new Grid(nx2 - nx1 + 1, ny2 - ny1 + 1);

            for (int h=0; h<newGrid.height; h++) {
                for (int w=0; w<newGrid.width; w++) {
                    newGrid.Set(w, h, this.Get(nx1 + w, ny1 + h));
                }
            }
            return newGrid;
        }
        
        /// <summary>
        /// Returns a copy of the specefied region as a new grid using a center position and a radius.  If the grid region would be OOB, the return grid is clamped to be in bounds
        /// </summary>
        /// <param name="x">X of the center</param>
        /// <param name="y">Y of the center</param>
        /// <param name="radius">Radius of the grid to return (1 returns a 1x1 grid, 2 returns a 3x3 grid, etc.)</param>
        /// <returns>Copy grid of the specefied subsection, containing identical references</returns>
        public Grid GetRegion(int x, int y, int radius) {
            int nx1 = Utils.Clamp(0, this.width - 1, x-radius+1);
            int nx2 = Utils.Clamp(0, this.width - 1, x+radius-1);
            int ny1 = Utils.Clamp(0, this.height - 1, y-radius+1);
            int ny2 = Utils.Clamp(0, this.height - 1, y+radius-1);

            Grid newGrid = new Grid(nx2 - nx1 + 1, ny2 - ny1 + 1);

            for (int h = 0; h < newGrid.height; h++) {
                for (int w = 0; w < newGrid.width; w++) {
                    newGrid.Set(w, h, this.Get(nx1 + w, ny1 + h));
                }
            }
            return newGrid;
        }
        
        /// <summary>
        /// Takes a grid and pastes its values into this grid.  Throws IndexOutOfRangeException when the provided grid would paste values out of the range of this grid
        /// </summary>
        /// <param name="leftX">X location where the top-left corner of the provided grid will be placed</param>
        /// <param name="topY">Y location where the top-left corner of the provided grid will be placed</param>
        /// <param name="pasteGrid">Grid holding values to be pasted</param>
        public void SetRegion(int leftX, int topY, Grid pasteGrid) {
            if (Utils.RangeContains(0, this.width - 1, leftX, leftX + pasteGrid.width - 1) &&
                Utils.RangeContains(0, this.height - 1, topY, topY + pasteGrid.height - 1)) {
                for (int h=0; h<pasteGrid.height; h++) {
                    for (int w=0; w<pasteGrid.width; w++) {
                        try {
                            this.Set(leftX + w, topY + h, pasteGrid.Get(w, h));
                        } catch {

                        }
                    }
                }
            } else {
                throw new IndexOutOfRangeException("Part of the grid to be pasted is out of range of the grid being pasted into.");
            }
        }
        
        /// <summary>
        /// Move the value at position 1 to position 2.  Throws IndexOutOfRangeException when one of the provided locations is OOB
        /// </summary>
        /// <param name="xStart">X of the cell to be moved</param>
        /// <param name="yStart">Y of the cell to be moved</param>
        /// <param name="xEnd">X of the target destination</param>
        /// <param name="yEnd">Y of the target destination</param>
        public void Move(int xStart, int yStart, int xEnd, int yEnd) {
            if (Utils.RangeContains(0, this.width-1, xStart, xEnd) && Utils.RangeContains(0, this.height-1, yStart, yEnd)) {
                this.Set(xEnd, yEnd, this.Get(xStart, yStart));
                this.Set(xStart, yStart, null);
            } else {
                throw new IndexOutOfRangeException("Provided movement coordinates Out of Bounds");
            }
        }
        
        /// <summary>
        /// Apply damage to a unit's health.  If the position is null, nothing happens.  Throws IndexOutOfRangeException when the target location is OOB
        /// </summary>
        /// <param name="x">X position of the unit to damage</param>
        /// <param name="y">Y position of the unit to damage</param>
        /// <param name="damage">How much damage to deal to the target unit's health</param>
        public void DamageUnit(int x, int y, int damage) {
            if (Utils.InRange(0, this.width-1, x) && Utils.InRange(0, this.height-1, y)) {
                LiveUnit thisUnit = this.Get(x, y);
                if (thisUnit != null) {
                    thisUnit.ApplyDamage(damage);
                }
            } else {
                throw new IndexOutOfRangeException("Target position out of bounds");
            }
        }
        
        /// <summary>
        /// Apply damage to all units within a region specefied with two coordinates.  If partially OOB, the region will be clamped in-bounds
        /// </summary>
        /// <param name="x1">X position of the region's top-left corner</param>
        /// <param name="y1">Y position of the region's top-left corner</param>
        /// <param name="x2">X position of the region's bottom-right corner</param>
        /// <param name="y2">Y position of the region's bottom-right corner</param>
        /// <param name="damage">How much damage to deal to units in the range</param>
        public void DamageRegion(int x1, int y1, int x2, int y2, int damage) {
            int nx1 = Utils.Clamp(0, this.width - 1, x1);
            int nx2 = Utils.Clamp(0, this.width - 1, x2);
            int ny1 = Utils.Clamp(0, this.height - 1, y1);
            int ny2 = Utils.Clamp(0, this.height - 1, y2);

            for (int h = 0; h < ny2-ny1+1; h++) {
                for (int w = 0; w < nx2-nx1+1; w++) {
                    this.DamageUnit(w, h, damage);
                }
            }
        }

        /// <summary>
        /// Apply damage to all units within a region specefied with a center coordinate and a radius.  If partially OOB, the region will be clamped in-bounds
        /// </summary>
        /// <param name="x1">X position of the region's center</param>
        /// <param name="y1">Y position of the region's center</param>
        /// <param name="radius">Radius of the area to deal damage to</param>
        /// <param name="damage">How much damage to deal to units in the range</param>
        public void DamageRegion(int x, int y, int radius, int damage) {
            int nx1 = Utils.Clamp(0, this.width - 1, x - radius + 1);
            int nx2 = Utils.Clamp(0, this.width - 1, x + radius - 1);
            int ny1 = Utils.Clamp(0, this.height - 1, y - radius + 1);
            int ny2 = Utils.Clamp(0, this.height - 1, y + radius - 1);

            for (int h = 0; h < ny2 - ny1 + 1; h++) {
                for (int w = 0; w < nx2 - nx1 + 1; w++) {
                    this.DamageUnit(w, h, damage);
                }
            }
        }
    }
}
