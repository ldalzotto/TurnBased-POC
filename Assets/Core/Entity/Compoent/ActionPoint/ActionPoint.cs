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
            ActionPointData.add(ref p_actionPoint.ActionPointData, p_delta);
        }

        public static void resetActionPoints(ActionPoint p_actionPoint)
        {
            ActionPointData.resetActionPoints(ref p_actionPoint.ActionPointData);
        }
    }

    public struct ActionPointData
    {
        public float InitialActionPoints;
        public float CurrentActionPoints;

        public static void add(ref ActionPointData p_actionPointData, float p_delta)
        {
            p_actionPointData.CurrentActionPoints = math.max(p_actionPointData.CurrentActionPoints + p_delta, 0.0f);
        }
        public static void resetActionPoints(ref ActionPointData p_actionPointData)
        {
            p_actionPointData.CurrentActionPoints = p_actionPointData.InitialActionPoints;
        }
    }

}
