using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScumbagGalaxy
{
    class TokenUnit : LiveUnit
    {
        public LiveUnit creator;

        //called at the start of the turn to prevent this item from performing actions
        public override void DecrementBuffs()
        {
            MovesLeft = 0;
            hasAttacked = true;
            hasMoved = true;
            base.DecrementBuffs();
        }

        public TokenUnit(LiveUnit creator, OwnerPlayer owner, int health = 9999) : base(health, owner)
        {
            this.creator = creator;

            ownerPlayer = (creator==null? OwnerPlayer.None : creator.ownerPlayer);
            
            MovesLeft = 0;
            hasAttacked = true;
            hasMoved = true;

            //NOTE: logic does not support tokens owned by multiple players. can't see why we'd need them, so DON'T MAKE THEM!
            if(ownerPlayer != OwnerPlayer.None)
            GetOwner().AddUnit(this);

        }
    }
}
