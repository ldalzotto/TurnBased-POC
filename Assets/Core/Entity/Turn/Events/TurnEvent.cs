using System.Collections;
using _EventQueue;
using _TurnTimeline;
using _ActionPoint;
using _GameLoop;
using _Functional;
using _EventQueue._Events;

namespace _Entity._Turn
{
    public class StartTurnEvent : AEvent
    {
        public TurnTimeline TurnTimeline;

        public static StartTurnEvent alloc(TurnTimeline p_turnTimeline)
        {
            StartTurnEvent l_instance = new StartTurnEvent();
            l_instance.TurnTimeline = p_turnTimeline;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            Entity l_nextTurnEntity = TurnTimelineAlgorithm.IncrementTimeline(TurnTimeline);

           // ExternalHooks.LogDebug(l_nextTurnEntity.GetHashCode().ToString());

            if (l_nextTurnEntity != null)
            {
                ActionPoint.resetActionPoints(EntityComponent.get_component<ActionPoint>(l_nextTurnEntity));

                EventQueue.enqueueEvent(p_eventQueue, IterateAndWaitForEmptyQueue.alloc(EventQueueContainer.EntityActionQueue));
                EventQueue.enqueueEvent(p_eventQueue, WaitForNextFrame.alloc());
                EventQueue.enqueueEvent(p_eventQueue, StartTurnEvent.alloc(TurnTimeline));

                EventQueue.enqueueEvent(EventQueueContainer.EntityActionQueue, StartEntityTurnEvent.alloc(l_nextTurnEntity));
            }
        }
    }

}
