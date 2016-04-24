using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UIDefaultActionButton : UIBox {
        public static Texture2D move;
        public static Texture2D moveActive;
        public static Texture2D attack;
        public static Texture2D attackActive;
        public static Texture2D passive1;
        public static Texture2D passive1Active;
        public static Texture2D passive2;
        public static Texture2D passive2Active;

        public int hOra;
        public LiveUnit lrep;

        //Text that appears on hover
        public UITextBox flavorText;

        public UIDefaultActionButton(GameBoard game, int hOra, LiveUnit unit, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.hOra = hOra;
            if (this.hOra == 0) {
                this.thisSprite = move;
            } else if (this.hOra == 1) {
                this.thisSprite = attack;
            } else if (this.hOra == 2) {
                this.thisSprite = passive1;
            } else if (this.hOra == 3) {
                this.thisSprite = passive2;
            }

            this.layer = 95;
            this.lrep = unit;
            //Create a hover text box
            if (this.hOra == 0) {
                this.flavorText = new UITextBox(game, "Move Unit // This unit can move " + (this.lrep.speed-1) + " this turn.", xPos, yPos, 89, 12);
            } else if (this.hOra == 1) {
                this.flavorText = new UITextBox(game, this.lrep.Abilities[0].Title + " // " + this.lrep.Abilities[0].Tooltip, xPos, yPos, 89, 12);
            } else if (this.hOra == 2) {
                if (this.lrep.Abilities.Count > 3) {
                    this.flavorText = new UITextBox(game, this.lrep.Abilities[3].Title + " // " + this.lrep.Abilities[3].Tooltip, xPos, yPos, 89, 12);
                }
            } else if (this.hOra == 3) {
                if (this.lrep.Abilities.Count > 4) {
                    this.flavorText = new UITextBox(game, this.lrep.Abilities[4].Title + " // " + this.lrep.Abilities[4].Tooltip, xPos, yPos, 89, 12);
                }
            }
        }

        public static void Load(ContentManager content) {
            move = content.Load<Texture2D>(@"Images/CharacterInfoElements/move");
            moveActive = content.Load<Texture2D>(@"Images/CharacterInfoElements/moveActive");
            attack = content.Load<Texture2D>(@"Images/CharacterInfoElements/attack");
            attackActive = content.Load<Texture2D>(@"Images/CharacterInfoElements/attackActive");
            passive1 = content.Load<Texture2D>(@"Images/CharacterInfoElements/passive1");
            passive1Active = content.Load<Texture2D>(@"Images/CharacterInfoElements/passive1Active");
            passive2 = content.Load<Texture2D>(@"Images/CharacterInfoElements/passive2");
            passive2Active = content.Load<Texture2D>(@"Images/CharacterInfoElements/passive2Active");
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            if (this.thisSprite == null) {
                if (this.hOra == 0) {
                    this.thisSprite = move;
                } else if (this.hOra == 1) {
                    this.thisSprite = attack;
                } else if (this.hOra == 2) {
                    this.thisSprite = passive1;
                } else if (this.hOra == 3) {
                    this.thisSprite = passive2;
                }
            }

            if (Managers.UnitManager.Manager.activeUnit == this.lrep) {
                if (this.mouseEvents == false) {
                    this.mouseEvents = true;
                }
                base.Draw(gameBoard, spriteBatch);
            } else {
                if (this.mouseEvents == true) {
                    this.mouseEvents = false;
                }
            }
        }

        public override void MoveTo(int xPos, int yPos) {
            base.MoveTo(xPos, yPos);
            if (this.flavorText != null) {
                this.flavorText.MoveTo(xPos, yPos - this.flavorText.boundaries.Height);
            }
        }

        //On mouse enter
        public override void MouseEnter() {
            if (this.lrep.MovesLeft > 0) {
                if (this.hOra == 0) {
                    if (this.lrep.hasMoved != true) {
                        this.thisSprite = moveActive;
                    }
                } else if (this.hOra == 1) {
                    this.thisSprite = attackActive;
                } else if (this.hOra == 2) {
                    this.thisSprite = passive1Active;
                } else if (this.hOra == 3) {
                    this.thisSprite = passive2Active;
                }
            }

            //Set the flavor text to visible
            if (this.flavorText != null) {
                this.flavorText.visible = true;
            }
        }

        //On mouse leave
        public override void MouseLeave() {
            if (this.hOra == 0) {
                this.thisSprite = move;
            } else if (this.hOra == 1) {
                this.thisSprite = attack;
            } else if (this.hOra == 2) {
                this.thisSprite = passive1;
            } else if (this.hOra == 3) {
                this.thisSprite = passive2;
            }

            if (this.flavorText != null) {
                //Set the flavor text to invisible
                this.flavorText.visible = false;
            }
        }

        //On mouse move
        public override void MouseMove(int prevX, int curX, int prevY, int curY, MouseState state) {
            //Reposition the flavor text
            if (this.flavorText != null) {
                this.flavorText.MoveTo(curX, curY - this.flavorText.boundaries.Height);
            }
            base.MouseMove(prevX, curX, prevY, curY, state);
        }

        public override void MouseUp(Mouse button) {
            if (button == Mouse.Left) {
                if (this.hOra == 0 && this.lrep.hasMoved == false) {
                    Managers.UnitManager.Manager.skillNum = -1;
                } else if (this.hOra == 1) {
                    Managers.UnitManager.Manager.defaultAdvance = false;
                    Managers.UnitManager.Manager.skillNum = 0;
                }
            }
        }
    }
}
