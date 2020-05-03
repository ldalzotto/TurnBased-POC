using _ActionPoint;
using _Entity._Events;
using _EventQueue;
using System.Collections;

namespace _Entity._Turn
{
    public class StartEntityTurnEvent : AEvent
    {
        public Entity Entity;

        public static StartEntityTurnEvent alloc(Entity p_entity)
        {
            StartEntityTurnEvent l_instance = new StartEntityTurnEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            ActionPoint.resetActionPoints(EntityComponent.get_component<ActionPoint>(Entity));

            // We insert the EndTurn event just to be sure that end will effectively occur.
            EventQueue.insertEventAt(p_eventQueue, 0, EndEntityTurnEvent.alloc(Entity));

            EventQueue.insertEventAt(p_eventQueue, 0, HealthReductionEvent.alloc(Entity));
        }
    }

    public class EndEntityTurnEvent : AEvent
    {
        public Entity Entity;
        public static EndEntityTurnEvent alloc(Entity p_entity)
        {
            EndEntityTurnEvent l_instance = new EndEntityTurnEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }
    }
}

