using _Entity;
using _Entity._Events;
using _EventQueue;
using _Health;
using _HealthRecovery;
using _NavigationGraph;

namespace _NavigationEngine
{
    public static class HealthRecoveryStep
    {
        public static void resolveHealthRecovery(NavigationEngine p_navigationEngine, Entity p_entity,
                NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode, EventQueue p_eventQueue)
        {
            if(p_newNavigationNode!= null)
            {
                if (EntityComponent.get_component<Health>(p_entity) != null)
                {
                    HealthRecovery l_nextNavigationNodeHelathRecovery = EntityQuery.get_firstComponentOfType<HealthRecovery>(ref p_navigationEngine.EntitiesIndexedByNavigationNodes, p_newNavigationNode);
                    if (l_nextNavigationNodeHelathRecovery != null)
                    {
                        EntityEventsComposition.pushEntityDestroyedEvents(p_eventQueue, l_nextNavigationNodeHelathRecovery.AssociatedEntity);
                        EventQueue.insertEventAt(p_eventQueue, 0, HealthRecoveryEvent.alloc(p_entity, l_nextNavigationNodeHelathRecovery));
                    }
                }
            }
           
        }
    }
}

