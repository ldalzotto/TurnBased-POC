using _Functional;
using System.Collections.Generic;

namespace _Entity._Turn
{
    public static class TurnGlobalEvents
    {
        public static RefDictionary<Entity, MyEvent> OnEntityTurnStartEvent;
        public static RefDictionary<Entity, MyEvent> OnEntityTurnEndEvent;

        static TurnGlobalEvents()
        {
            OnEntityTurnStartEvent = new RefDictionary<Entity, MyEvent>();
            OnEntityTurnEndEvent = new RefDictionary<Entity, MyEvent>();
        }

        public static void onEntityTurnStart(Entity p_entity)
        {
            if (OnEntityTurnStartEvent.ContainsKey(p_entity))
            {
                MyEvent.broadcast(ref OnEntityTurnStartEvent.ValueRef(p_entity));
            }
        }

        public static void onEntityTurnEnd(Entity p_entity)
        {
            if (OnEntityTurnEndEvent.ContainsKey(p_entity))
            {
                MyEvent.broadcast(ref OnEntityTurnEndEvent.ValueRef(p_entity));
            }
        }
    }
}
