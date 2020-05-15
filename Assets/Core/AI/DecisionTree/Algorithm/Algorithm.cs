using _ActionPoint;
using _Entity;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _AI._DecisionTree._Algorithm
{
    public static class Algorithm
    {

        public static Action<AIdecisionTreeTraversalResponse> OnAIDecisionTreeTraversed;

        public static AIdecisionTreeTraversalResponse traverseDecisionTree(DecisionTree p_decisionTree, Entity p_calledEntity)
        {
            RefList<AIDecisionTreeChoice> l_choices = new RefList<AIDecisionTreeChoice>();

            RefList<TraversalStack> l_traversalStacks = new RefList<TraversalStack>();
            l_traversalStacks.Add(TraversalStack.build(p_decisionTree.RootNode, EntityDecisionContext.build()));

            while (l_traversalStacks.Count > 0)
            {
                ref TraversalStack l_currentTraversalStack = ref l_traversalStacks.ValueRef(l_traversalStacks.Count - 1);
                //If there if the current node ha links
                if (l_currentTraversalStack.DecisionNode.LinkedNodes != null)
                {
                    if (l_currentTraversalStack.LinkIterationCounter < l_currentTraversalStack.DecisionNode.LinkedNodes.Count)
                    {
                        //We traverse the link and go one level deeper
                        ADecisionNode l_nextNode = l_currentTraversalStack.DecisionNode.LinkedNodes[l_currentTraversalStack.LinkIterationCounter];

                        l_currentTraversalStack.LinkIterationCounter += 1;
                        TraversalStack l_oneLevelDepperStack = TraversalStack.build(l_nextNode, l_currentTraversalStack.EntityDecisionContextdata);
                        l_traversalStacks.AddRef(ref l_oneLevelDepperStack);

                        l_nextNode.TreeTraversal(l_currentTraversalStack.DecisionNode, ref l_traversalStacks.ValueRef(l_traversalStacks.Count - 1).EntityDecisionContextdata);
                    }
                    else
                    {
                        #region Updating stack

                        l_traversalStacks.RemoveAt(l_traversalStacks.Count - 1);

                        #endregion
                    }
                }
                else // Ne links have been found, this means that the current node is a leaf node.
                {
                    #region Creating the choice as the current node is a leaf

                    ADecisionNode[] l_choiceNodes = new ADecisionNode[l_traversalStacks.Count];
                    for (int i = 0; i < l_traversalStacks.Count; i++)
                    {
                        l_choiceNodes[i] = l_traversalStacks.ValueRef(i).DecisionNode;
                    }

                    AIDecisionTreeChoice l_choice = AIDecisionTreeChoice.build(ref l_currentTraversalStack.EntityDecisionContextdata.AIDecisionScore, l_choiceNodes);
                    l_choices.AddRef(ref l_choice);

                    #endregion

                    l_traversalStacks.RemoveAt(l_traversalStacks.Count - 1);
                }
            }

            ref AIDecisionTreeChoice l_pickedChoice = ref pickTreeChoice(l_choices, EntityComponent.get_component<ActionPoint>(p_calledEntity));
            AIdecisionTreeTraversalResponse l_response = AIdecisionTreeTraversalResponse.build(ref l_pickedChoice, l_choices);
            OnAIDecisionTreeTraversed?.Invoke(l_response);
            return l_response;
        }

        /// <summary>
        /// Picking the best choice between <paramref name="p_choices"/>.
        /// The choice is made by associating a score to every choices. The choice with the highest score is picked.
        /// </summary>
        /// <param name="p_choices"> Compared choices. </param>
        /// <returns> The picked choice </returns>
        static ref AIDecisionTreeChoice pickTreeChoice(RefList<AIDecisionTreeChoice> p_choices, ActionPoint p_calledEntityActionPoint)
        {
            // Becuase PathScore represents the distance crossed and that we want to minimize movement, 
            // we normalize the PathScore by it's potential maxmimum value calculated if the Entity were using all it's ActionPoints to move.
            float l_maxPathScoreThatCanBeCrossed = _ActionPoint.Calculations.actionPointToCrossableWorldDistance(p_calledEntityActionPoint.ActionPointData.CurrentActionPoints);

            for (int i = 0; i < p_choices.Count; i++)
            {
                ref AIDecisionTreeChoice l_aIDecisionTreeChoice = ref p_choices.ValueRef(i);
            }

            for (int i = 0; i < p_choices.Count; i++)
            {
                ref AIDecisionTreeChoice l_aIDecisionTreeChoice = ref p_choices.ValueRef(i);
                if (l_maxPathScoreThatCanBeCrossed != 0.0f)
                {
                    l_aIDecisionTreeChoice.AIDecisionScore.PathScore =
                        math.max(l_maxPathScoreThatCanBeCrossed - l_aIDecisionTreeChoice.AIDecisionScore.PathScore, 0.0f);
                }
            }

            ref AIDecisionTreeChoice l_choiceWithTheHighestTotalScore = ref p_choices.ValueRef(0);

            for (int i = 0; i < p_choices.Count; i++)
            {
                ref AIDecisionTreeChoice l_aIDecisionTreeChoice = ref p_choices.ValueRef(i);
                if (AIDecisionScore.totalScore(ref l_aIDecisionTreeChoice.AIDecisionScore) > AIDecisionScore.totalScore(ref l_choiceWithTheHighestTotalScore.AIDecisionScore))
                {
                    l_choiceWithTheHighestTotalScore = l_aIDecisionTreeChoice;
                }
            }

            return ref l_choiceWithTheHighestTotalScore;
        }

        /// <summary>
        /// <see cref="TraversalStack"/> is a structure used by <see cref="Algorithm.traverseDecisionTree(DecisionTree, Entity)"/> that 
        /// represents the state of every <see cref="DecisionTree"/> possibility.
        /// </summary>
        struct TraversalStack
        {
            public ADecisionNode DecisionNode;
            public int LinkIterationCounter;
            public EntityDecisionContext EntityDecisionContextdata;

            public static TraversalStack build(ADecisionNode p_decisionNodeHandler, EntityDecisionContext p_entityDecisionContext)
            {
                TraversalStack l_instance = new TraversalStack();
                l_instance.DecisionNode = p_decisionNodeHandler;
                l_instance.LinkIterationCounter = 0;
                l_instance.EntityDecisionContextdata = p_entityDecisionContext;
                return l_instance;
            }
        }

        public struct EntityDecisionContext
        {
            public AIDecisionScore AIDecisionScore;

             public static EntityDecisionContext build()
            {
                EntityDecisionContext l_instance = new EntityDecisionContext();
                return l_instance;
            }
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

            public static float totalScore(ref AIDecisionScore p_aiDecisionScore)
            {
                return p_aiDecisionScore.DamageScore + p_aiDecisionScore.PathScore + p_aiDecisionScore.HealScore;
            }
        }


        public struct AIdecisionTreeTraversalResponse
        {
            public AIDecisionTreeChoice PickedChoice;
            public RefList<AIDecisionTreeChoice> AllChoices;

            public static AIdecisionTreeTraversalResponse build(ref AIDecisionTreeChoice p_pickedChoice, RefList<AIDecisionTreeChoice> p_allChoices)
            {
                AIdecisionTreeTraversalResponse l_instance = new AIdecisionTreeTraversalResponse();
                l_instance.PickedChoice = p_pickedChoice;
                l_instance.AllChoices = p_allChoices;
                return l_instance;
            }
        }

        /// <summary>
        /// The <see cref="AIDecisionTreeChoice"/> represents graph traversal from root to leaf.
        /// By traversing all these <see cref="ADecisionNode"/>, the <see cref="AIDecisionScore"/> is the <see cref="EntityDecisionContext"/> state
        /// for the leaf node.
        /// </summary>
        public struct AIDecisionTreeChoice
        {
            public AIDecisionScore AIDecisionScore;
            public ADecisionNode[] DecisionNodesChoiceOrdered;

            public static AIDecisionTreeChoice build(ref AIDecisionScore p_aIDecisionScore, ADecisionNode[] p_decisionNodesChoiceOrdered)
            {
                AIDecisionTreeChoice l_instance = new AIDecisionTreeChoice();
                l_instance.AIDecisionScore = p_aIDecisionScore;
                l_instance.DecisionNodesChoiceOrdered = p_decisionNodesChoiceOrdered;
                return l_instance;
            }
        };
    }
}