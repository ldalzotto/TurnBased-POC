using System.Collections.Generic;

namespace _AI._DecisionTree._Algorithm
{
    public static class Traversal
    {
        /// <summary>
        /// Traversal of <see cref="DecisionTree"/> is done by recusrively calling the <see cref="ADecisionNode.TreeTraversal(ADecisionNode)"/>.
        /// </summary>
        public static RefList<AIDecisionTreeChoice> traverseDecisionTree(DecisionTree p_decisionTree)
        {
            RefList<AIDecisionTreeChoice> l_choices = new RefList<AIDecisionTreeChoice>();

            RefList<TraversalStack> l_traversalStacks = new RefList<TraversalStack>();
            l_traversalStacks.Add(TraversalStack.build(p_decisionTree.RootNode));

            while (l_traversalStacks.Count > 0)
            {
                ref TraversalStack l_currentTraversalStack = ref l_traversalStacks.ValueRef(l_traversalStacks.Count - 1);
                //If there if the current node has links
                if (l_currentTraversalStack.DecisionNode.LinkedNodes != null)
                {
                    if (l_currentTraversalStack.LinkIterationCounter < l_currentTraversalStack.DecisionNode.LinkedNodes.Count)
                    {
                        //We traverse the link and go one level deeper
                        ADecisionNode l_nextNode = l_currentTraversalStack.DecisionNode.LinkedNodes[l_currentTraversalStack.LinkIterationCounter];

                        l_currentTraversalStack.LinkIterationCounter += 1;
                        TraversalStack l_oneLevelDepperStack = TraversalStack.build(l_nextNode);
                        l_traversalStacks.AddRef(ref l_oneLevelDepperStack);

                        l_nextNode.TreeTraversal(l_currentTraversalStack.DecisionNode);
                    }
                    else
                    {
                        #region Updating stack

                        l_traversalStacks.RemoveAt(l_traversalStacks.Count - 1);

                        #endregion
                    }
                }
                else // No links have been found, this means that the current node is a leaf node.
                {
                    #region Creating the choice as the current node is a leaf

                    ADecisionNode[] l_choiceNodes = new ADecisionNode[l_traversalStacks.Count];
                    for (int i = 0; i < l_traversalStacks.Count; i++)
                    {
                        l_choiceNodes[i] = l_traversalStacks.ValueRef(i).DecisionNode;
                    }

                    AIDecisionTreeChoice l_choice = AIDecisionTreeChoice.build(l_choiceNodes);
                    l_choices.AddRef(ref l_choice);

                    #endregion

                    l_traversalStacks.RemoveAt(l_traversalStacks.Count - 1);
                }
            }
            return l_choices;
        }

        /// <summary>
        /// <see cref="TraversalStack"/> is a structure used by <see cref="Algorithm.traverseDecisionTree(DecisionTree, Entity)"/> that 
        /// represents the state of every <see cref="DecisionTree"/> possibility.
        /// </summary>
        struct TraversalStack
        {
            public ADecisionNode DecisionNode;
            public int LinkIterationCounter;

            public static TraversalStack build(ADecisionNode p_decisionNodeHandler)
            {
                TraversalStack l_instance = new TraversalStack();
                l_instance.DecisionNode = p_decisionNodeHandler;
                l_instance.LinkIterationCounter = 0;
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
            public ADecisionNode[] DecisionNodesChoiceOrdered;

            public static AIDecisionTreeChoice build(ADecisionNode[] p_decisionNodesChoiceOrdered)
            {
                AIDecisionTreeChoice l_instance = new AIDecisionTreeChoice();
                l_instance.DecisionNodesChoiceOrdered = p_decisionNodesChoiceOrdered;
                return l_instance;
            }
        };
    }

}