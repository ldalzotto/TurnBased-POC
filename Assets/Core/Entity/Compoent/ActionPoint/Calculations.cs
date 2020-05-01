using _Navigation;
using Unity.Mathematics;

namespace _ActionPoint
{
    public static class Calculations
    {
        public static float actionPointBetweenNavigationNodes(NavigationNode p_navigationNode1, NavigationNode p_navigationNode2)
        {
            return math.distance(p_navigationNode1.LocalPosition, p_navigationNode2.LocalPosition);
        }
    }

}
