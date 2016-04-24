using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy
{
	/// <summary> An ability that changes a unit or group of unit's position on the grid.
	/// </summary>
    public class TeleportAbility : Ability
    {
        int maxDistance;
        public int MaxDistance { get { return maxDistance; } }//How far away units may be teleporated

        public int targetX, targetY;
        public TeleportAbility(int range, int radius, int distance, int cost) : base(radius, range, cost)
        {
            this.maxDistance = distance;
            this.teleportAbility = true;
            RegisterBuff(Buff.SINGLE_TELEPORT);
            targetX = -1;
            targetY = -1;
        }
		
		/// <summary> Moves the grid position with a center at the x/y co-ordinate to the stored target position<para></para><para></para>
		/// SetTarget must be called first, or an exception will be thrown.
		/// </summary>
        public override void ApplyTo(int x, int y)
        {
			//first, apply the teleportation buff to valid units within the coordinates
			base.ApplyTo (x, y);

            //exception will be thrown if targetx&y are not set first in SetTarget
            if (targetX < 0 || targetY < 0)
                throw new Exception();
            
            //store existing state of grid at target location
            Grid temp = Grid.mainGrid.GetRegion(x, y, Radius);

            //update the position of any units that should be moved.
            for(int i = 0; i<temp.height; i++)
            {
                for(int j = 0; j<temp.width; j++)
                {
                    LiveUnit u = temp.Get(i, j);
                    LiveUnit targetDest = Grid.mainGrid.Get(x + targetX - Radius + 1 + i, y + targetY - Radius + 1 + j);
                    if (u != null && DoesAffect(u) && targetDest == null) {

                        //stack teleports
                        if (u.ContainsBuffType(BuffType.OnTeleportApplyBuff)) {
                            Buff inQuestion = u.FirstBuffOfType(BuffType.OnTeleportApplyBuff);
                            Buff toCreate = new Buff((BuffType)inQuestion.Strength, 1, inQuestion.DoesStack, 1);
                            toCreate.Title = inQuestion.Title.Substring(1);
                            u.StackBuff(toCreate);
                        }


                        //Move the UI
                        u.sprite.xInd = targetX - Radius + 1 + i;
                        u.sprite.yInd = targetY - Radius + 1 + j;
                        u.sprite.MoveTo(((targetX - Radius + 1 + i) * 63) + 6 + UI.UIGrid.margin, ((targetY - Radius + 1 + j) * 63) - 21 + UI.UIGrid.margin);
                        u.sprite.MoveSubUI();

                        //set corresponding position on target to u
                        Grid.mainGrid.Set(targetX - Radius + 1 + i, targetY - Radius + 1 + j, u);
                        //clear old position
                        Grid.mainGrid.Set(x - Radius + 1 + i, y - Radius + 1 + j, null);
                    }

                }
            }
            
            //clear target
            targetX = -1;
            targetY = -1;
        }

        /// <summary> Sets the center of the target location (to which units will be teleported to) to the x/y grid coordinates given.
        /// </summary>
        public void SetTarget(int x, int y)
        {
            targetX = x;
            targetY = y;
        }
    }

}
