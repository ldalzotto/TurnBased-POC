using _Entity;
using _EventQueue;
using _NavigationGraph;
using System.Collections.Generic;

namespace _NavigationEngine._Events
{

    /// <summary>
    /// Updates the <see cref="Entity.CurrentNavigationNode"/> to <see cref="NavigationNode"/>.
    /// <see cref="NavigationEngine"/> is updated to trigger <see cref="INavigationTriggerComponent"/> events if elligible.
    /// /!\ All <see cref="Entity.CurrentNavigationNode"/> modification must be done via this event as he manages the update of the <see cref="NavigationEngine"/>.
    /// </summary>
    public class EntityCurrentNavigationNodeChange : AEvent
    {
        public Entity Entity;
        /// <summary>
        /// The target <see cref="NavigationNode"/> can be null. Meaning that the <see cref="Entity"/> is no more on the <see cref="NavigationGraph"/>.
        /// </summary>
        public NavigationNode NavigationNode;

        public static EntityCurrentNavigationNodeChange alloc(Entity p_entity, NavigationNode p_navigationNode)
        {
            EntityCurrentNavigationNodeChange l_instance = new EntityCurrentNavigationNodeChange();
            l_instance.Entity = p_entity;
            l_instance.NavigationNode = p_navigationNode;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            NavigationNode l_oldNavigationNode = Entity.CurrentNavigationNode;
            Entity.set_currentNavigationNode(Entity, NavigationNode);

            List<AEvent> l_triggeredEvents = NavigationEngine.resolveEntityNavigationNodeChange(NavigationEngineContainer.UniqueNavigationEngine, Entity, l_oldNavigationNode, NavigationNode);

            for (int i = l_triggeredEvents.Count - 1; i >= 0; i--)
            {
                EventQueue.insertEventAt(p_eventQueue, 0, l_triggeredEvents[i]);
            }
        }
    }
}
