using _NavigationGraph;
using Unity.Mathematics;

namespace _ActionPoint
{
    public static class Calculations
    {
        public static float actionPointBetweenNavigationNodes(NavigationNode p_navigationNode1, NavigationNode p_navigationNode2)
        {
            return worldDistanceToActionPoint(math.distance(p_navigationNode1.LocalPosition, p_navigationNode2.LocalPosition));
        }

        /// <summary>
        /// Converts a world distance to it's associated <see cref="ActionPoint"/> cost.
        /// </summary>
        public static float worldDistanceToActionPoint(float p_worldDistance)
        {
            return p_worldDistance;
        }

        /// <summary>
        /// Converts <see cref="ActionPoint"/> to it's associated world distance. This operation is the inverse of <see cref="worldDistanceToActionPoint"/>.
        /// </summary>
        public static float actionPointToCrossableWorldDistance(float p_actionPoint)
        {
            return p_actionPoint;
        }
    }

}
