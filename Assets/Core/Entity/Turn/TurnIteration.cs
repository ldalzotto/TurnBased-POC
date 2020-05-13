using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _AI._DecisionTree._Builder;
using _Entity._Events;
using _EventQueue;
using static _AI._DecisionTree._Algorithm.Algorithm;

namespace _Entity._Turn
{
    public static class TurnIteration
    {
        public static void Iterate(Entity p_entity, EventQueue p_eventQueue)
        {
            DecisionTree l_decisionTree = DecisionTree.alloc();
            TreeBuilder.buildAggressiveTree(l_decisionTree, p_entity);
            var l_choice = Algorithm.traverseDecisionTree(l_decisionTree, p_entity);
            PushEventsFromAIDecision(p_entity, p_eventQueue, l_choice);
        }

        private static void PushEventsFromAIDecision(Entity p_entity, EventQueue p_eventQueue, AIdecisionTreeTraversalResponse p_aiDecision)
        {
            // TODO -> this piece of logic must be elsewhere.
            for (int i = 0; i < p_aiDecision.PickedChoice.DecisionNodesChoiceOrdered.Length; i++)
            {
                ADecisionNode l_decisionNode = p_aiDecision.PickedChoice.DecisionNodesChoiceOrdered[i];
                if (l_decisionNode.DecisionNodeConsumerAction == EDecisionNodeConsumerAction.EXECUTE)
                {
                    switch (l_decisionNode)
                    {
                        // Push to the event queue the will of moving along a path
                        case MoveToNavigationNodeNode l_moveToNavigationNode:
                            EventBuilder.moveToNavigationNode(p_entity, p_eventQueue, l_moveToNavigationNode.CalculatedPath);
                            break;

                        case AttackNode l_attackNode:
                            for (int j = 0; j < l_attackNode.NumberOfAttacks; j++)
                            {
                                EventBuilder.attackEvent(p_entity, p_eventQueue, l_attackNode.SourceEntity, l_attackNode.TargetEntity, l_attackNode.Attack);
                            }
                            break;
                    }
                }
            }
        }


    }
}