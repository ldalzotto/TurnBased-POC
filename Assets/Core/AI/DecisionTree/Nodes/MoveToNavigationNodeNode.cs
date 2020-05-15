using _ActionPoint;
using _NavigationGraph;
using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Algorithm;

namespace _AI._DecisionTree
{
    /// <summary>
    /// The intention to move to the TargetNavigationNode.
    /// Updates the <see cref="AIDecisionScore.PathScore"/>.
    /// Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public class MoveToNavigationNodeNode : ADecisionNode
    {
        public NavigationNode SourceNavigationNode;
        public NavigationNode TargetNavigationNode;

        /// <summary>
        /// The list of all <see cref="NavigationNode"/> that will be followed by the calling <see cref="Entity"/>.
        /// </summary>
        public List<NavigationNode> CalculatedPath;

        public static MoveToNavigationNodeNode alloc(NavigationNode p_sourceNavigationNode, NavigationNode p_targetNavigationnode)
        {
            MoveToNavigationNodeNode l_instance = new MoveToNavigationNodeNode();
            l_instance.SourceNavigationNode = p_sourceNavigationNode;
            l_instance.TargetNavigationNode = p_targetNavigationnode;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode, ref EntityDecisionContext p_entityDecisionContextdata)
        {
            using (NavigationGraphAlgorithm.CalculatePathRequest l_calculationRequest = NavigationGraphAlgorithm.CalculatePathRequest.CalculatePathRequestPool.popOrCreate())
            {
                NavigationGraphAlgorithm.CalculatePathRequest.prepareForCalculation(
                                l_calculationRequest,
                                NavigationGraphContainer.UniqueNavigationGraph,
                                SourceNavigationNode,
                                TargetNavigationNode,
                                PathCalculationParameters.build(2.0f));
                NavigationGraphAlgorithm.CalculatePath(l_calculationRequest);
                ref NavigationPath l_calculatedNavigationPath = ref l_calculationRequest.ResultPath;
                p_entityDecisionContextdata.AIDecisionScore.PathScore += l_calculatedNavigationPath.PathCost;
                CalculatedPath = new List<NavigationNode>(l_calculatedNavigationPath.NavigationNodes);
            }
        }
    };

}

