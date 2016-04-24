using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy {
    static class Utils {
        /// <summary>
        /// Determine if a value is within a range (inclusive)
        /// </summary>
        /// <param name="min">Low-end of the range</param>
        /// <param name="max">High-end of the range</param>
        /// <param name="value">Value to be tested</param>
        public static bool InRange(int min, int max, int value) {
            return value >= Math.Min(min, max) && value <= Math.Max(min, max);
        }
        
        /// <summary>
        /// Detect if one range is fully in another range (inclusive)
        /// </summary>
        /// <param name="min0">Low-end of the larger range</param>
        /// <param name="max0">High-end of the larger range</param>
        /// <param name="min1">Low-end of the smaller range</param>
        /// <param name="max1">High-end of the smaller range</param>
        public static bool RangeContains(int min0, int max0, int min1, int max1) {
            return Math.Max(min0, max0) >= Math.Max(min1, max1) &&
                   Math.Min(min0, max0) <= Math.Min(min1, max1);
        }
        
        /// <summary>
        /// Detects if a point is inside of a grid
        /// </summary>
        /// <param name="x">Point x position</param>
        /// <param name="y">Point y position</param>
        /// <param name="gridWidth">Width of the grid</param>
        /// <param name="gridHeight">Height of the grid</param>
        public static bool InGrid(int x, int y, int gridWidth, int gridHeight) {
            if (Utils.InRange(0, gridWidth-1, x) && Utils.InRange(0, gridHeight-1, y)) {
                return true;
            } else {
                return false;
            }
        }
        
        /// <summary>
        /// Restricts an integer value to a given range
        /// </summary>
        /// <param name="min">Lower end of the range</param>
        /// <param name="max">Upper end of the range</param>
        /// <param name="value">Value to clamp</param>
        public static int Clamp(int min, int max, int value) {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Restricts a double value to a given range
        /// </summary>
        /// <param name="min">Lower end of the range</param>
        /// <param name="max">Upper end of the range</param>
        /// <param name="value">Value to clamp</param>
        public static double ClampDouble(double min, double max, double value) {
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Sorts a list of UIBoxes by layer.  Nothing is returned, it sorts the original List object
        /// </summary>
        /// <param name="data">List to sort</param>
        public static void SortUIBoxList(List<UI.UIBox> data) {
            for (int i = 0; i < data.Count; i++) {
                for (int k = i; (k >= 1) && (data[k].layer < data[k - 1].layer); k--) {
                    SwapUIBox(data, k, k - 1);
                }
            }
        }
        /// <summary>
        /// Swaps two items in a UIBox list
        /// </summary>
        /// <param name="arr">UIBox list with items to swap</param>
        /// <param name="targ">Index of the item to swap with dest</param>
        /// <param name="dest">Index of the item to swap with targ</param>
        public static void SwapUIBox(List<UI.UIBox> arr, int targ, int dest) {
            UI.UIBox temp = arr[dest];
            arr[dest] = arr[targ];
            arr[targ] = temp;
        }

        public static float AsPercent(int val)
        {
            return (val / 100.0f);
        }
    }
}
