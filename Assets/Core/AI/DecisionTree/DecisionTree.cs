using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Algorithm;

namespace _AI._DecisionTree
{
    public class DecisionTree
    {
        public Dictionary<ADecisionNode, RefList<DecisionLink>> DecisionLinks;
        public RootDecisionNode RootNode;

        public static DecisionTree alloc()
        {
            DecisionTree l_instance = new DecisionTree();
            l_instance.DecisionLinks = new Dictionary<ADecisionNode, RefList<DecisionLink>>();
            l_instance.RootNode = new RootDecisionNode();
            return l_instance;
        }

        public static void linkDecisionNodes(DecisionTree p_decisiontree, ADecisionNode p_sourceNode, ADecisionNode p_targetNode)
        {
            DecisionLink l_instanciatedLink = DecisionLink.build(p_sourceNode, p_targetNode);
            if (!p_decisiontree.DecisionLinks.ContainsKey(p_sourceNode))
            {
                p_decisiontree.DecisionLinks.Add(p_sourceNode, new RefList<DecisionLink>() {l_instanciatedLink});
            }
            else
            {
                p_decisiontree.DecisionLinks[p_sourceNode].Add(l_instanciatedLink);
            }
        }
    }

    /// <summary>
    /// An <see cref="ADecisionNode"/> is a data container without any logic.
    /// It's purpose is to be red by the algorithm that traverse the Navigation tree.
    /// </summary>
    public abstract class ADecisionNode
    {
        public EDecisionNodeConsumerAction DecisionNodeConsumerAction;

        public virtual void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            DecisionNodeConsumerAction = EDecisionNodeConsumerAction.SKIP;
        }
    }

    public class RootDecisionNode : ADecisionNode
    {
    }

    public struct DecisionLink
    {
        public ADecisionNode Source;
        public ADecisionNode Target;

        public static DecisionLink build(ADecisionNode p_sourceNode, ADecisionNode p_targetNode)
        {
            DecisionLink l_instance = new DecisionLink()
            {
                Source = p_sourceNode,
                Target = p_targetNode
            };
            return l_instance;
        }
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