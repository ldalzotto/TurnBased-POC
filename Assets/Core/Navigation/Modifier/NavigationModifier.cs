using System.Collections;
using _Entity;
using _Functional;

namespace _Navigation._Modifier
{
    /// <summary>
    /// A set of properties that defines how the <see cref="Entity"/> is modifying the <see cref="NavigationGraph"/>.
    /// </summary>
    public class NavigationModifier : AEntityComponent
    {
        public NavigationModifierData NavigationModifierData;

        public static NavigationModifier alloc(ref NavigationModifierData p_navigationModifierData)
        {
            NavigationModifier l_instance = new NavigationModifier();
            l_instance.NavigationModifierData = p_navigationModifierData;
            return l_instance;
        }
    }



    public struct NavigationModifierData
    {
        /// <summary>
        /// If true, the <see cref="Entity"/> is considered as an obstacle and thus, remove the possibitlity for other Entities
        /// to move to.
        /// </summary>
        public bool IsObstacle;
    }

}
