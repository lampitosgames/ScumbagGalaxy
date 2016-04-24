using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ScumbagGalaxy.UI.UIUnits {
    public enum stat { health, damage, range, speed, critChance, critDamage}
    public class UIStatText : UIBox {
        //Unit whose stat is being displayed
        LiveUnit rep;
        //Which stat does this particular instance display
        stat toDisplay;

        //Constructor
        public UIStatText(GameBoard game, LiveUnit rep, stat display, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.rep = rep;
            this.toDisplay = display;
            this.mouseEvents = false;
            this.visible = true;
            this.layer = 95;
        }

        //Override the draw method
        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            //If this is the active unit
            if (Managers.UnitManager.Manager.activeUnit == this.rep) {
                //Calculate the strings to display and the highly-exact locations to display them at based on the type of stat being displayed
                string drawStringPre = "";
                int afterPre = 0;
                string drawStringSep = "//";
                int afterMid = 0;
                string drawStringVal = "";
                switch (this.toDisplay) {
                    case stat.health:
                        drawStringPre = "HP";
                        drawStringVal = this.rep.currentHealth.ToString();
                        afterPre = 8;
                        afterMid = 15;
                        break;
                    case stat.damage:
                        drawStringPre = "Dmg";
                        AttackAbility abl = (AttackAbility)this.rep.Abilities[0];
                        drawStringVal = abl.Damage.ToString();
                        afterPre = 13;
                        afterMid = 20;
                        break;
                    case stat.range:
                        drawStringPre = "Rng";
                        drawStringVal = this.rep.attackRange.ToString();
                        afterPre = 11;
                        afterMid = 18;
                        break;
                    case stat.speed:
                        drawStringPre = "Spd";
                        drawStringVal = this.rep.speed.ToString();
                        afterPre = 10;
                        afterMid = 17;
                        break;
                    case stat.critChance:
                        drawStringPre = "CrC";
                        drawStringVal = this.rep.critChance.ToString();
                        afterPre = 10;
                        afterMid = 17;
                        break;
                    case stat.critDamage:
                        drawStringPre = "CrD";
                        drawStringVal = this.rep.critDamage.ToString();
                        afterPre = 10;
                        afterMid = 17;
                        break;
                }

                //Draw the stat name
                spriteBatch.DrawString(GameBoard.font, drawStringPre,
                    new Vector2(
                        (int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale),
                        (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale)
                        ),
                    new Color(228, 220, 209), 0f, new Vector2(0, 0), (float)gameBoard.viewScale / 3f, SpriteEffects.None, 0);

                //Draw the seperator between the stat name and the stat value
                spriteBatch.DrawString(GameBoard.font, drawStringSep,
                    new Vector2(
                        (int)((afterPre + this.boundaries.X) * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale),
                        (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale)
                        ),
                    new Color(0, 135, 255), 0f, new Vector2(0, 0), (float)gameBoard.viewScale / 3f, SpriteEffects.None, 0);
                
                //Draw the stat value
                spriteBatch.DrawString(GameBoard.font, drawStringVal,
                    new Vector2(
                        (int)((afterMid + this.boundaries.X) * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale),
                        (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale)
                        ),
                    new Color(228, 220, 209), 0f, new Vector2(0, 0), (float)gameBoard.viewScale / 3f, SpriteEffects.None, 0);
            }
        }
    }
}
