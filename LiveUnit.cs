using System;
using System.Collections.Generic;
using System.Linq;

namespace ScumbagGalaxy
{
    //Stores additional information required to allow the unit to "live" on the board
    public class LiveUnit : Unit
    {
        //UI object representing this unit
        public UI.UIUnits.UIUnit sprite;

        //Movement status
        public bool hasMoved, hasAttacked;

        //int 
        internal int currentHealth = 0;
        public int CurrentHealth { get { return currentHealth; } }
        float currentHealsTaken = 0;
        int movesleft = 0;
        string spritePath = "";
        string portraitpath = "";
        public string SpritePath { get { return spritePath; } set { spritePath = value; } }
        public string PortraitPath { get { return portraitpath; } set { portraitpath = value; } }
        public int MovesLeft { get { return movesleft; } set { movesleft = value; } }
        //byte
        internal OwnerPlayer ownerPlayer;
        public OwnerPlayer OwnerPlayer { get { return ownerPlayer; } }

        //buff
        Dictionary<String, Buff> buffs;//string-based dictionary
        private int v;

        public List<Buff> Buffs { get { return buffs.Values.ToList(); } }
        public LiveUnit(int maxHealth, OwnerPlayer owner)
        {
            this.maxHealth = maxHealth;
            this.ownerPlayer = owner;
            currentHealth = maxHealth;
            currentHealsTaken = baseHealsTaken;
            this.Abilities = new List<Ability>();
            this.buffs = new Dictionary<string, Buff>();

            //The unit has moved and has attacked by default
            this.hasMoved = true;
            this.hasAttacked = true;
        }

        public void ApplyDamage(int damage)
        {
            if (damage <= 0)
            {
                //heal functionality
                float multiplier = currentHealsTaken + Utils.AsPercent(StrengthOfBuffType(BuffType.FlatHealingRecieved));
                currentHealth -= Math.Min((int)Math.Floor(damage * multiplier),-1);
            } else
            {
                float damageModifier = 1;// + Utils.AsPercent(StrengthOfBuffType(BuffType.FlatDamageRecieved));
                currentHealth -= (int)(damage * damageModifier);
            }
            currentHealth = Math.Max(Math.Min(currentHealth, maxHealth), 0);//increased max health buff requires special functionality here.

            //deregister self here
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //special functionality for shade
                if(ContainsBuffType(BuffType.OnDeathApplyBuff))
                {
                    Buff inQuestion = FirstBuffOfType(BuffType.OnDeathApplyBuff);
                    buffs.Remove(inQuestion.Title);
                    StackBuff(new Buff((BuffType)inQuestion.Strength, 1, false, -1));
                }
                //moved to update loop to prevent concurent modification
            }
        }

        public Player GetOwner()
        {
            switch (ownerPlayer)
            {
                case OwnerPlayer.One: return ScumbagGalaxy.Managers.PlayerManager.Manager.playerList[0];
                case OwnerPlayer.Two: return ScumbagGalaxy.Managers.PlayerManager.Manager.playerList[1];
            }
            throw new Exception("Owner player not assigned");
        }

        public int StrengthOfBuffType(BuffType type)
        {
            int total = 0;
            foreach (Buff b in buffs.Values)
            {
                if (b.Type == type)
                    total += b.Strength;
            }
            return total;
        }

        //returns a list of all buffs that should be displayed
        public List<Buff> GetBuffs()
        {
            List<Buff> buffsToDisplay = new List<Buff>();
            foreach (Buff b in buffs.Values)
            {
                if (((int)b.Type <= (int)BuffType.IsTeleporter && (int)b.Type != 0)&&b.Type!=BuffType.IsRecon)
                    ;else
                    buffsToDisplay.Add(b);
            }

            return buffsToDisplay;
        }

        public bool ContainsBuffType(BuffType t)
        {
            return (FirstBuffOfType(t) != null);
        }

        public Buff FirstBuffOfType(BuffType type)
        {
            //bask in the glory of my linear search
            foreach (Buff b in buffs.Values)
            {
                if (b.Type == type)
                    return b;
            }
            return null;
        }
        public void StackBuff(Buff b)
        {
            //check for instant buffs (these immediatley expire so don't add them)
            if (b.Type == BuffType.InstantDamage)
            {
                ApplyDamage(b.Strength);
                return;
            }
            if (b.Type == BuffType.InstantHealsTaken)
            {
                currentHealsTaken += ((float)b.Strength) / 100.0f;//TODO: ensure this does float math

                //clamp within
                if (currentHealsTaken < Unit.MIN_HEALS_TAKEN)
                    currentHealsTaken = Unit.MIN_HEALS_TAKEN;
                return;
            }

            //search for buff
            if (!buffs.ContainsKey(b.Title))
            {
                //case: not contained
                buffs.Add(b.Title, b);

                OnBuffApplied(b);
                return;
            }
            //else

            //housekeeping for if they don't have the same strength
            OnBuffExpired(buffs[b.Title]);
            OnBuffApplied(b);

            //update buff
            if (buffs[b.Title].DoesStack)
            {
                int temp = buffs[b.Title].RemainingDuration;
                buffs[b.Title] = b;
                buffs[b.Title].RemainingDuration += temp;
            } else
            {
                //if name same: replace existing buff
                buffs[b.Title] = b;
            }
        }
        public virtual void DecrementBuffs()
        {
            List<string> keysToRemove = new List<string>();
            foreach (Buff b in buffs.Values)
            {
                b.RemainingDuration--;
                if (b.RemainingDuration <= 0)
                    keysToRemove.Add(b.Title);

                //tick buffs
                if(b.Type==BuffType.TickHealth)
                {
                    ApplyDamage(b.Strength);
                }
            }
            foreach (string key in keysToRemove)
            {
                OnBuffExpired(buffs[key]);
                buffs.Remove(key);
            }
        }

        //check for flat buffs
        private void OnBuffApplied(Buff b, bool applied = true)
        {
            //whether we're adding or subtracting
            int amt = b.Strength * (applied ? 1 : -1);
            
            switch(b.Type)
            {
                case BuffType.FlatCriticalDamage: critDamage += amt; break;
                case BuffType.FlatCriticalChance: critChance += amt; break;
                case BuffType.FlatHealsTaken: currentHealsTaken += amt; break;
                case BuffType.FlatMaxHealth: maxHealth += amt; currentHealth = Math.Min(maxHealth, currentHealth); break;
                case BuffType.FlatMovement: speed += amt; break;
            }
        }
        private void OnBuffExpired(Buff b)
        {
            OnBuffApplied(b, false);
        }
    }
}