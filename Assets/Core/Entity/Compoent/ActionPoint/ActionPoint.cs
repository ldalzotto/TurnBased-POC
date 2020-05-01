using _Entity;
using Unity.Mathematics;

namespace _ActionPoint
{
    public class ActionPoint : AEntityComponent
    {
        public ActionPointData ActionPointData;

        public static ActionPoint alloc(ref ActionPointData p_actionPointData)
        {
            ActionPoint l_instance = new ActionPoint();
            l_instance.ActionPointData = p_actionPointData;
            return l_instance;
        }

        public static void add(ActionPoint p_actionPoint, float p_delta)
        {
            p_actionPoint.ActionPointData.CurrentActionPoints = math.max(p_actionPoint.ActionPointData.CurrentActionPoints + p_delta, 0.0f);
        }

        public static void resetActionPoints(ActionPoint p_actionPoint)
        {
            p_actionPoint.ActionPointData.CurrentActionPoints = p_actionPoint.ActionPointData.InitialActionPoints;
        }
    }

    public struct ActionPointData
    {
        public float InitialActionPoints;
        public float CurrentActionPoints;
    }

}
