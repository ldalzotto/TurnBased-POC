using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Algorithm;

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
        public EDecisionNodeConsumerAction DecisionNodeConsumerAction;

        public virtual void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            DecisionNodeConsumerAction = EDecisionNodeConsumerAction.SKIP;
        }
    }

    public class RootDecisionNode : ADecisionNode
    {
    }


    /// <summary>
    /// This enumeration is used by the consumer of the <see cref="DecisionTree"/>. It indicates whether or not an operation
    /// must be executed when the choiche made encounter the associated <see cref="ADecisionNode"/>.
    /// </summary>
    public enum EDecisionNodeConsumerAction : ushort
    {
        SKIP = 0,

        /// <summary>
        /// The consumer is likely to execute another process by reading the associated <see cref="ADecisionNode"/> data.
        /// </summary>
        EXECUTE = 1
    };
}