using _ActionPoint;
using _Entity;
using System.Collections.Generic;
using Unity.Mathematics;
using static _AI._DecisionTree._Algorithm.Traversal;

namespace _AI._DecisionTree._Algorithm
{
    public static class ChoicePicking
    {

        public delegate ref AIDecisionTreeChoice PickChoiceDelegate(RefList<AIDecisionTreeChoice> p_choices, Entity p_calledEntity);

        /// <summary>
        /// Picking the best choice between <paramref name="p_choices"/>.
        /// The choice is made by associating a score to every choices. The choice with the highest score is picked.
        /// </summary>
        /// <param name="p_choices"> Compared choices. </param>
        /// <returns> The picked choice </returns>
        public static ref AIDecisionTreeChoice defaultTestPickChoice(RefList<AIDecisionTreeChoice> p_choices, Entity p_calledEntity)
        {
            // Becuase PathScore represents the distance crossed and that we want to minimize movement, 
            // we normalize the PathScore by it's potential maxmimum value calculated if the Entity were using all it's ActionPoints to move.
            ActionPoint l_calledEntityActionPoint = EntityComponent.get_component<ActionPoint>(p_calledEntity);
            float l_maxPathScoreThatCanBeCrossed = _ActionPoint.Calculations.actionPointToCrossableWorldDistance(l_calledEntityActionPoint.ActionPointData.CurrentActionPoints);


            RefList<AIDecisionScore> l_choiceScores = new RefList<AIDecisionScore>(p_choices.Count);

            for (int i = 0; i < p_choices.Count; i++)
            {
                AIDecisionScore l_choiceScore = AIDecisionScore.build();

                ref AIDecisionTreeChoice l_aIDecisionTreeChoice = ref p_choices.ValueRef(i);

                for (int j = 0; j < l_aIDecisionTreeChoice.DecisionNodesChoiceOrdered.Length; j++)
                {
                    switch (l_aIDecisionTreeChoice.DecisionNodesChoiceOrdered[j])
                    {
                        case MoveToNavigationNodeNode l_moveToNavigationNodeNode:
                            {
                                l_choiceScore.PathScore += math.max(l_maxPathScoreThatCanBeCrossed - l_moveToNavigationNodeNode.PathCost, 0.0f);
                                break;
                            }
                        case AttackNode l_attackNode:
                            {
                                l_choiceScore.DamageScore += l_attackNode.DamageDone;
                                break;
                            }
                        case HealNode l_healNode:
                            {
                                l_choiceScore.HealScore += l_healNode.RecoveredHealth;
                                break;
                            }
                    }
                }

                l_choiceScores.AddRef(ref l_choiceScore);
            }


            int l_choiceIndex = 0;
            ref AIDecisionTreeChoice l_choiceWithTheHighestTotalScore = ref p_choices.ValueRef(l_choiceIndex);

            for (int i = 0; i < p_choices.Count; i++)
            {
                ref AIDecisionTreeChoice l_aIDecisionTreeChoice = ref p_choices.ValueRef(i);
                ref AIDecisionScore l_aIDecisionScore = ref l_choiceScores.ValueRef(i);
                if (AIDecisionScore.totalScore(ref l_aIDecisionScore) > AIDecisionScore.totalScore(ref l_choiceScores.ValueRef(l_choiceIndex)))
                {
                    l_choiceIndex = i;
                    l_choiceWithTheHighestTotalScore = l_aIDecisionTreeChoice;
                }
            }

            return ref l_choiceWithTheHighestTotalScore;
        }


        /// <summary>
        /// The AIDecisionScore is used to quantify an AIDecisionTree choice.
        /// Values are update by IAIDecisionLink when traversed.
        /// </summary>
        public struct AIDecisionScore
        {
            /// <summary>
            /// The distance crossed along the NavigationGraph. 
            /// </summary>
            public float PathScore;

            /// <summary>
            /// The amount of damage done by Attack.
            /// </summary>
            public float DamageScore;

            /// <summary>
            /// The amount of healing done.
            /// </summary>
            public float HealScore;

            public static AIDecisionScore build()
            {
                AIDecisionScore l_instance = new AIDecisionScore();
                l_instance.PathScore = 0.0f;
                l_instance.DamageScore = 0.0f;
                l_instance.HealScore = 0.0f;
                return l_instance;
            }

            public static float totalScore(ref AIDecisionScore p_aiDecisionScore)
            {
                return p_aiDecisionScore.DamageScore + p_aiDecisionScore.PathScore + p_aiDecisionScore.HealScore;
            }
        }
    }
}

