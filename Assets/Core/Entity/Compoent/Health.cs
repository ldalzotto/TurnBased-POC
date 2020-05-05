
using _Entity;

namespace _Health
{
    public class Health : AEntityComponent
    {
        public HealthData HealthData;

        public static Health alloc(ref HealthData p_healthData)
        {
            Health l_instance = new Health();
            l_instance.HealthData = p_healthData;
            return l_instance;
        }

        public static void addToCurrentHealth(Health p_health, float p_delta)
        {
            p_health.HealthData.CurrentHealth += p_delta;
            if(p_health.HealthData.CurrentHealth <= 0)
            {
                Entity.markForDestruction(p_health.AssociatedEntity);
            }
        }

        public static void resetHealth(Health p_health)
        {
            p_health.HealthData.CurrentHealth = p_health.HealthData.MaxHealth;
        }

        public static float getHealthRatio(Health p_health)
        {
            return p_health.HealthData.CurrentHealth / p_health.HealthData.MaxHealth;
        }
    }

    public struct HealthData
    {
        public float MaxHealth;
        public float CurrentHealth;
    }

}
