using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy
{
	/// <summary>An ability that modifies a unit's hitpoints</summary>
    class AttackAbility : Ability
    {
        int damage;
        public int Damage { get { return damage; }  }//How many hitpoints affected units lose (positive) or gain (negative)

		
		/// <summary>An ability that modifies a unit's hitpoints</summary>
        public AttackAbility(int damage, int range, int radius, int cost, List<Buff> buffs = null) : base(range, radius, cost)
        {
            //setup damage
            this.damage = damage;
            
            //Set what type of ability this is
            if (damage <= 0) {
                this.healAbility = true;
            } else {
                this.attackAbility = true;
            }

            //register any buffs
            if(buffs!=null)foreach(Buff b in buffs) RegisterBuff(b);
        }

        /// <summary>
        /// Damages and applies any registered buffs to the unit paramater.
        /// </summary>
        override internal void ApplyTo(LiveUnit u)
        {
            if (DoesAffect(u))
            {
                int insdamage = damage;
                base.ApplyTo(u);
                Random Crit = new Random();
                if (Crit.Next(100) <= Managers.UnitManager.Manager.activeUnit.critChance)
                {
                    insdamage = (int)((damage * Managers.UnitManager.Manager.activeUnit.critDamage) / 100);
                }
                u.ApplyDamage(insdamage);
            }
        }
    }
}
