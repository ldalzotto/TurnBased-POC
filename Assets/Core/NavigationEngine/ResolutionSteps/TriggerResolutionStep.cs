﻿using _Entity;
using _NavigationGraph;
using System;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public static class TriggerResolutionStep
    {
        /// <summary>
        /// Calls trigger event <see cref="INavigationTriggerComponent.OnTriggerEnter(Entity, List{_EventQueue.AEvent})"/> if the <paramref name="p_movingEntity"/> 
        /// moves to a <see cref="NavigationNode"/> that contains any <see cref="INavigationTriggerComponent"/>.
        /// <see cref="INavigationTriggerComponent.OnTriggerEnter"/> event call are ordered by order defined by <see cref="TriggerResolutionOrder.TriggerComponentResolutionOrder"/>.
        /// </summary>
        public static void resolveTrigger(NavigationEngine p_navigationEngine, Entity p_movingEntity,
               NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            if (p_newNavigationNode != null)
            {
                if (p_navigationEngine.EntitiesIndexedByNavigationNodes.Entities.ContainsKey(p_newNavigationNode))
                {
                    List<Entity> l_entitiesOnTheNewNavigationNode = p_navigationEngine.EntitiesIndexedByNavigationNodes.Entities[p_newNavigationNode];
                    List<INavigationTriggerComponent> l_currentEntityTriggerComponents = new List<INavigationTriggerComponent>();

                    for (int i = 0; i < l_entitiesOnTheNewNavigationNode.Count; i++)
                    {
                        Entity l_entity = l_entitiesOnTheNewNavigationNode[i];
                        if (l_entity != p_movingEntity)
                        {

                            TriggerResolutionOrder.extractTriggerComponentSortedByExecutionOrder(l_currentEntityTriggerComponents, l_entity);

                            for (int j = 0; j < l_currentEntityTriggerComponents.Count; j++)
                            {
                                INavigationTriggerComponent l_resolvedComponent = l_currentEntityTriggerComponents[j];
                                l_resolvedComponent.OnTriggerEnter(p_movingEntity, p_navigationEngine.CachedProducedEventsStackByTriggers);
                            }
                        }
                    }
                }
            }

        }


    }

    public static class TriggerResolutionOrder
    {
        /// <summary>
        /// Data structure defining the order of execution of <see cref="INavigationTriggerComponent.OnTriggerEnter"/>.
        /// A value of 0 will be called before a value of 1.
        /// </summary>
        public static Dictionary<Type, float> TriggerComponentResolutionOrder = new Dictionary<Type, float>();

        /// <summary>
        /// Extract all <see cref="INavigationTriggerComponent"/> components from <paramref name="p_requestedEntity"/> and sort them by querying the
        ///     <see cref="TriggerResolutionOrder.TriggerComponentResolutionOrder"/>.
        /// </summary>
        public static void extractTriggerComponentSortedByExecutionOrder(List<INavigationTriggerComponent> p_extractedComponents, Entity p_requestedEntity)
        {
            p_extractedComponents.Clear();

            var l_components = p_requestedEntity.Components.GetRefEnumerator();
            while (l_components.MoveNext())
            {
                if (l_components.GetCurrentRef().value is INavigationTriggerComponent)
                {
                    p_extractedComponents.Add(l_components.GetCurrentRef().value as INavigationTriggerComponent);
                }
            }

            p_extractedComponents.Sort(ProducedEventOrdererFunction.build(TriggerComponentResolutionOrder));
        }

        struct ProducedEventOrdererFunction : IComparer<INavigationTriggerComponent>
        {
            public Dictionary<Type, float> TriggerComponentPriorityLookup;

            public static ProducedEventOrdererFunction build(Dictionary<Type, float> p_triggerComponentPriorityLookup)
            {
                ProducedEventOrdererFunction l_instance = new ProducedEventOrdererFunction();
                l_instance.TriggerComponentPriorityLookup = p_triggerComponentPriorityLookup;
                return l_instance;
            }

            public int Compare(INavigationTriggerComponent x, INavigationTriggerComponent y)
            {
                return TriggerComponentPriorityLookup[x.GetType()].CompareTo(TriggerComponentPriorityLookup[y.GetType()]);
            }
        }
    }
}

