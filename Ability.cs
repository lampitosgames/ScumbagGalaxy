using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ScumbagGalaxy
{
    /// <summary>These are the possible ability statuses
    /// <para>Passive: reserved for passive abilities<para></para>Available: the ability can be used without any additional restrictions</para><para>Unavailable: the ability can not be used until a condition is reevaluated</para>
    /// </summary>
	public enum AbilityStatus { Unavailable, Available = 0, Passive }

	/// <summary>The ability class provides common functionality for attacks, teleportation, and passives 
	/// <para></para>
	/// </summary>
    public abstract class Ability
    {
        int range, radius, cost;
        public bool attackAbility;
        public bool healAbility;
        public bool teleportAbility;
        public String Title { get; set; }
        public String Tooltip { get; set; }
        public int status { get; set; }
        public int Range { get { return range; } }//How many spaces away from the character it may be casted
		public int Radius { get { return radius; } }//How many spaces around its center are affected
		public int Cost { get { return cost; } }//How much energy must be expended to use this ability
        List<Buff> appliedBuffs;//Any buffs that are applied to affected units
        public List<Buff> AppliedBuffs { get { return appliedBuffs; } set { appliedBuffs = value; } }
        public Texture2D AbTexture { get; set; }
        public string AbTexturePath { get; set; }

        internal OwnerPlayer validTargetMask = OwnerPlayer.Any;//Units owned by these players can be affected by this ability


        public Ability(int range, int radius, int cost)
        {
            this.radius = radius;
            this.range = range;
            this.status = status;
            this.attackAbility = false;
            this.healAbility = false;
            this.teleportAbility = false;
            appliedBuffs = new List<Buff>();
        }

		/// <summary>Returns an enum representing whether the ability can be used or not or if the ability is passive.
		/// </summary>
        public AbilityStatus GetStatus()
        {
			if (cost == -1) {
				return AbilityStatus.Passive;
			} else
				//TODO: check for any buffs on unit that could prevent usage of ability/increase cost.
				return AbilityStatus.Available;//TODO: Change status from available to unavailable if cost exceeds available resources.
        }

		/// <summary>
		/// Applies the ability to valid targets within an area with the x/y coordinates of the grid as its center  
		/// </summary>
        virtual public void ApplyTo(int x, int y)
        {
            ApplyTo(Grid.mainGrid.GetRegion(x, y, radius));
        }

		/// <summary>
		/// Returns true if the ability is allowed to be casted with its center at the given x/y position on the grid
		/// </summary>
		private bool CanBeAppliedTo(int x, int y)
		{
			return true;
		}
		
		/// <summary>
		/// Returns true if there is a valid target within the grid
		/// <para></para> Can be called on every square of a grid portion to hilight valid targets.
		/// </summary>
		private bool ContainsValidTarget(Grid g)
		{
			return true;
		}

		/// <summary>
		/// Applies any registered buffs to any valid units within the grid segment.<para></para>Additional funcationality is present in the overridden methods. 
		/// </summary>
        virtual internal void ApplyTo(Grid g)
        {
            foreach(LiveUnit u in g._Grid)
            {
                if (u != null)
                {
                    if(DoesAffect(u))
                        ApplyTo(u);
                }
            }
        }
		
		/// <summary>
		/// Returns true if the ability will be applied to the unit
		/// </summary>
        internal bool DoesAffect(LiveUnit u)
        {
            if (u.ContainsBuffType(BuffType.IsImmortal))
                return false;
            OwnerPlayer targets = validTargetMask & u.OwnerPlayer;
            return targets == u.OwnerPlayer;
        }
		
		/// <summary>
		/// Applies any registered buffs to the unit.<para></para>Additional funcationality is present in the overridden methods. 
		/// </summary>
        virtual internal void ApplyTo(LiveUnit u)
        {
            if (appliedBuffs.Count > 0)//skips if no buffs applied
                foreach (Buff b in appliedBuffs)
            {
                u.StackBuff(b);
            }
        }

		/// <summary>
		/// Adds the buff to the list of registered buffs, to be applied to any units as they are affected by this ability.
		/// </summary>
        internal void RegisterBuff(Buff b)
        {
            appliedBuffs.Add(b);
        }
    }
}
