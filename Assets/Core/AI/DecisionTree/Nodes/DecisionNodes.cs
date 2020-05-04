using System.Collections;
using _Entity;
using _Attack;
using _ActionPoint;
using static _AI._DecisionTree._Algorithm.Algorithm;
using _Navigation;
using System;
using System.Collections.Generic;

namespace _AI._DecisionTree
{

    /// <summary>
    /// Usually linked to MoveToNavigationNodeNode for every reachable neighbors(m_neighborNavigationNodes) of the <see cref="TargetEntity"/>.
    /// It describes the intend to get in range of another <see cref="Entity"/>.
    /// </summary>
    public class MoveToEntityNode : ADecisionNode
    {
        public Entity SourceEntity;
        public Entity TargetEntity;

        public static MoveToEntityNode alloc(Entity p_sourceEntity, Entity p_targetEntity)
        {
            MoveToEntityNode l_instance = new MoveToEntityNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetEntity = p_targetEntity;
            return l_instance;
        }
    };

    /// <summary>
    /// The intention to move to the TargetNavigationNode.
    /// Updates the <see cref="AIDecisionScore.PathScore"/>.
    /// Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public class MoveToNavigationNodeNode : ADecisionNode
    {
        public NavigationNode SourceNavigationNode;
        public NavigationNode TargetNavigationNode;

        /// <summary>
        /// The list of all <see cref="NavigationNode"/> that will be followed by the calling <see cref="Entity"/>.
        /// </summary>
        public List<NavigationNode> CalculatedPath;

        public static MoveToNavigationNodeNode alloc(NavigationNode p_sourceNavigationNode, NavigationNode p_targetNavigationnode)
        {
            MoveToNavigationNodeNode l_instance = new MoveToNavigationNodeNode();
            l_instance.SourceNavigationNode = p_sourceNavigationNode;
            l_instance.TargetNavigationNode = p_targetNavigationnode;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            using (NavigationGraphAlgorithm.CalculatePathRequest l_calculationRequest = NavigationGraphAlgorithm.CalculatePathRequest.CalculatePathRequestPool.popOrCreate())
            {
                NavigationGraphAlgorithm.CalculatePathRequest.prepareForCalculation(
                                l_calculationRequest,
                                NavigationGraphContainer.UniqueNavigationGraph,
                                SourceNavigationNode,
                                TargetNavigationNode,
                                PathCalculationParameters.build(2.0f));

                NavigationGraphAlgorithm.CalculatePath(l_calculationRequest);

                ref NavigationPath l_calculatedNavigationPath = ref l_calculationRequest.ResultPath;

                /* The PathScore is kepts as if, because we want that the AIDecisionTree algorithm compare all possible path until the destination is reached to
                effectively retrieve the shortest one. */
                p_entityDecisionContextdata.AIDecisionScore.PathScore += l_calculatedNavigationPath.PathCost;

                NavigationPath.limitPathByMaximumPathCost(ref l_calculatedNavigationPath, p_entityDecisionContextdata.ActionPoint.CurrentActionPoints);

                if (l_calculatedNavigationPath.NavigationNodes.Count > 1)
                {
                    DecisionNodeConsumerAction = EDecisionNodeConsumerAction.EXECUTE;
                    CalculatedPath = new List<NavigationNode>(l_calculatedNavigationPath.NavigationNodes);
                }

                /* However, the virtual ActionPoints is updated as if the Entity has traversed the calculated path until it's virtual action points is depleted. */
                ActionPointData.add(ref p_entityDecisionContextdata.ActionPoint, -1 * l_calculatedNavigationPath.PathCost);
            }
        }
    };

    /// <summary>
    /// The intension to attack the TargetEntity by using the <see cref="Attack"/>.
    /// Updates the <see cref="AIDecisionScore.DamageScore"/>.
    /// Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public class AttackNode : ADecisionNode
    {
        public Entity SourceEntity;
        public Entity TargetEntity;
        public Attack Attack;
        public int NumberOfAttacks;

        public static AttackNode alloc(Entity p_sourceEntity, Entity p_targetEntity, Attack p_attack)
        {
            AttackNode l_instance = new AttackNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetEntity = p_targetEntity;
            l_instance.Attack = p_attack;
            l_instance.NumberOfAttacks = 0;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            if (NavigationGraphAlgorithm.areNavigationNodesNeighbors(NavigationGraphContainer.UniqueNavigationGraph, SourceEntity.CurrentNavigationNode, TargetEntity.CurrentNavigationNode))
            {
                if (p_entityDecisionContextdata.ActionPoint.CurrentActionPoints >= Attack.AttackData.APCost)
                {
                    DecisionNodeConsumerAction = EDecisionNodeConsumerAction.EXECUTE;

                    //TODO -> Store executed attacks in a vector, then the consumer read the vector and transforms them in EntityActions.
                    while (p_entityDecisionContextdata.ActionPoint.CurrentActionPoints >= Attack.AttackData.APCost)
                    {
                        ActionPointData.add(ref p_entityDecisionContextdata.ActionPoint, -1 * Attack.AttackData.APCost);
                        p_entityDecisionContextdata.AIDecisionScore.DamageScore += Attack.AttackData.Damage;
                        NumberOfAttacks += 1;
                    }
                }
            }
        }
    };
}

