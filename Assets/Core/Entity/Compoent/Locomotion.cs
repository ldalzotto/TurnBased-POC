using _Entity;
using System;
using _Navigation;

namespace _Locomotion
{
    public class Locomotion : AEntityComponent
    {
        /// <summary>
        /// A callback that physically move the <see cref="Entity"/>.
        /// </summary>
        public Action<NavigationNode, Action<NavigationNode, NavigationNode>> MoveToNavigationNode;

        public static Locomotion alloc()
        {
            Locomotion l_instance = new Locomotion();
            return l_instance;
        }

    }
}