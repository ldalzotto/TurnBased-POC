
using _Attack;
using _Entity;
using _EntityCharacteristics;
using _Navigation;
using System.Collections.Generic;

namespace _AI._DecisionTree._Builder
{
    public static class TreeBuilder
    {
        public static void buildAggressiveTree(DecisionTree p_decisionTree, Entity p_sourceEntity)
        {
            for (int i = 0; i < EntityContainer.Entities.Count; i++)
            {
                Entity l_entity = EntityContainer.Entities[i];
                if (l_entity != p_sourceEntity)
                {
                    /* 
                        For now, only Entities with characteristics are considered elligible to move to.
                        //TODO, having a more complex targetting system.
                    */
                    if (EntityComponent.get_component<EntityCharacteristics>(l_entity) != null)
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
                    p_moveToEntityNode.TargetEntity.CurrentNavigationNode))
            {
                MoveToNavigationNodeNode l_moveToNavigationNodeNode = MoveToNavigationNodeNode.alloc(p_moveToEntityNode.SourceEntity.CurrentNavigationNode, p_moveToEntityNode.SourceEntity.CurrentNavigationNode);
                DecisionTree.linkDecisionNodes(p_aiDecisionTree, p_moveToEntityNode, l_moveToNavigationNodeNode);
                yield return l_moveToNavigationNodeNode;
            }

        }

    }
}