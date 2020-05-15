using _ActionPoint;
using _AI._DecisionTree._Algorithm;
using _Attack;
using _Entity;
using System.Collections.Generic;

namespace _AI._DecisionTree
{
    /// <summary>
    /// The intension to attack the TargetEntity by using the <see cref="Attack"/>.
    /// </summary>
    public class AttackNode : ADecisionNode, IAtionPointPredictable
    {
        public Entity SourceEntity;
        public Entity TargetEntity;
        public Attack Attack;
        public int NumberOfAttacks;
        public float DamageDone;

        public static AttackNode alloc(Entity p_sourceEntity, Entity p_targetEntity, Attack p_attack)
        {
            AttackNode l_instance = new AttackNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetEntity = p_targetEntity;
            l_instance.Attack = p_attack;
            l_instance.NumberOfAttacks = 0;
            l_instance.DamageDone = 0.0f;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode)
        {
            //TODO -> Store executed attacks in a vector, then the consumer read the vector and transforms them in EntityActions.
            ActionPoint l_sourceEntityActionPoint = EntityComponent.get_component<ActionPoint>(SourceEntity);
            ActionPointData l_virtualActionPointData = new ActionPointData()
            {
                InitialActionPoints = l_sourceEntityActionPoint.ActionPointData.InitialActionPoints,
                CurrentActionPoints = l_sourceEntityActionPoint.ActionPointData.InitialActionPoints
            };
            while (l_virtualActionPointData.CurrentActionPoints >= Attack.AttackData.APCost)
            {
                ActionPointData.add(ref l_virtualActionPointData, -1 * Attack.AttackData.APCost);
                DamageDone += Attack.AttackData.Damage;
                NumberOfAttacks += 1;
            }
        }

        public void AddActionPointPrediction(List<float> p_actionPointPredictions)
        {
            if (NumberOfAttacks > 0)
            {
                for (int i = 0; i < NumberOfAttacks; i++)
                {
                    p_actionPointPredictions.Add(Attack.AttackData.APCost);
                }
            }
        }
    };
}

