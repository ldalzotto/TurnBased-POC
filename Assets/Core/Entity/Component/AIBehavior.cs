﻿using _AI._DecisionTree;
using _Entity;
using static _AI._DecisionTree._Algorithm.ChoicePicking;

namespace _AI._Behavior
{
    public class AIBehavior : AEntityComponent
    {
        public IAIBehaviorProvider IAIBehaviorProvider;

        public static AIBehavior alloc(IAIBehaviorProvider p_IAIBehaviorProvider)
        {
            AIBehavior l_instance = new AIBehavior();
            l_instance.IAIBehaviorProvider = p_IAIBehaviorProvider;
            return l_instance;
        }
    }

    public interface IAIBehaviorProvider
    {
        void buildDecisionTree(DecisionTree p_decisionTree, Entity p_sourceEntity);
        PickChoiceDelegate get_choicePicking();
    }
}
