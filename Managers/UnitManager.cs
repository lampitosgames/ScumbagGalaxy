using ScumbagGalaxy.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.Managers {
    public class UnitManager {
        public static UnitManager Manager;

        public Grid dataGrid;
        public UIGrid UIGrid;

        public LiveUnit activeUnit;
        public Ability activeAbility;
        public bool defaultAdvance;
        public int skillNum;

        //State variables
        bool displayingMovement;
        bool moveTargetSelected;
        bool readyMove;
        int moveTargetX, moveTargetY;
        bool attacking;
        bool attack;
        int attackHoverX, attackHoverY, attackRadius;
        bool healing;
        bool heal;
        int healHoverX, healHoverY, healRadius;
        bool teleporting;
        bool teleportTargetSelected;
        bool teleport;
        int teleportTargetX, teleportTargetY, teleportHoverX, teleportHoverY, teleportRadius;

        public UnitManager() {
            this.dataGrid = Grid.mainGrid;
            this.activeUnit = null;
            this.activeAbility = null;
            this.defaultAdvance = true;
            this.skillNum = 0;

            //Initialize all the state variables
            this.displayingMovement = false;
            this.moveTargetSelected = false;
            this.readyMove = false;
            this.attacking = false;
            this.attack = false;
            this.healing = false;
            this.heal = false;
            this.teleporting = false;
            this.teleportTargetSelected = false;
            this.teleport = false;
            this.teleportTargetX = -1;
            this.teleportTargetY = -1;
        }

        public void Update(Player curPlayer) {
            //If there is an active unit and no skill is selected, do default
            if (this.activeUnit != null) {
                if (this.defaultAdvance) {
                    //If the active unit has 2 moves left
                    switch (this.activeUnit.MovesLeft) {
                        //This unit can still move and attack, or attack twice.  Display the possible movement UI
                        case 2:
                            this.MoveDetection();
                            break;
                        case 1:
                            //If this unit has not yet moved
                            if (!this.activeUnit.hasMoved) {
                                this.MoveDetection();
                            //If this unit has not yet attacked
                            } else if (!this.activeUnit.hasAttacked) {
                                //Get the skill and set it to active if there is no active skill
                                if (this.activeAbility != this.activeUnit.Abilities[0]) {
                                    this.activeAbility = this.activeUnit.Abilities[0];
                                }
                                //Determine the type of this unit's default skill
                                //Try to cast this ability to an attack ability
                                if (this.activeUnit.Abilities[0].healAbility) {
                                    this.HealAbility(0);
                                } else if (this.activeUnit.Abilities[0].attackAbility) {
                                    this.AttackAbility(0);
                                } else if (this.activeUnit.Abilities[0].teleportAbility) {
                                    this.TeleportAbility(0);
                                }
                            }
                            break;
                        case 0:
                            //This unit has no moves left.  Highlight it as being un-selectable
                            this.UIGrid.Set(UIGridCell.CellState.Active, this.activeUnit.sprite.xInd, this.activeUnit.sprite.yInd);
                            this.UIGrid.grid[this.activeUnit.sprite.yInd, this.activeUnit.sprite.xInd].fixedState = true;
                            break;
                    }
                } else {
                    if (this.activeUnit.MovesLeft > 0) {
                        switch (this.skillNum) {
                            case 0:
                                //Get the skill and set it to active if there is no active skill
                                if (this.activeAbility != this.activeUnit.Abilities[0]) {
                                    this.activeAbility = this.activeUnit.Abilities[0];
                                }
                                //Determine the type of this unit's default skill
                                //Try to cast this ability to an attack ability
                                if (this.activeUnit.Abilities[0].healAbility) {
                                    this.HealAbility(0);
                                } else if (this.activeUnit.Abilities[0].attackAbility) {
                                    this.AttackAbility(0);
                                } else if (this.activeUnit.Abilities[0].teleportAbility) {
                                    this.TeleportAbility(0);
                                }
                                break;
                            case 1:
                                //Get the skill and set it to active if there is no active skill
                                if (this.activeAbility != this.activeUnit.Abilities[1]) {
                                    this.activeAbility = this.activeUnit.Abilities[1];
                                }
                                //Determine the type of this unit's default skill
                                //Try to cast this ability to an attack ability
                                if (this.activeUnit.Abilities[1].healAbility) {
                                    this.HealAbility(1);
                                } else if (this.activeUnit.Abilities[1].attackAbility) {
                                    this.AttackAbility(1);
                                } else if (this.activeUnit.Abilities[1].teleportAbility) {
                                    this.TeleportAbility(1);
                                }
                                break;
                            case 2:
                                //Get the skill and set it to active if there is no active skill
                                if (this.activeAbility != this.activeUnit.Abilities[2]) {
                                    this.activeAbility = this.activeUnit.Abilities[2];
                                }
                                //Determine the type of this unit's default skill
                                //Try to cast this ability to an attack ability
                                if (this.activeUnit.Abilities[2].healAbility) {
                                    this.HealAbility(2);
                                } else if (this.activeUnit.Abilities[2].attackAbility) {
                                    this.AttackAbility(2);
                                } else if (this.activeUnit.Abilities[2].teleportAbility) {
                                    this.TeleportAbility(2);
                                }
                                break;
                            default:
                                this.displayingMovement = false;
                                this.moveTargetSelected = false;
                                this.attacking = false;
                                this.attack = false;
                                this.healing = false;
                                this.heal = false;
                                this.teleporting = false;
                                this.teleportTargetSelected = false;
                                this.teleport = false;
                                this.defaultAdvance = true;
                                break;
                        }
                    }
                }
            }
            //Find dead units and deactivate them
            for (int h = 0; h < dataGrid.height; h++) {
                for (int w = 0; w < dataGrid.width; w++) {
                    if (dataGrid.Get(w, h) != null) {
                        if (dataGrid.Get(w, h).CurrentHealth <= 0) {
                            GameBoard.game.RemoveUIBox(dataGrid.Get(w, h).sprite);
                            GameBoard.game.RemoveUIBox(dataGrid.Get(w, h).sprite.healthBar);
                            GameBoard.game.RemoveUIBox(dataGrid.Get(w, h).sprite.movesLeft);
                            dataGrid.Set(w, h, null);
                        }
                    }
                }
            }
        }

        //A cell was clicked at this index, process that data and set relevant loop variables
        //The first variable shows what state the cell was in when it was clicked
        public void CellClick(UIGridCell.CellState state, int xInd, int yInd) {
            switch (state) {
                //If the clicked grid cell was Inactive
                case UIGridCell.CellState.Possible:
                    //If active unit isn't null
                    if (activeUnit != null) {
                        //If the target location is not the currently active unit, and it contains a unit
                        if ((xInd != this.activeUnit.sprite.xInd || yInd != this.activeUnit.sprite.yInd) && this.dataGrid.Get(xInd, yInd) != null) {
                            //Remove the fixed qualifiaction from the current unit's cell
                            this.UIGrid.grid[this.activeUnit.sprite.yInd, this.activeUnit.sprite.xInd].fixedState = false;

                            //Check if the unit can be selected

                            if (!this.UIGrid.dataGrid.Get(xInd, yInd).ContainsBuffType(BuffType.IsUnselectable))
                            {
                                //If it can be, set the new active unit
                                this.activeUnit = this.UIGrid.dataGrid.Get(xInd, yInd);
                            } else
                            {
                                this.activeUnit = null;
                            }
                            //Clear the UIGrid of all active states
                            this.UIGrid.Clear();
                            //Set attacking to false
                            this.attacking = false;
                            //Set displayingMovement to false
                            this.displayingMovement = false;
                            //Set moveTargetSelected to false
                            this.moveTargetSelected = false;
                        }
                    }
                    //If the target location contains no units
                    if (this.dataGrid.Get(xInd, yInd) == null) {
                        //Remove the fixed qualification from the current unit's cell
                        if (this.activeUnit != null) {
                            this.UIGrid.grid[this.activeUnit.sprite.yInd, this.activeUnit.sprite.xInd].fixedState = false;
                        }
                        //Set activeUnit to null
                        this.activeUnit = null;
                        //Clear the grid
                        this.UIGrid.Clear();
                        //Reset state variables
                        this.attacking = false;
                        this.displayingMovement = false;
                        this.moveTargetSelected = false;
                    //If the target location contains a unit
                    } else {
                        //Set the new active unit


                        if (!this.UIGrid.dataGrid.Get(xInd, yInd).ContainsBuffType(BuffType.IsUnselectable))
                        {
                            this.activeUnit = this.dataGrid.Get(xInd, yInd);
                        }
                    }
                    break;

                //If the clicked grid cell was MoveActive
                case UIGridCell.CellState.MoveActive:
                    //If the target movement location has not yet been selected, set the clicked cell to active
                    if (this.moveTargetSelected == false) {
                        this.moveTargetSelected = true;
                        this.moveTargetX = xInd;
                        this.moveTargetY = yInd;
                    
                    //Else, moveTargetSelected is true.  Make sure the clicked location is the same cell.  If it is not, move the target cell
                    } else if (xInd != this.moveTargetX || yInd != this.moveTargetY) {
                        this.UIGrid.grid[this.moveTargetY, this.moveTargetX].cellState = UIGridCell.CellState.MovePossible;
                        this.UIGrid.grid[this.moveTargetY, this.moveTargetX].fixedState = false;
                        this.moveTargetSelected = true;
                        this.moveTargetX = xInd;
                        this.moveTargetY = yInd;
                    
                    //Else, this is the second click on the target cell.  Set readyMove to true
                    } else {
                        this.readyMove = true;
                    }
                    break;

                //If the clicked grid cell was AttackActive
                case UIGridCell.CellState.AttackActive:
                    this.attack = true;
                    break;

                //If the clicked grid cell was HealActive
                case UIGridCell.CellState.HealActive:
                    this.heal = true;
                    break;

                //If the clicked grid cell was TeleportActive
                case UIGridCell.CellState.TeleportActive:
                    if (this.teleportTargetSelected == false) {
                        this.teleportTargetX = xInd;
                        this.teleportTargetY = yInd;

                        this.teleportTargetSelected = true;
                    
                    //Make sure the second location is not the same cell
                    } else if (xInd != this.teleportTargetX || yInd != this.teleportTargetY) {
                        this.teleport = true;
                    }
                    break;
            }
        }

        //When the mouse enters a cell
        public void CellEnter(UIGridCell.CellState state, int xInd, int yInd) {
            //Set the mouse hover.  Contain it to the attack range
            int range = this.activeAbility.Range;
            if (this.activeAbility.healAbility) {
                this.healHoverX = Utils.Clamp(this.activeUnit.sprite.xInd - range + 1, this.activeUnit.sprite.xInd + range - 1, xInd);
                this.healHoverY = Utils.Clamp(this.activeUnit.sprite.yInd - range + 1, this.activeUnit.sprite.yInd + range - 1, yInd);
                //Else, this is an attack ability
            } else if (this.activeAbility.attackAbility) {
                this.attackHoverX = Utils.Clamp(this.activeUnit.sprite.xInd - range + 1, this.activeUnit.sprite.xInd + range - 1, xInd);
                this.attackHoverY = Utils.Clamp(this.activeUnit.sprite.yInd - range + 1, this.activeUnit.sprite.yInd + range - 1, yInd);
            } else if (this.activeAbility.teleportAbility) {
                this.teleportHoverX = Utils.Clamp(this.activeUnit.sprite.xInd - range + 1, this.activeUnit.sprite.xInd + range - 1, xInd);
                this.teleportHoverY = Utils.Clamp(this.activeUnit.sprite.yInd - range + 1, this.activeUnit.sprite.yInd + range - 1, yInd);
            }
        }

        //When the mouse exits a cell
        public void CellLeave(UIGridCell.CellState state, int xInd, int yInd) {
            //Reset cell states surrounding the left cell
            if (this.activeAbility.healAbility) {
                    this.UIGrid.SetRegion(UIGridCell.CellState.Inactive, xInd, yInd, this.healRadius);
            } else if (this.activeAbility.attackAbility) {
                this.UIGrid.SetRegion(UIGridCell.CellState.Inactive, xInd, yInd, this.attackRadius);
            } else if (this.activeAbility.teleportAbility) {
                this.UIGrid.SetRegion(UIGridCell.CellState.Inactive, xInd, yInd, this.teleportRadius);
            }
        }

        public void AttackAbility(int index) {
            //Get the Active Unit's loaction
            int aux = this.activeUnit.sprite.xInd;
            int auy = this.activeUnit.sprite.yInd;
            //If this unit is not yet attacking, display the attacking UI
            if (!this.attacking) {
                //Clear the grid
                this.UIGrid.Clear();
                //Get the attack radius
                this.attackRadius = this.activeUnit.Abilities[index].Radius;
                //Draw attacking possibilities
                //Set the possible attack region
                this.UIGrid.SetRegion(UIGridCell.CellState.AttackPossible, aux, auy, this.activeUnit.Abilities[index].Range);
                //Default the attack hover to the unit's location
                this.attackHoverX = aux;
                this.attackHoverY = auy;
                //Set the cell under the active unit to active attack
                this.UIGrid.Set(UIGridCell.CellState.AttackActive, aux, auy);
                this.UIGrid.grid[auy, aux].fixedState = true;
                //Set attacking to true
                this.attacking = true;
            } else {
                //If the user clicked to start the attack
                if (this.attack) {
                    this.activeUnit.Abilities[index].ApplyTo(this.attackHoverX, this.attackHoverY);
                    this.UIGrid.Clear();
                    this.attacking = false;
                    this.attack = false;
                    this.activeUnit.hasAttacked = true;
                    this.activeUnit.MovesLeft -= 1;
                    this.skillNum = -1;
                //Else, if they are still trying to attack
                } else {
                    //Set the possible attack region
                    this.UIGrid.SetRegion(UIGridCell.CellState.AttackPossible, aux, auy, this.activeUnit.Abilities[index].Range);
                    //Set cells covered by the AOE to AttackActive
                    this.UIGrid.SetRegion(UIGridCell.CellState.AttackActive, this.attackHoverX, this.attackHoverY, this.attackRadius);
                }
            }

        }

        public void HealAbility(int index) {
            //Get the Active Unit's loaction
            int aux = this.activeUnit.sprite.xInd;
            int auy = this.activeUnit.sprite.yInd;
            //If this unit is not yet healing, display the healing UI
            if (!this.healing) {
                //Clear the grid
                this.UIGrid.Clear();
                //Get the heal radius
                this.healRadius = this.activeUnit.Abilities[index].Radius;
                //Draw healing possibilities
                //Set the possible heal region
                this.UIGrid.SetRegion(UIGridCell.CellState.HealPossible, aux, auy, this.activeUnit.Abilities[index].Range);
                //Default the heal hover to the unit's location
                this.healHoverX = aux;
                this.healHoverY = auy;
                //Set the cell under the active unit to active heal
                this.UIGrid.Set(UIGridCell.CellState.HealActive, aux, auy);
                this.UIGrid.grid[auy, aux].fixedState = true;
                //Set healing to true
                this.healing = true;
            } else {
                //If the user clicked to start the heal
                if (this.heal) {
                    this.activeUnit.Abilities[index].ApplyTo(this.healHoverX, this.healHoverY);
                    this.UIGrid.Clear();
                    this.healing = false;
                    this.heal = false;
                    this.activeUnit.hasAttacked = true;
                    this.activeUnit.MovesLeft -= 1;
                    this.skillNum = -1;
                    //Else, if they are still trying to heal
                } else {
                    //Set the possible heal region
                    this.UIGrid.SetRegion(UIGridCell.CellState.HealPossible, aux, auy, this.activeUnit.Abilities[index].Range);
                    //Set cells covered by the AOE to HealActive
                    this.UIGrid.SetRegion(UIGridCell.CellState.HealActive, this.healHoverX, this.healHoverY, this.healRadius);
                }
            }
        }

        public void TeleportAbility(int index) {
            //Get the active unit's location
            int aux = this.activeUnit.sprite.xInd;
            int auy = this.activeUnit.sprite.yInd;
            //If this unit is not yet teleporting, display the teleporting UI
            if (!this.teleporting) {
                //Clear the grid
                this.UIGrid.Clear();
                //Get the teleport radius from the skill
                this.teleportRadius = this.activeUnit.Abilities[index].Radius;
                //Draw teleporting possibilities
                //Set the possible teleport region
                this.UIGrid.SetRegion(UIGridCell.CellState.TeleportPossible, aux, auy, this.activeUnit.Abilities[index].Range);
                //Default the teleport hover to the unit's location
                this.teleportHoverX = aux;
                this.teleportHoverY = auy;
                //Set teleporting to true
                this.teleporting = true;
            } else {
                //If the user clicked to start the teleport
                if (this.teleport) {
                    TeleportAbility ability = (TeleportAbility)this.activeUnit.Abilities[index];
                    ability.SetTarget(this.teleportHoverX, this.teleportHoverY);
                    ability.ApplyTo(this.teleportTargetX, this.teleportTargetY);
                    this.UIGrid.Clear();
                    this.teleporting = false;
                    this.teleport = false;
                    this.teleportTargetSelected = false;
                    this.teleportTargetX = -1;
                    this.teleportTargetY = -1;
                    this.activeUnit.hasAttacked = true;
                    this.activeUnit.MovesLeft -= 1;
                    this.skillNum = -1;
                    //Else, if they are still trying to teleport
                } else {
                    //Set the possible teleport region
                    this.UIGrid.SetRegion(UIGridCell.CellState.TeleportPossible, aux, auy, this.activeUnit.Abilities[index].Range);
                    //Set cells covered by the AOE to TeleportActive
                    this.UIGrid.SetRegion(UIGridCell.CellState.TeleportActive, this.teleportHoverX, this.teleportHoverY, this.teleportRadius);
                    if (this.teleportTargetX > 0) {
                        //Set cells in the target area to teleport active
                        this.UIGrid.SetRegion(UIGridCell.CellState.TeleportActive, this.teleportTargetX, this.teleportTargetY, this.teleportRadius);
                    }
                }
            }
        }

        public void MoveDetection() {
            //Get the Active Unit's loaction
            int aux = this.activeUnit.sprite.xInd;
            int auy = this.activeUnit.sprite.yInd;
            //Display possible movement
            if (this.displayingMovement == false) {
                //Set the possible movement region to possible movement
                this.UIGrid.SetRegion(UIGridCell.CellState.MovePossible, aux, auy, this.activeUnit.speed);
                //Set the cell under the active unit to active movement
                this.UIGrid.Set(UIGridCell.CellState.MoveActive, aux, auy);

                //Set the central grid cell to fixed
                this.UIGrid.grid[auy, aux].fixedState = true;

                this.displayingMovement = true;

            //If movement is already being displayed, check for a click within the movement radius
            } else {
                //If an inactive location was selected
                if (this.moveTargetSelected) {
                    //Make sure that this location is valid
                    //If this location contains another unit, set that unit to the active unit instead of running default code
                    if (this.UIGrid.dataGrid.Get(this.moveTargetX, this.moveTargetY) != null) {
                        //Remove the fixed state from the current grid cell
                        this.UIGrid.grid[auy, aux].fixedState = false;
                        //Set the new active unit

                        if (!this.UIGrid.dataGrid.Get(this.moveTargetX, this.moveTargetY).ContainsBuffType(BuffType.IsUnselectable))
                        {
                            this.activeUnit = this.UIGrid.dataGrid.Get(this.moveTargetX, this.moveTargetY);
                        } else
                        {
                            this.activeUnit = null;
                        }
                        //Clear the UIGrid of all active states
                        this.UIGrid.Clear();
                        //Set displayingMovement to false
                        this.displayingMovement = false;
                        //Set moveTargetSelected to false
                        this.moveTargetSelected = false;

                        //Else, this is a valid move location
                    } else {
                        //Set the grid cell to MoveActive, and set it's state to fixed
                        this.UIGrid.grid[this.moveTargetY, this.moveTargetX].cellState = UIGridCell.CellState.MoveActive;
                        this.UIGrid.grid[this.moveTargetY, this.moveTargetX].fixedState = true;
                        /*
                            DRAW THE SHORTEST PATH FROM THE UNIT POSITION TO THE SELECTED POSITION
                        */

                        //If this was the second click at this location
                        if (this.readyMove) {
                            //Remove the fixed qualification from the active unit's current cell
                            this.UIGrid.grid[auy, aux].fixedState = false;
                            //Remove the fixed qualification from the target cell
                            this.UIGrid.grid[this.moveTargetY, this.moveTargetX].fixedState = false;
                            //Move the unit and de-increment it's MovesLeft variable
                            this.UIGrid.Move(aux, auy, this.moveTargetX, this.moveTargetY);
                            this.activeUnit.sprite.MoveSubUI();
                            this.activeUnit.MovesLeft -= 1;
                            this.displayingMovement = false;
                            this.moveTargetSelected = false;
                            this.readyMove = false;
                            //Clear the grid, allowing other draw code to take over
                            this.UIGrid.Clear();
                            //Show that this unit has already moved
                            this.activeUnit.hasMoved = true;
                        }
                    }
                }
            }
        }
    }
}
