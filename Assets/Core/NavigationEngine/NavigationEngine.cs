﻿using _Entity;
using _EventQueue;
using _NavigationGraph;
using System;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public static class NavigationEngineContainer
    {
        public static NavigationEngine UniqueNavigationEngine;
    }

    /// <summary>
    /// The <see cref="NavigationEngine"/> is responsible of logic executed when an <see cref="Entity"/> has it's <see cref="Entity.CurrentNavigationNode"/> modified.
    /// When this occurs, the <see cref="NavigationEngine"/> triggers event based on the presence of <see cref="INavigationTriggerComponent"/>.
    /// </summary>
    public class NavigationEngine
    {
        public EntitiesIndexedByNavigationNodes EntitiesIndexedByNavigationNodes;
        public List<AEvent> CachedProducedEventsStackByTriggers;

        public static NavigationEngine alloc()
        {
            NavigationEngine l_instance = new NavigationEngine();
            l_instance.EntitiesIndexedByNavigationNodes = EntitiesIndexedByNavigationNodes.build(l_instance);
            l_instance.CachedProducedEventsStackByTriggers = new List<AEvent>();

            NavigationEngineContainer.UniqueNavigationEngine = l_instance;
            return l_instance;
        }

        public static void free(NavigationEngine p_navigationEngine)
        {
            EntitiesIndexedByNavigationNodes.free(ref p_navigationEngine.EntitiesIndexedByNavigationNodes);
            if (NavigationEngineContainer.UniqueNavigationEngine == p_navigationEngine) { NavigationEngineContainer.UniqueNavigationEngine = null; };
        }

        /// <summary>
        /// Tick the <see cref="NavigationEngine"/> : 
        ///     - Updates the position of the <paramref name="p_entity"/> on the <see cref="NavigationNode"/> indexed containers.
        ///     (see <see cref="EntitiesIndexedByNavigationNodes.onNavigationNodeChange(ref EntitiesIndexedByNavigationNodes, Entity, NavigationNode, NavigationNode)"/>).
        ///     - If the <paramref name="p_entity"/> is defined as an obstacle <see cref="_Navigation._Modifier.NavigationModifier"/>, then obstacles representation
        ///     of the <see cref="NavigationGraph"/> is updated (see <see cref="ObstacleStep.resolveNavigationObstacleAlterations(NavigationEngine, Entity, NavigationNode, NavigationNode)"/>).
        ///     - Calls the trigger events if elligible (see <see cref="TriggerResolutionStep.resolveTrigger(NavigationEngine, Entity, NavigationNode, NavigationNode)"/>).
        /// </summary>
        /// <param name="p_oldNavigationNode"> Thr old <see cref="NavigationNode"/> can be null. Meaning that the <paramref name="p_entity"/> wasn't positioned inside the <see cref="NavigationGraph"/> before. </param>
        /// <param name="p_newNavigationNode"> The target <see cref="NavigationNode"/> can be null. Meaning that the <paramref name="p_entity"/> have been destroyed. </param>
        public static List<AEvent> resolveEntityNavigationNodeChange(NavigationEngine p_navigationEngine,
                                    Entity p_entity, NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            p_navigationEngine.CachedProducedEventsStackByTriggers.Clear();
            EntitiesIndexedByNavigationNodes.onNavigationNodeChange(ref p_navigationEngine.EntitiesIndexedByNavigationNodes, p_entity, p_oldNavigationNode, p_newNavigationNode);
            ObstacleStep.resolveNavigationObstacleAlterations(p_navigationEngine, p_entity, p_oldNavigationNode, p_newNavigationNode);
            TriggerResolutionStep.resolveTrigger(p_navigationEngine, p_entity, p_oldNavigationNode, p_newNavigationNode);
            return p_navigationEngine.CachedProducedEventsStackByTriggers;
        }

    }

    public struct EntitiesIndexedByNavigationNodes
    {
        public Dictionary<NavigationNode, List<Entity>> Entities;

        public static EntitiesIndexedByNavigationNodes build(NavigationEngine p_navigationEngine)
        {
            EntitiesIndexedByNavigationNodes l_instance = new EntitiesIndexedByNavigationNodes();
            l_instance.Entities = new Dictionary<NavigationNode, List<Entity>>();
            return l_instance;
        }

        public static void free(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes)
        {
            p_entitiesIndexedByNavigationNodes.Entities.Clear();
        }

        public static void onNavigationNodeChange(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes,
                                Entity p_entity, NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            if (p_entity != null)
            {
                if (p_newNavigationNode != null) { addEntityToNavigationNode(ref p_entitiesIndexedByNavigationNodes, p_entity, p_newNavigationNode); }
                if (p_oldNavigationNode != null) { removeEntityToNavigationNode(ref p_entitiesIndexedByNavigationNodes, p_entity, p_oldNavigationNode); }
            }
        }

        private static void addEntityToNavigationNode(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes, Entity p_entity, NavigationNode p_navigationNode)
        {
            if (!p_entitiesIndexedByNavigationNodes.Entities.ContainsKey(p_navigationNode))
            {
                p_entitiesIndexedByNavigationNodes.Entities.Add(p_navigationNode, new List<Entity>());
            }

            List<Entity> l_navigationNodeEntities = p_entitiesIndexedByNavigationNodes.Entities[p_navigationNode];
            if (!l_navigationNodeEntities.Contains(p_entity))
            {
                l_navigationNodeEntities.Add(p_entity);
            }
        }

        private static void removeEntityToNavigationNode(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes, Entity p_entity, NavigationNode p_navigationNode)
        {
            if (p_entitiesIndexedByNavigationNodes.Entities.ContainsKey(p_navigationNode))
            {
                p_entitiesIndexedByNavigationNodes.Entities[p_navigationNode].Remove(p_entity);
            }
        }
    }

    /// <summary>
    /// Query the <see cref="EntitiesIndexedByNavigationNodes.Entities"/>.
    /// </summary>
    public static class EntityQuery
    {

        /// <summary>
        /// Returns true if <paramref name="p_entitiesIndexedByNavigationNodes"/> has any <see cref="Entity"/> that has a component of type COMPONENT
        /// and is satisfying the <paramref name="p_filterCondition"/> condition. 
        /// </summary>
        public static bool isThereAtLeastOfComponentOfType<COMPONENT>(
                        ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes,
                        NavigationNode p_requestedNode,
                        Func<COMPONENT, bool> p_filterCondition = null)
                where COMPONENT : AEntityComponent
        {
            if (p_entitiesIndexedByNavigationNodes.Entities.ContainsKey(p_requestedNode))
            {
                List<Entity> l_entities = p_entitiesIndexedByNavigationNodes.Entities[p_requestedNode];
                for (int i = 0; i < l_entities.Count; i++)
                {
                    COMPONENT l_component = EntityComponent.get_component<COMPONENT>(l_entities[i]);
                    if (l_component != null)
                    {
                        if (p_filterCondition != null)
                        {
                            if (p_filterCondition.Invoke(l_component))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }
    }
}
