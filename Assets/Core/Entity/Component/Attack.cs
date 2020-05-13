﻿using _Entity;
using _Health;

namespace _Attack
{
    public class Attack : AEntityComponent
    {
        public AttackData AttackData;

        public static Attack alloc(ref AttackData p_attackData)
        {
            Attack l_instance = new Attack();
            l_instance.AttackData = p_attackData;
            return l_instance;
        }

        /// <summary>
        /// Calculates damage from the <paramref name="p_attack"/>.
        /// In the future, this function will take into account <see cref="_EntityCharacteristics.EntityCharacteristics"/> for defense, ect ...
        /// </summary>
        public static float resolve(Attack p_attack, Entity p_targetEntity)
        {
            return p_attack.AttackData.Damage * -1;
        }
    }

    public struct AttackData
    {
        public float APCost;
        public float Damage;
    }
}
