
using _Attack;
using _Entity;
using _EntityCharacteristics;
using _Health;
using _HealthRecovery;
using _NavigationGraph;
using System.Collections.Generic;

namespace _AI._DecisionTree._Builder
{
    public static class TreeBuilder
    {
        public static void buildAggressiveTree(DecisionTree p_decisionTree, Entity p_sourceEntity)
        {
            Health p_sourceEntityHealth = EntityComponent.get_component<Health>(p_sourceEntity);

            if (Health.getHealthRatio(p_sourceEntityHealth) <= 0.5f)
            {
                if (EntityComponentContainer.Components.ContainsKey(typeof(HealthRecoveryTrigger)))
                {
                    buildMoveToHealthTrigger(p_decisionTree, p_sourceEntity);
                }
                else
                {
                    buildMoveToRangeOfEntity(p_decisionTree, p_sourceEntity);
                }
            }
            else
            {
                buildMoveToRangeOfEntity(p_decisionTree, p_sourceEntity);
            }
        }

        private static void buildMoveToRangeOfEntity(DecisionTree p_decisionTree, Entity p_sourceEntity)
        {
            /* 
               For now, only Entities with characteristics are considered elligible to move to.
               //TODO, having a more complex targetting system.
            */
            if (EntityComponentContainer.Components.ContainsKey(typeof(EntityCharacteristics)))
            {
                var l_entitiesToGo = EntityComponentContainer.Components[typeof(EntityCharacteristics)];
                for (int i = 0; i < l_entitiesToGo.Count; i++)
                {
                    Entity l_entity = l_entitiesToGo[i].AssociatedEntity;
                    if (l_entity != p_sourceEntity)
                    {
                        foreach (ADecisionNode l_moveToNavigationNode in createMoveToEntityTree(p_decisionTree, p_decisionTree.RootNode, p_sourceEntity, l_entity))
                        {
                            DecisionTree.linkDecisionNodes(
                                    p_decisionTree,
                                    l_moveToNavigationNode,
                                    AttackNode.alloc(p_sourceEntity, l_entity, EntityComponent.get_component<Attack>(p_sourceEntity)));
                        }
                    }

                }
            }
        }

        private static void buildMoveToHealthTrigger(DecisionTree p_decisionTree, Entity p_sourceEntity)
        {
            if (EntityComponentContainer.Components.ContainsKey(typeof(HealthRecoveryTrigger)))
            {
                var l_healthRecoveryTriggers = EntityComponentContainer.Components[typeof(HealthRecoveryTrigger)];
                for (int i = 0; i < l_healthRecoveryTriggers.Count; i++)
                {
                    HealthRecoveryTrigger l_healthRecoveryTrigger = l_healthRecoveryTriggers[i] as HealthRecoveryTrigger;
                    MoveToNavigationNodeNode l_moveToNavigationNodeNode = MoveToNavigationNodeNode.alloc(p_sourceEntity.CurrentNavigationNode,
                        l_healthRecoveryTrigger.AssociatedEntity.CurrentNavigationNode);
                    DecisionTree.linkDecisionNodes(p_decisionTree, p_decisionTree.RootNode, l_moveToNavigationNodeNode);

                    HealNode l_healNode = HealNode.alloc(p_sourceEntity, l_healthRecoveryTrigger);
                    DecisionTree.linkDecisionNodes(p_decisionTree, l_moveToNavigationNodeNode, l_healNode);
                }
            }
        }

        private static IEnumerable<ADecisionNode> createMoveToEntityTree(DecisionTree p_aiDecisionTree, ADecisionNode p_rootNode, Entity p_sourceEntity, Entity p_targetEntity)
        {
            MoveToEntityNode l_moveToEntityNode = MoveToEntityNode.alloc(p_sourceEntity, p_targetEntity);
            DecisionTree.linkDecisionNodes(p_aiDecisionTree, p_rootNode, l_moveToEntityNode);
            return createMoveToNavigationNodesLink(p_aiDecisionTree, l_moveToEntityNode);
        }

        /// <summary>
        /// Calculates all possibilities to move next to the Entity provided by <paramref name="p_moveToEntityNode"/>.
        /// </summary>
        private static IEnumerable<ADecisionNode> createMoveToNavigationNodesLink(DecisionTree p_aiDecisionTree, MoveToEntityNode p_moveToEntityNode)
        {
            var l_reachableNavigationNodes =
                        NavigationGraphAlgorithm.getReachableNeighborNavigationNodes(
                                NavigationGraphContainer.UniqueNavigationGraph,
                                p_moveToEntityNode.TargetEntity.CurrentNavigationNode,
                                NavigationGraphFlag.CURRENT).GetEnumerator();

            while (l_reachableNavigationNodes.MoveNext())
            {
                MoveToNavigationNodeNode l_moveToNavigationNodeNode = MoveToNavigationNodeNode.alloc(p_moveToEntityNode.SourceEntity.CurrentNavigationNode, l_reachableNavigationNodes.Current);
                DecisionTree.linkDecisionNodes(p_aiDecisionTree, p_moveToEntityNode, l_moveToNavigationNodeNode);
                yield return l_moveToNavigationNodeNode;
            }


            /* If the source entity is already a neighbor of the target entity, we still create a node to move to same position to simulate the fact of staying at the same position. */
            if (NavigationGraphAlgorithm.areNavigationNodesNeighbors(
                    NavigationGraphContainer.UniqueNavigationGraph,
                    p_moveToEntityNode.SourceEntity.CurrentNavigationNode,
                    p_moveToEntityNode.TargetEntity.CurrentNavigationNode,
                    NavigationGraphFlag.SNAPSHOT))
            {
                MoveToNavigationNodeNode l_moveToNavigationNodeNode = MoveToNavigationNodeNode.alloc(p_moveToEntityNode.SourceEntity.CurrentNavigationNode, p_moveToEntityNode.SourceEntity.CurrentNavigationNode);
                DecisionTree.linkDecisionNodes(p_aiDecisionTree, p_moveToEntityNode, l_moveToNavigationNodeNode);
                yield return l_moveToNavigationNodeNode;
            }

        }

    }
}