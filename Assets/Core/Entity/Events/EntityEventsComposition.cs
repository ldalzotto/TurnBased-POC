using _EventQueue;
using System.Collections.Generic;

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
        public static void addEntityDestroyedEvents(List<AEvent> p_events, Entity p_entity, bool front = false)
        {
            if (front)
            {
                p_events.Insert(0, EntityDestroyEvent.alloc(p_entity));
                p_events.Insert(0, EntityCurrentNavigationNodeChange.alloc(p_entity, null));
            }
            else
            {
                p_events.Add(EntityCurrentNavigationNodeChange.alloc(p_entity, null));
                p_events.Add(EntityDestroyEvent.alloc(p_entity));
            }

        }
    }
}
