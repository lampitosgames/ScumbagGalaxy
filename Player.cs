using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy
{
    //this is used for bitwise masking; values must be in binary
    public enum OwnerPlayer { None = 0, One = 1, Two = 2, Any = 255};
    //The player class holds all data related to a single user's dynamic gameplay.
    public class Player
    {
        //units under ownership of the player
        public List<LiveUnit> Units;
        //determines if first player. Using enums incase we extend to support more users at a later data.
        OwnerPlayer playerNum;

        //Name of this player
        public string name;

        //Constructor
        public Player(OwnerPlayer num)
        {
            this.playerNum = num;
            Units = new List<LiveUnit>();
        }

        //Determines gameOver
        public bool HasLost()
        {
            foreach(LiveUnit u in Units)
            {
                if (!(u is TokenUnit))
                    return false;
            }
            return true;
        }
        public void AddUnit(LiveUnit u)
        {
            Units.Add(u);
            //... additional code to hook it up to the ui
        }
        public void RemoveUnit(LiveUnit u)
        {
            Units.Remove(u);
            //... additional code to clear it from the ui

            //... check for game over (HasLost)
        }
    }
}
