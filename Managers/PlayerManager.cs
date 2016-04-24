using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.Managers {
    public class PlayerManager {
        //The static manager object
        public static PlayerManager Manager;

        //Player list
        public List<Player> playerList;
        //Variable to keep track of turn
        public int playerTurn;

        //Constructor
        public PlayerManager(List<Player> players) {
            this.playerList = players;
            playerTurn = 1;
            this.SwitchTurn();
        }

        public void Update() {
            Player curPlayer = this.playerList[playerTurn];

            //remove dead units
            List<LiveUnit> toRemove = new List<LiveUnit>();
            foreach (LiveUnit u in curPlayer.Units)
            {
                if (u.CurrentHealth <= 0)
                    toRemove.Add(u);
            }
            foreach (LiveUnit u in this.playerList[(playerTurn + 1) % 2].Units)
            {
                if (u.CurrentHealth <= 0)
                    toRemove.Add(u);
            }

            foreach (LiveUnit u in toRemove)
            {
                u.GetOwner().RemoveUnit(u);
            }


            //check if game over
            if (curPlayer.HasLost())
            {
                UI.UIBox winnerDisplay = new UI.UIBox(UI.GameBoard.endMenu, (UI.GameBoard.endMenu.viewWidth - 820) / 2, ((UI.GameBoard.endMenu.viewHeight - 211) / 2) - 100, 820, 211);
                if (Manager.playerTurn+1 == 0) {
                    winnerDisplay.thisSprite = UI.UIStartScreen.UIExitButton.player1Win;
                } else {
                    winnerDisplay.thisSprite = UI.UIStartScreen.UIExitButton.player2Win;
                }
                winnerDisplay.layer = 98;
                winnerDisplay.visible = true;
                winnerDisplay.mouseEvents = false;
                UI.GameBoard.activeState = 4;//game over state
                return;
            } else if (this.playerList[(playerTurn+1)%2].HasLost())
            {
                UI.GameBoard.activeState = 4;//game over state
                return;
            }

            //If all units this player has are out of actions, switch turns
            bool outOfTurns = true;
            for (int i=0; i<curPlayer.Units.Count; i++) {
                if (curPlayer.Units[i].MovesLeft > 0) {
                    outOfTurns = false;
                }
            }
            if (outOfTurns) {
                this.SwitchTurn();

                //Set the first active unit
                UnitManager.Manager.activeUnit = curPlayer.Units[0];
            }

            UnitManager.Manager.Update(this.playerList[playerTurn]);
        }

        public void SwitchTurn() {
            //Remove dead units from the unit list of the previous player
            for (int i = 0; i < playerList[playerTurn].Units.Count; i++) {
                if (playerList[playerTurn].Units[i].CurrentHealth <= 0) {
                    playerList[playerTurn].Units.RemoveAt(i);
                    i -= 1;
                }
            }

                playerTurn++;
            if (playerTurn > playerList.Count - 1) {
                playerTurn = 0;
            }

            Player curPlayer = playerList[playerTurn];

            //Loop through the upcoming player's units
            for (int i=0; i<curPlayer.Units.Count; i++) {
                //Reset all moves left on this player's units
                curPlayer.Units[i].MovesLeft = 2;
                //Set all units to having neither moved nor attacked
                curPlayer.Units[i].hasMoved = false;
                curPlayer.Units[i].hasAttacked = false;
                //Decriment buffs on all units
                curPlayer.Units[i].DecrementBuffs();

                if (curPlayer.HasLost())
                {
                    UI.GameBoard.activeState = 4;
                    return;
                }
                //apply passive abilities
                foreach (Ability a in curPlayer.Units[i].Abilities)
                {
                    //NOTE: This won't apply aura abilities properly!
                    //TODO: change to unit's position on grid as paramater.
                    if (a is PassiveAbility)
                        a.ApplyTo(curPlayer.Units[i]);
                }
            }
        }
    }
}
