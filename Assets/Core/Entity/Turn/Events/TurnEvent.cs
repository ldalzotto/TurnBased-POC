﻿using _ActionPoint;
using _EventQueue;
using _EventQueue._Events;
using _Functional;
using _TurnTimeline;

namespace _Entity._Turn
{
    public class StartTurnEvent : AEvent
    {
        public TurnTimeline TurnTimeline;

        /// <summary>
        /// The <see cref="EventQueue"/> where all <see cref="AEvent"/> that occurs during the turn will be queued while the current queue
        /// is waiting for <see cref="EntityActionQueue"/> completion (no more <see cref="AEvent"/>).
        /// </summary>
        public EventQueue EntityActionQueue;

        public EndTurnEvent FutureEndTurnEvent;

        public static StartTurnEvent alloc(TurnTimeline p_turnTimeline, EventQueue p_entityTurnActionQueue)
        {
            StartTurnEvent l_instance = new StartTurnEvent();
            l_instance.TurnTimeline = p_turnTimeline;
            l_instance.EntityActionQueue = p_entityTurnActionQueue;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            Entity l_nextTurnEntity = TurnTimelineAlgorithm.IncrementTimeline(TurnTimeline);

            // ExternalHooks.LogDebug(l_nextTurnEntity.GetHashCode().ToString());

            if (l_nextTurnEntity != null)
            {
                ActionPoint.resetActionPoints(EntityComponent.get_component<ActionPoint>(l_nextTurnEntity));
                MyEvent<Entity>.broadcast(ref l_nextTurnEntity.OnEntityTurnStart, ref l_nextTurnEntity);

                EventQueue.enqueueEvent(p_eventQueue, IterateAndWaitForEmptyQueue.alloc(EntityActionQueue));
                FutureEndTurnEvent = EndTurnEvent.alloc(TurnTimeline, EntityActionQueue, l_nextTurnEntity);
                EventQueue.enqueueEvent(p_eventQueue, FutureEndTurnEvent);

                EventQueue.clearAll(EntityActionQueue);
                EventQueue.enqueueEvent(EntityActionQueue, StartEntityTurnEvent.alloc(l_nextTurnEntity));
            }
        }
    }

    public class EndTurnEvent : AEvent
    {
        public TurnTimeline TurnTimeline;
        public EventQueue EntityActionQueue;
        public Entity Entity;
        public static EndTurnEvent alloc(TurnTimeline p_turnTimeline, EventQueue p_entityTurnActionQueue, Entity p_entity)
        {
            EndTurnEvent l_instance = new EndTurnEvent();
            l_instance.TurnTimeline = p_turnTimeline;
            l_instance.EntityActionQueue = p_entityTurnActionQueue;
            l_instance.Entity = p_entity;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);

            MyEvent<Entity>.broadcast(ref Entity.OnEntityTurnEnd, ref Entity);
            EventQueue.enqueueEvent(p_eventQueue, WaitForNextFrame.alloc());
            EventQueue.enqueueEvent(p_eventQueue, StartTurnEvent.alloc(TurnTimeline, EntityActionQueue));
        }
    }

}
