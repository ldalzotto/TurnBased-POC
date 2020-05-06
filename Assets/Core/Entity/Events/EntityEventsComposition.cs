using _EventQueue;

namespace _Entity._Events
{
    public static class EntityEventsComposition
    {
        /// <summary>
        /// When an <see cref="Entity"/> is destroyed, the following <see cref="AEvent"/> occurs in this order :
        ///     - <see cref="EntityCurrentNavigationNodeChange"/> with a <see cref="EntityCurrentNavigationNodeChange.NavigationNode"/> set to null to 
        ///      trigger cleanup to <see cref="_NavigationEngine.NavigationEngine"/>.
        ///     - <see cref="EntityDestroyEvent"/> to effectively destroy the <see cref="Entity"/>.
        /// </summary>
        public static void pushEntityDestroyedEvents(EventQueue p_eventQueue, Entity p_entity)
        {
            // Destruction events are inserted only if there isn't already the same destroy events in the queue.
            bool l_allowedToDestruct = true;
            var l_alreadyInsertedEntityDestroyEvents = EventQueue.get_allEvents<EntityDestroyEvent>(p_eventQueue);
            while (l_alreadyInsertedEntityDestroyEvents.MoveNext())
            {
                if (l_alreadyInsertedEntityDestroyEvents.Current.EntityToDestroy == p_entity)
                {
                    l_allowedToDestruct = false;
                    break;
                }
            }

            if (l_allowedToDestruct)
            {
                EntityCurrentNavigationNodeChange l_navigationNodeChangeEvent = EntityCurrentNavigationNodeChange.alloc(p_entity, null);
                EventQueue.insertEventAt(p_eventQueue, 0, l_navigationNodeChangeEvent);
                EventQueue.insertAfter(p_eventQueue, l_navigationNodeChangeEvent, EntityDestroyEvent.alloc(p_entity));
            }
        }
    }
}
