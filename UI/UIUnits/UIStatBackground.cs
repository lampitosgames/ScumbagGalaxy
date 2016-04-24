using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy.UI.UIUnits {
    public class UIStatBackground : UIBox {
        public static Texture2D statBackground;
        public static Texture2D skillBackground;
        public Rectangle skillBoundaries;
        public LiveUnit rep;
        public UIStatText health, damage, range, speed, critChance, critDamage;
        public UISkillButton active1, active2;
        public UIDefaultActionButton move, attack, passive1, passive2;

        public UIStatBackground(GameBoard game, LiveUnit rep, int xPos, int yPos, int width, int height) : base(game, xPos, yPos, width, height) {
            this.rep = rep;
            //Draw the skill boundaries window
            this.skillBoundaries = new Rectangle(xPos+1, yPos - 60, width-2, 55);
            //Create buttons for both active skills
            this.active1 = new UISkillButton(game, 1, rep.Abilities[1], this.rep, xPos + 6, yPos - 54, 83, 8);
            this.active2 = new UISkillButton(game, 2, rep.Abilities[2], this.rep, xPos + 6, yPos - 42, 83, 8);
            this.active1.visible = true;
            this.active2.visible = true;
            //Create buttons for movement, default attack, and both passives
            this.move = new UIDefaultActionButton(game, 0, rep, xPos + 6, yPos - 30, 16, 16);
            this.attack = new UIDefaultActionButton(game, 1, rep, xPos + 27, yPos - 30, 16, 16);
            this.passive1 = new UIDefaultActionButton(game, 2, rep, xPos + 49, yPos - 30, 16, 16);
            this.passive2 = new UIDefaultActionButton(game, 3, rep, xPos + 72, yPos - 30, 16, 16);
            this.move.visible = true;
            this.attack.visible = true;
            this.passive1.visible = true;
            this.passive2.visible = true;
            //Create stat text for all unit stats
            this.health = new UIStatText(game, rep, stat.health, xPos + 7, yPos + 6, 28, 28);
            this.damage = new UIStatText(game, rep, stat.damage, xPos + 33, yPos + 6, 28, 28);
            this.range = new UIStatText(game, rep, stat.range, xPos + 65, yPos + 6, 28, 28);
            this.speed = new UIStatText(game, rep, stat.speed, xPos + 4, yPos + 19, 28, 28);
            this.critChance = new UIStatText(game, rep, stat.critChance, xPos + 34, yPos + 19, 28, 28);
            this.critDamage = new UIStatText(game, rep, stat.critDamage, xPos + 64, yPos + 19, 28, 28);
        }

        public static void Load(ContentManager content) {
            statBackground = content.Load<Texture2D>(@"Images/CharacterInfoElements/statsBackground");
            skillBackground = content.Load<Texture2D>(@"Images/CharacterInfoElements/textBoxBackground");
        }

        public void ToggleVisible() {
            if (this.visible) {
                this.visible = false;
                this.health.visible = false;
                this.damage.visible = false;
                this.range.visible = false;
                this.speed.visible = false;
                this.critChance.visible = false;
                this.critDamage.visible = false;
            } else {
                this.visible = true;
                this.health.visible = true;
                this.damage.visible = true;
                this.range.visible = true;
                this.speed.visible = true;
                this.critChance.visible = true;
                this.critDamage.visible = true;
            }
        }

        public override void MoveTo(int xPos, int yPos) {
            base.MoveTo(xPos, yPos);
            this.skillBoundaries = new Rectangle(xPos+1, yPos-60, this.skillBoundaries.Width, this.skillBoundaries.Height);
            this.active1.MoveTo(xPos + 6, yPos - 54);
            this.active2.MoveTo(xPos + 6, yPos - 42);
            this.move.MoveTo(xPos + 6, yPos - 30);
            this.attack.MoveTo(xPos + 27, yPos - 30);
            this.passive1.MoveTo(xPos + 49, yPos - 30);
            this.passive2.MoveTo(xPos + 72, yPos - 30);
            this.health.MoveTo(xPos + 7, yPos + 6);
            this.damage.MoveTo(xPos + 33, yPos + 6);
            this.range.MoveTo(xPos + 65, yPos + 6);
            this.speed.MoveTo(xPos + 4, yPos + 19);
            this.critChance.MoveTo(xPos + 34, yPos + 19);
            this.critDamage.MoveTo(xPos + 64, yPos + 19);
        }

        public override void Draw(GameBoard gameBoard, SpriteBatch spriteBatch) {
            if (Managers.UnitManager.Manager.activeUnit == this.rep) {
                //Create a new rectangle for the draw position, taking into account the view.
                Rectangle drawBounds1 = new Rectangle((int)(this.boundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.boundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)(this.boundaries.Width * gameBoard.viewScale), (int)(this.boundaries.Height * gameBoard.viewScale));
                spriteBatch.Draw(statBackground, drawBounds1, Color.White);
                
                Rectangle drawBounds2 = new Rectangle((int)(this.skillBoundaries.X * gameBoard.viewScale) - (int)(gameBoard.viewX * gameBoard.viewScale), (int)(this.skillBoundaries.Y * gameBoard.viewScale) - (int)(gameBoard.viewY * gameBoard.viewScale), (int)(this.skillBoundaries.Width * gameBoard.viewScale), (int)(this.skillBoundaries.Height * gameBoard.viewScale));
                spriteBatch.Draw(skillBackground, drawBounds2, Color.White);
            }
        }
    }
}
