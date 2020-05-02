using System.Collections;
using _Entity;
using _Attack;
using _ActionPoint;
using static _AI._DecisionTree._Algorithm.Algorithm;
using _Navigation;
using System;

namespace _AI._DecisionTree
{

    /// <summary>
    /// Usually linked to MoveToNavigationNodeNode for every reachable neighbors(m_neighborNavigationNodes) of the TargetEntity.
    /// It describes the intend to get in range of another Entity.
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
    /// Updates the AIDecisionScore::PathScore.
    /// Consumes ActionPoint.
    /// </summary>
    public class MoveToNavigationNodeNode : ADecisionNode
    {
        public NavigationNode SourceNavigationNode;
        public NavigationNode TargetNavigationNode;

        public override void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            using (NavigationGraphAlgorithm.CalculatePathRequest l_calculationRequest = NavigationGraphAlgorithm.CalculatePathRequest.CalculatePathRequestPool.popOrCreate())
            {
                l_calculationRequest.PathCalculationParameters = PathCalculationParameters.build(2.0f);
                l_calculationRequest.BeginNode = SourceNavigationNode;
                l_calculationRequest.EndNode = TargetNavigationNode;

                NavigationGraphAlgorithm.CalculatePath(l_calculationRequest);
                //TODO
            }

        }
    };

    /// <summary>
    /// The intension to attack the TargetEntity by using the MAttack.
    /// Updates the AIDecisionScore::DamageScore.
    /// Consumes ActionPoint.
    /// </summary>
    public class AttackNode : ADecisionNode
    {
        public Entity SourceEntity;
        public Entity TargetEntity;
        public Attack Attack;

        public static AttackNode alloc(Entity p_sourceEntity, Entity p_targetEntity, Attack p_attack)
        {
            AttackNode l_instance = new AttackNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetEntity = p_targetEntity;
            l_instance.Attack = p_attack;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            if (p_entityDecisionContextdata.ActionPoint.CurrentActionPoints >= Attack.AttackData.APCost)
            {
                DecisionNodeConsumerAction = EDecisionNodeConsumerAction.EXECUTE;

                //TODO -> Store executed attacks in a vector, then the consumer read the vector and transforms them in EntityActions.
                while (p_entityDecisionContextdata.ActionPoint.CurrentActionPoints >= Attack.AttackData.APCost)
                {
                    ActionPointData.add(ref p_entityDecisionContextdata.ActionPoint, -1 * Attack.AttackData.APCost);
                    p_entityDecisionContextdata.AIDecisionScore.DamageScore += Attack.AttackData.Damage;
                }
            }
        }
    };
}

