using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ScumbagGalaxy
{
    public class Unit
    {
        //static variables
        protected static readonly float MIN_HEALS_TAKEN = .2f;

        //numerical values
        public int maxHealth, speed;
        public int attackRange, attackDamage;
        public float critChance, critDamage, baseHealsTaken;
        public string name, description, unitClass;

        //aesthetics
        Texture2D sprite, portriat;
        public Texture2D Portriat { get; set; }
        public Texture2D Sprite { get; set; }
        //abilites
        List<Ability> abilities;

        public List<Ability> Abilities { get; set; }
    }
}