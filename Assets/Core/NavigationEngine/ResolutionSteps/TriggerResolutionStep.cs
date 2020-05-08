using _Entity;
using _NavigationGraph;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public static class TriggerResolutionStep
    {
        /// <summary>
        /// Calls trigger event <see cref="INavigationTriggerComponent.OnTriggerEnter(Entity, List{_EventQueue.AEvent})"/> if the <paramref name="p_movingEntity"/> 
        /// moves to a <see cref="NavigationNode"/> that contains any <see cref="INavigationTriggerComponent"/>.
        /// </summary>
        public static void resolveTrigger(NavigationEngine p_navigationEngine, Entity p_movingEntity,
               NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            if (p_newNavigationNode != null)
            {
                if (p_navigationEngine.EntitiesIndexedByNavigationNodes.Entities.ContainsKey(p_newNavigationNode))
                {
                    List<Entity> l_entitiesOnTheNewNavigationNode = p_navigationEngine.EntitiesIndexedByNavigationNodes.Entities[p_newNavigationNode];
                    for (int i = 0; i < l_entitiesOnTheNewNavigationNode.Count; i++)
                    {
                        Entity l_entity = l_entitiesOnTheNewNavigationNode[i];
                        if (l_entity != p_movingEntity)
                        {
                            var l_components = l_entity.Components.GetRefEnumerator();
                            while (l_components.MoveNext())
                            {
                                if (l_components.GetCurrentRef().value is INavigationTriggerComponent)
                                {
                                    ((INavigationTriggerComponent)l_components.GetCurrentRef().value).OnTriggerEnter(p_movingEntity, p_navigationEngine.CachedProducedEventsStackByTriggers);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}

