using _AI._DecisionTree._Builder;
using _Entity;
using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Traversal;

namespace _AI._DecisionTree._Algorithm
{
    public static class TreeIteration
    {
        public static TreeIterationResult iterate(Entity p_entity,
                    ChoicePicking.PickChoiceDelegate p_choicePickingFunc)
        {
            DecisionTree l_decisionTree = DecisionTree.alloc();
            TreeBuilder.buildAggressiveTree(l_decisionTree, p_entity);
            RefList<AIDecisionTreeChoice> l_choices = Traversal.traverseDecisionTree(l_decisionTree);
            ref AIDecisionTreeChoice l_choice = ref p_choicePickingFunc.Invoke(l_choices, p_entity);
            List<float> l_actionPointConsumptionPrediction = ActionPointPrediction.predictActionPointConsumptions(ref l_choice);
            return TreeIterationResult.build(ref l_choice, l_actionPointConsumptionPrediction, l_choices);
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

