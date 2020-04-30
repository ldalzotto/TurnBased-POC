using _Entity;

namespace _EntityCharacteristics
{
    /// <summary>
    /// A set of properties that influences <see cref="Entity"/> actions.
    /// </summary>
    public class EntityCharacteristics : AEntityComponent
    {
        public static float MAX_SPEED = 100.0f;
        /// <summary>
        /// Speed influences the position of the <see cref="Entity"/> in the TurnTimeline and how often this <see cref="Entity"/> turn occurs.
        /// </summary>
        public float Speed;
    }
}
     

