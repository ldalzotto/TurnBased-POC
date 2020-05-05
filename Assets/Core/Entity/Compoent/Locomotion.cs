using _Entity;
using _NavigationGraph;
using System;

namespace _Locomotion
{
    public class Locomotion : AEntityComponent
    {
        public static void EMPTY_MOVE_TO_NAVIGATION_NODE(NavigationNode n1, Action<NavigationNode, NavigationNode> p1) { }
        public static void EMPTY_WARP(NavigationNode n1) { }

        /// <summary>
        /// A callback that physically move the <see cref="Entity"/>.
        /// </summary>
        public Action<NavigationNode, Action<NavigationNode, NavigationNode>> MoveToNavigationNode;
        public Action<NavigationNode> WarpTo;

        public static Locomotion alloc(Action<NavigationNode, Action<NavigationNode, NavigationNode>> p_moveToNavigationNode, Action<NavigationNode> p_warpTo)
        {
            Locomotion l_instance = new Locomotion();
            l_instance.MoveToNavigationNode = p_moveToNavigationNode;
            l_instance.WarpTo = p_warpTo;
            return l_instance;
        }

    }
}