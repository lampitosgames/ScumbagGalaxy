using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UISkillButton : UIBox {
        public static Texture2D skill1;
        public static Texture2D skill1Active;
        public static Texture2D skill2;
        public static Texture2D skill2Active;

        public int skillNum;
        public Ability rep;
        public LiveUnit lrep;

        //Text that appears on hover
        public UITextBox flavorText;

        public UISkillButton(GameBoard game, int skillNum, Ability ability, LiveUnit unit, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.skillNum = skillNum;
            if (this.skillNum == 1) {
                this.thisSprite = skill1;
            } else if (this.skillNum == 2) {
                this.thisSprite = skill2;
            }

            this.layer = 95;
            this.rep = ability;
            this.lrep = unit;
            //Create a hover text box
            this.flavorText = new UITextBox(game, this.rep.Title + " // " + this.rep.Tooltip, xPos, yPos, width, 12);
        }

        public static void Load(ContentManager content) {
            skill1 = content.Load<Texture2D>(@"Images/CharacterInfoElements/active1");
            skill1Active = content.Load<Texture2D>(@"Images/CharacterInfoElements/active1Hover");
            skill2 = content.Load<Texture2D>(@"Images/CharacterInfoElements/active2");
            skill2Active = content.Load<Texture2D>(@"Images/CharacterInfoElements/active2Hover");
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            if (this.thisSprite == null) {
                if (this.skillNum == 1) {
                        this.thisSprite = skill1;
                } else if (this.skillNum == 2) {
                    this.thisSprite = skill2;
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
            this.flavorText.MoveTo(xPos, yPos - this.flavorText.boundaries.Height);
        }

        //On mouse enter
        public override void MouseEnter() {
            if (this.lrep.MovesLeft > 0) {
                if (this.skillNum == 1) {
                    this.thisSprite = skill1Active;
                } else if (this.skillNum == 2) {
                    this.thisSprite = skill2Active;
                }
            }

            //Set the flavor text to visible
            this.flavorText.visible = true;
        }

        //On mouse leave
        public override void MouseLeave() {
            if (this.skillNum == 1) {
                this.thisSprite = skill1;
            } else if (this.skillNum == 2) {
                this.thisSprite = skill2;
            }

            //Set the flavor text to invisible
            this.flavorText.visible = false;
        }

        //On mouse move
        public override void MouseMove(int prevX, int curX, int prevY, int curY, MouseState state) {
            //Reposition the flavor text
            this.flavorText.MoveTo(curX, curY - this.flavorText.boundaries.Height);
            base.MouseMove(prevX, curX, prevY, curY, state);
        }

        //On mouse up
        public override void MouseUp(Mouse button) {
            if (button == Mouse.Left) {
                if (this.skillNum == 1) {
                    Managers.UnitManager.Manager.defaultAdvance = false;
                    Managers.UnitManager.Manager.skillNum = 1;
                } else if (this.skillNum == 2) {
                    Managers.UnitManager.Manager.defaultAdvance = false;
                    Managers.UnitManager.Manager.skillNum = 2;
                }
            }
        }
    }
}
