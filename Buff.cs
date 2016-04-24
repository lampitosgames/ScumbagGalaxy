namespace ScumbagGalaxy
{
    /**PREFIXES**/
    //Is : Identifying attribute. Strength IS NOT of consequence.
    //Amt : Identifying attribute. Stength IS of consequence.
    //On : has an action-based conditional check. (subscribes to an event)
    //Instant : Denotes a buff of -1 initial duration which is not shown to the player. Resolved during Apply(buff).
    //Tick : Applied at the start of every turn. Convention is to be used by stats managed by "LiveUnit"
    //Flat : Applied at application/removal of buff. Convention is to be used by stats managed by "Unit"
    //Crit : Forces critical damage of all abilities.

    /**SUFFIXES**/
    //HasBuff : Strength will can be casted to reference a bufftype
    //DoTrack : Increments the screnght of another buff by 1 for each occurance of the event in question.

    /**CONVENTION**/
    //Strength as a POSITIVE of whatever attribute is listed.
    //      -When applied to percentage stats, 1 = 1%
    //
    //Buffs applied with remainingduration <0 will check any onApply effects, then will remove themselves. They will never be shown to the player.
    //
    // a "/**/" at the start of a buff means it has been implemented. Be wary that not all buffs in this list may exist.
    //To apply a managed buff which as no effects, use the "FlavorText" type.

//!!!!DO NOT REORDER THESE. to add new buffs, APPEND AT END!!
    public enum BuffType { FlavorText = -1, IsDPS = 0, IsHealer = 1, IsTank =2 , IsTeleporter = 3,//to differentiate subclasses
        InstantIsTeleporting,//applied by base teleport method
        //^remove this buff 

/**/    TickHealth, //deals -strength damage at the start of the turn (can be negative for healing)
        
/**/    InstantDamage, //Deals strength amt of damage, then removes self.
/**/    InstantHealsTaken,//Increases heals taken

/**/    FlatMovement, //modifies (increases) movement attribute by x. movement can go below -1, but game will clamp it upon attemtping motion
/**/    FlatMaxHealth, //Created for Optimist
/**/    FlatHealsTaken,
        FlatDamageDealt, //percent increase for all damage >0
        FlatHealingDealt, //percent increase for all damage <0
/**/    FlatDamageRecieved, //these are exceptions to flat convention which have no stat change when applied.
/**/    FlatHealingRecieved, //
/**/    FlatCriticalChance,
/**/    FlatCriticalDamage,

        FlatAttackRange,//This modifies the range of ALL abilities, INCLUDING base attack.

/**/    OnTeleportApplyBuff,//Created for blunderstriker. will apply a single stack thestrength casted buff with a length of 1.
        OnMovementApplyBuff,//Created for Telepriest to increment "amtMoved"(see onTeleportApplyBuff)
        AmtTeleported,//Created for blunderstriker. Every application of IsJustTeleported increases this by 1.
        AmtMoved,
        AmtTurnsPassed,//Created for bomblobber

        CritIfTargetHasBuff,
        CritIfAdjacentHasBuff,//Created for Paladin/portmage
        CritIfSelfHasBuff,

        IsNotGoingToCrit,//a zero-turn buff applied by abilities which will never crit.

        IsRejectingHeals,//Created for Shiv
        IsInvertingDamage,//Created for Stinger. Checked for after failing to determine object fits mask.

        OnCritSelfMovement, //Created for berzerker
        OnTeleportSelfMovement, //Created for blunderstriker

 /**/   IsUnselectable, //used for token units like roadbloacks to prevent thier stats from being shown. hooked into UI
 /**/   IsImmortal, //totally unaffected by abilities. one stack is removed each time
        InstantGenerateToken, //generates a token at the center of this ability's application. strength corresponds to static tokenarray.existingUnits. wall should be "0", bomb should be "1", portal sould be "2"
        InstantExplode, //created for bomblobber's bomb token. will deal x damage to all units within 3 radius of unit upon death
        FlatSelfDamageRecieved, //created for bomblobber's blastproofing

        OnDamageFlatDamage,//bomblobber
        OnDeathApplyBuff,//shade and bomblobber's bomb token
        InstantRandom50, //50% chance to apply strength casted buff w/1 strength. true for single turn. false for eternal.

        OnDamageRemoveBuff,//optomist, strength casted
        OnDealtDamageApplyBuff,//telepriest, holy fire, 2 turn duration and strength are hard-coded.
        OnDamagedApplyBuff,//telepriest,
        OnDecrementApplyBuff,//optomist/recon, strength casted. hard-coded for casted buff to have a strength of 20 
        InstantScaleOffHealsTaken,//optomist
        IsSelfSingleTurnBuffs,//optomist

        IsOverhealing,//strength = %
        IsConstantHealsTaken,//stinger

        InstantApplyBuffToActiveUnit,//created for hauler. reduce damage buff. hard coded 1-%
        IsRejectingTeleports,//created for hauler
        IsMysteriousPortal,//created for portmage
        IsCanBeFiredFromPortal,//portmage ability
        InstantFindTeleportPortal30,//portmage, on damaged passive buff strength cast target

        IsRecon,//so much unique functionality.. idgaf. just use some flavor text to make other buffs look legit
        OnCritShieldUp,//Created for Shiv.  Creates a shield to absorb one hit upon critical.
        ActsAsHeal,//Created for Optimist.  Attacks are taken as a heal.
        MovementBasedHeal,//Created for Telepriest
        TeleportBased, //Created for Blunderstriker.  Strength determines how much damage is dealt for each stock.
        TradeHealth, //Created for Paladin.  Gives health to others in exchange for its own

    }
	
	/// <summary> A buff provides auxilary information around a BuffType which allow it to be applied to and managed by live units
	/// </summary>
    public class Buff
    {
        BuffType type;//what kind of known buff this is
        public BuffType Type { get { return type; } }
        int remainingDuration;//how many turns until this buff expires. If it is not yet applied to a unit, this equals initial duration
        public int RemainingDuration { get { return remainingDuration; } set { remainingDuration = value; } }
        int strength;//numerical value for how potent this buff is. This does not need to be set if it will no tbe used
        public int Strength { get { return strength; } }
        bool doesStack;//true if applying this same buff adds strengths/remaining durations together instead of overwriting an existing buff.
        public bool DoesStack { get { return doesStack; } }
        string tooltip;//displayed with UI
        public string Tooltip { get { return tooltip; } set { tooltip = value; } }
        string title = "temp_NO_TITLE";//must be UNIQUE!
        public string Title { get { return title; } set { title = value; } }
        public static Buff SINGLE_TELEPORT = new Buff(BuffType.InstantIsTeleporting);

        //full constructor
		public Buff(BuffType t, int initialDuration,  bool doesStack, int strength = 0)
        {
            type = t;
            remainingDuration = initialDuration;
            this.strength = strength;
            this.doesStack = doesStack;
        }

        //overloaded constructors
        public Buff(BuffType t) :
            this(t, 1, false)
        { }
    }
}