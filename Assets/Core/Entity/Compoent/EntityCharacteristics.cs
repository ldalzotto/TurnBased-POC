using _Entity;

namespace _EntityCharacteristics
{
    /// <summary>
    /// A set of properties that influences <see cref="Entity"/> actions.
    /// </summary>
    public class EntityCharacteristics : AEntityComponent
    {
        public static float MAX_SPEED = 100.0f;
        public EntityCharacteristicsData EntityCharacteristicsData;

        public static EntityCharacteristics alloc(ref EntityCharacteristicsData p_entityCharacteristicsData)
        {
            EntityCharacteristics l_instance = new EntityCharacteristics();
            l_instance.EntityCharacteristicsData = p_entityCharacteristicsData;
            return l_instance;
        }
    }

    public struct EntityCharacteristicsData
    {
        /// <summary>
        /// Speed influences the position of the <see cref="Entity"/> in the TurnTimeline and how often this <see cref="Entity"/> turn occurs.
        /// </summary>
        public float Speed;
    }
}
     

