using System;
using System.Collections.Generic;

namespace _AI._DecisionTree._Algorithm
{
    public static class Algorithm
    {
        public static void traverseDecisionTree(DecisionTree p_decisionTree)
        {
            RefList<TraversalStack> l_traversalStacks = new RefList<TraversalStack>();
            l_traversalStacks.Add(TraversalStack.build(p_decisionTree.RootNode));


            while (l_traversalStacks.Count > 0)
            {
                ref TraversalStack l_currentTraversalStack = ref l_traversalStacks.ValueRef(l_traversalStacks.Count - 1);
                //If there if the current node ha links
                if (p_decisionTree.DecisionLinks.ContainsKey(l_currentTraversalStack.DecisionNodeHandler))
                {
                    RefList<DecisionLink> l_links = p_decisionTree.DecisionLinks[l_currentTraversalStack.DecisionNodeHandler];
                    if (l_currentTraversalStack.LinkIterationCounter < l_links.Count)
                    {
                        //We traverse the link and go one level deeper
                        ref DecisionLink l_link = ref l_links.ValueRef(l_currentTraversalStack.LinkIterationCounter);
                        l_link.Target.TreeTraversal(l_link.Source);

                        l_currentTraversalStack.LinkIterationCounter += 1;
                        TraversalStack l_oneLevelDepperStack = TraversalStack.build(l_link.Target);
                        l_traversalStacks.AddRef(ref l_oneLevelDepperStack);
                    }
                    else
                    {
                        l_traversalStacks.RemoveAt(l_traversalStacks.Count - 1);
                    }
                }
                else
                {
                    l_traversalStacks.RemoveAt(l_traversalStacks.Count - 1);
                }
            }
        }


        struct TraversalStack
        {
            public ADecisionNode DecisionNodeHandler;
            public int LinkIterationCounter;

            public static TraversalStack build(ADecisionNode p_decisionNodeHandler)
            {
                TraversalStack l_instance = new TraversalStack();
                l_instance.DecisionNodeHandler = p_decisionNodeHandler;
                l_instance.LinkIterationCounter = 0;
                return l_instance;
            }
        }
    }
}