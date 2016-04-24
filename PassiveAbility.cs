using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy
{
	/// <summary> An ability which, at the start of its unit's owner's turn, applies a buff to the location centered at the unit's x/y position.
	/// </summary>
    public class PassiveAbility : Ability
    {
        public PassiveAbility(Buff b) : this(b, 1) { }
        public PassiveAbility(Buff b, int radius) : base(radius, radius,-1)//range as radius is only used for visual purposes.
        {
            RegisterBuff(b);
        }
    }
}
