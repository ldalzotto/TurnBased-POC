using _Entity;

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
    }

    public struct AttackData
    {
        public float APCost;
        public float Damage;
    }
}
