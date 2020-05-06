using _ActionPoint;
using _Attack;
using _Entity;
using _NavigationGraph;
using static _AI._DecisionTree._Algorithm.Algorithm;

namespace _AI._DecisionTree
{
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
            if (NavigationGraphAlgorithm.areNavigationNodesNeighbors(NavigationGraphContainer.UniqueNavigationGraph, SourceEntity.CurrentNavigationNode, TargetEntity.CurrentNavigationNode, NavigationGraphFlag.SNAPSHOT))
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

