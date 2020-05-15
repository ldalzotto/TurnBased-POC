using _AI._DecisionTree._Algorithm;
using _Entity;
using _NavigationGraph;
using System.Collections.Generic;

namespace _AI._DecisionTree
{
    /// <summary>
    /// The intention to move to the TargetNavigationNode.
    /// </summary>
    public class MoveToNavigationNodeNode : ADecisionNode, IAtionPointPredictable
    {
        public NavigationNode SourceNavigationNode;
        public NavigationNode TargetNavigationNode;

        /// <summary>
        /// The list of all <see cref="NavigationNode"/> that will be followed by the calling <see cref="Entity"/>.
        /// </summary>
        public List<NavigationNode> NavigationPath;
        public float PathCost;
        public static MoveToNavigationNodeNode alloc(NavigationNode p_sourceNavigationNode, NavigationNode p_targetNavigationnode)
        {
            MoveToNavigationNodeNode l_instance = new MoveToNavigationNodeNode();
            l_instance.SourceNavigationNode = p_sourceNavigationNode;
            l_instance.TargetNavigationNode = p_targetNavigationnode;
            return l_instance;
        }


        public override void TreeTraversal(ADecisionNode p_sourceNode)
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
                NavigationPath = new List<NavigationNode>(l_calculatedNavigationPath.NavigationNodes);
                PathCost = l_calculatedNavigationPath.PathCost;
            }
        }

        public void AddActionPointPrediction(List<float> p_actionPointPredictions)
        {
            if (NavigationPath != null && NavigationPath.Count > 1)
            {
                for (int i = 1; i < NavigationPath.Count; i++)
                {
                    p_actionPointPredictions.Add(_ActionPoint.Calculations.actionPointBetweenNavigationNodes(NavigationPath[i - 1], NavigationPath[i]));
                }
            }
        }
    };

}

