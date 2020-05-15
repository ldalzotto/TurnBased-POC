using _AI._Behavior;
using _AI._DecisionTree._Builder;
using _Entity;
using System;
using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Traversal;

namespace _AI._DecisionTree._Algorithm
{
    public static class TreeIteration
    {
        public static Action<TreeIterationResult> OnDecisionTreeIterated;

        public delegate void BuildDecisionTreeDelegate(DecisionTree p_decisionTree, Entity p_sourceEntity);

        public static TreeIterationResult iterate(AIBehavior p_aiBehavior)
        {
            IAIBehaviorProvider l_aiBehaviorProvider = p_aiBehavior.IAIBehaviorProvider;
            
            DecisionTree l_decisionTree = DecisionTree.alloc();
            l_aiBehaviorProvider.buildDecisionTree(l_decisionTree, p_aiBehavior.AssociatedEntity);
            RefList<AIDecisionTreeChoice> l_choices = Traversal.traverseDecisionTree(l_decisionTree);
            ref AIDecisionTreeChoice l_choice = ref l_aiBehaviorProvider.get_choicePicking().Invoke(l_choices, p_aiBehavior.AssociatedEntity);
            List<float> l_actionPointConsumptionPrediction = ActionPointPrediction.predictActionPointConsumptions(ref l_choice);
            TreeIterationResult l_result = TreeIterationResult.build(ref l_choice, l_actionPointConsumptionPrediction, l_choices);
            OnDecisionTreeIterated?.Invoke(l_result);
            return l_result;
        }

        public struct TreeIterationResult
        {
            public AIDecisionTreeChoice PickedChoice;
            public List<float> ActionPointConsumptionPrediction;
            public RefList<AIDecisionTreeChoice> PossibleChoices;

            public static TreeIterationResult build(ref AIDecisionTreeChoice pickedChoice, List<float> actionPointConsumptionPrediction, RefList<AIDecisionTreeChoice> possibleChoices)
            {
                TreeIterationResult l_instance = new TreeIterationResult();
                l_instance.PickedChoice = pickedChoice;
                l_instance.ActionPointConsumptionPrediction = actionPointConsumptionPrediction;
                l_instance.PossibleChoices = possibleChoices;
                return l_instance;
            }
        }
    }
}

