﻿using System.Collections;
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

            if (l_nextTurnEntity != null)
            {
                ActionPoint.resetActionPoints(EntityComponent.get_component<ActionPoint>(l_nextTurnEntity));
                EventQueue.insertEventAt(p_eventQueue, 0, StartEntityTurnEvent.alloc(l_nextTurnEntity));
            }
        }
    }

    public class OnEntityTurnEndEventListener : AEventListener<EndEntityTurnEvent>
    {
        public TurnTimeline TurnTimeline;

        public static OnEntityTurnEndEventListener alloc(TurnTimeline p_turnTimeline)
        {
            OnEntityTurnEndEventListener l_instance = new OnEntityTurnEndEventListener();
            l_instance.TurnTimeline = p_turnTimeline;
            return l_instance;
        }

        public override void OnEventExecuted(EventQueue p_eventQueue, EndEntityTurnEvent p_event)
        {
            EventQueue.clearAll(p_eventQueue);
            EventQueue.insertEventAt(p_eventQueue, 0, WaitForNextFrame.alloc());
            EventQueue.insertEventAt(p_eventQueue, 1, StartTurnEvent.alloc(TurnTimeline));
        }
    }

}
