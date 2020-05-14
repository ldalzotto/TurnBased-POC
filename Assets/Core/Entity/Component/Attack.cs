using _Entity;
using _EventQueue;

namespace _Attack
{
    public class Attack : AEntityComponent
    {
        public AttackData AttackData;

        /// <summary>
        /// <see cref="AEvent"/> hooked just before applying damage from the <see cref="Attack"/>.
        /// </summary>
        public IAttackBeforeDamageEventHook AttackBeforeDamageEventHook;

        public static Attack alloc(ref AttackData p_attackData, IAttackBeforeDamageEventHook p_attackBeforeDamageEventHook)
        {
            Attack l_instance = new Attack();
            l_instance.AttackData = p_attackData;
            l_instance.AttackBeforeDamageEventHook = p_attackBeforeDamageEventHook;
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

    public interface IAttackBeforeDamageEventHook : IEventHook
    {
        Entity SourceEntity { get; set; }
    }
}
