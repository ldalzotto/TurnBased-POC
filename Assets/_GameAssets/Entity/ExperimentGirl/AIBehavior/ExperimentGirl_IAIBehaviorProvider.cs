using _AI._Behavior;
using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _AI._DecisionTree._Builder;
using _Entity;
using _Health;
using _HealthRecovery;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _GameAssets._Entity._ExperimentGirl
{
    public class ExperimentGirl_IAIBehaviorProvider : SerializedScriptableObject, IAIBehaviorProvider
    {
        [Range(0.0f, 1.0f)]
        public float HealthPercentageForSearchingRecorevory;

        public void buildDecisionTree(DecisionTree p_decisionTree, Entity p_sourceEntity)
        {
            Health p_sourceEntityHealth = EntityComponent.get_component<Health>(p_sourceEntity);

            if (Health.getHealthRatio(p_sourceEntityHealth) <= HealthPercentageForSearchingRecorevory)
            {
                if (EntityComponentContainer.Components.ContainsKey(typeof(HealthRecoveryTrigger)))
                {
                    TreeBuilderLibrary.buildMoveToHealthTrigger(p_decisionTree, p_sourceEntity);
                }
                else
                {
                    TreeBuilderLibrary.buildMoveToRangeOfEntity(p_decisionTree, p_sourceEntity);
                }
            }
            else
            {
                TreeBuilderLibrary.buildMoveToRangeOfEntity(p_decisionTree, p_sourceEntity);
            }
        }

        public ChoicePicking.PickChoiceDelegate get_choicePicking()
        {
            return ChoicePicking.defaultTestPickChoice;
        }
    }

}

