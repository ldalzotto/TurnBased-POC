using _ActionPoint;
using _AI._Behavior;
using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _Entity._Events;
using _EventQueue;
using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Traversal;
using static _AI._DecisionTree._Algorithm.TreeIteration;

namespace _Entity._Turn
{
    public static class TurnIteration
    {
        /// <summary>
        /// Build a <see cref="DecisionTree"/> and push generated <see cref="AEvent"/> from the choice made.
        /// Returns true if an additional iteration is requested after all <see cref="AEvent"/> have been executed.
        /// </summary>
        public static bool Iterate(Entity p_entity, EventQueue p_eventQueue)
        {
            AIBehavior l_aiBehavior = EntityComponent.get_component<AIBehavior>(p_entity);
            if (l_aiBehavior != null)
            {
                TreeIterationResult l_treeIterationResult = TreeIteration.iterate(l_aiBehavior);
                PushEventsFromAIDecision(p_entity, p_eventQueue, ref l_treeIterationResult.PickedChoice);
                return DoesTheEntityNeedsAnAdditionalTurnIteration(p_entity, l_treeIterationResult.ActionPointConsumptionPrediction);
            }
            else
            {
                return false;
            }

        }

        private static void PushEventsFromAIDecision(Entity p_entity, EventQueue p_eventQueue, ref AIDecisionTreeChoice p_aiDecision)
        {
            for (int i = 0; i < p_aiDecision.DecisionNodesChoiceOrdered.Length; i++)
            {
                ADecisionNode l_decisionNode = p_aiDecision.DecisionNodesChoiceOrdered[i];

                switch (l_decisionNode)
                {
                    // Push to the event queue the will of moving along a path
                    case MoveToNavigationNodeNode l_moveToNavigationNode:
                        {
                            EventBuilder.moveToNavigationNode(p_entity, p_eventQueue, l_moveToNavigationNode.NavigationPath);
                        }
                        break;

                    case AttackNode l_attackNode:
                        {
                            for (int j = 0; j < l_attackNode.NumberOfAttacks; j++)
                            {
                                EventBuilder.attackEvent(p_entity, p_eventQueue, l_attackNode.SourceEntity, l_attackNode.TargetEntity, l_attackNode.Attack);
                            }
                        }
                        break;
                }
            }
        }

        private static bool DoesTheEntityNeedsAnAdditionalTurnIteration(Entity p_entity, List<float> p_actionpoinConsumptionPrediction)
        {
            if (p_actionpoinConsumptionPrediction == null || p_actionpoinConsumptionPrediction.Count == 0)
            {
                return false;
            }

            float l_virtualActionPoint = EntityComponent.get_component<ActionPoint>(p_entity).ActionPointData.CurrentActionPoints;
            // The action points is already depleted.
            if (l_virtualActionPoint <= 0.0f)
            {
                return false;
            }

            l_virtualActionPoint -= p_actionpoinConsumptionPrediction[0];
            // This means that the first action cannot be performed.
            if (l_virtualActionPoint <= 0.0f)
            {
                return false;
            }

            return true;
        }

    }
}