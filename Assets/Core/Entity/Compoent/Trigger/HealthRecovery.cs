using _Entity;

namespace _HealthRecovery
{
    public class HealthRecovery : AEntityComponent
    {
        public HealthRecoveryData HealthRecoveryData;

        public static HealthRecovery alloc(HealthRecoveryData p_healthRecoveryData)
        {
            HealthRecovery l_instance = new HealthRecovery();
            l_instance.HealthRecoveryData = p_healthRecoveryData;
            return l_instance;
        }
    }
    public struct HealthRecoveryData
    {
        public float RecoveredHealth;
    }
}
