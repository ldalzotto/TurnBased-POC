using System.Collections.Generic;

namespace _AI._DecisionTree
{
    public class DecisionTree
    {
        public RootDecisionNode RootNode;

        public static DecisionTree alloc()
        {
            DecisionTree l_instance = new DecisionTree();
            l_instance.RootNode = new RootDecisionNode();
            return l_instance;
        }

        public static void linkDecisionNodes(DecisionTree p_decisiontree, ADecisionNode p_sourceNode, ADecisionNode p_targetNode)
        {
            if (p_sourceNode.LinkedNodes == null) { p_sourceNode.LinkedNodes = new List<ADecisionNode>(); }
            p_sourceNode.LinkedNodes.Add(p_targetNode);
        }
    }

    /// <summary>
    /// An <see cref="ADecisionNode"/> is a data container without any logic.
    /// It's purpose is to be red by the algorithm that traverse the Navigation tree.
    /// </summary>
    public abstract class ADecisionNode
    {
        public List<ADecisionNode> LinkedNodes;

        public virtual void TreeTraversal(ADecisionNode p_sourceNode)
        {
        }
    }

    public class RootDecisionNode : ADecisionNode
    {
    }
}