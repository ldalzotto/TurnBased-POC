using _Memory;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _NavigationGraph
{
    /// <summary>
    /// Structure used by all algorithm that calculates <see cref="NavigationPath"/>.
    /// It is used as a unique parameter object for these calculations.
    /// </summary>
    public struct PathCalculationParameters
    {
        /// <summary>
        /// This factor controls the weight of the <see cref="NavigationNodePathTraversalCalculations.HeuristicScore"/> calculation.
        /// If > 1, <see cref="NavigationNode"/> with higher <see cref="NavigationNodePathTraversalCalculations.HeuristicScore"/> are likely to be picked for calculation 
        /// even if ovther <see cref="NavigationNode"/>'s path score is higher.
        /// </summary>
        public float HeurisitcDistanceWeight;

        public static PathCalculationParameters build(float p_heurisitcDistanceWeight)
        {
            return new PathCalculationParameters()
            {
                HeurisitcDistanceWeight = p_heurisitcDistanceWeight
            };
        }
    }

    public struct NavigationPath
    {
        public List<NavigationNode> NavigationNodes;
        public Dictionary<NavigationNode, NavigationNodePathTraversalCalculations> NavigationNodesTraversalCalculations;
        public float PathCost;

        public static NavigationPath build(List<NavigationNode> p_navigationNodes, Dictionary<NavigationNode, NavigationNodePathTraversalCalculations> p_calculations)
        {
            NavigationPath l_instance = new NavigationPath();
            l_instance.NavigationNodes = p_navigationNodes;
            l_instance.NavigationNodesTraversalCalculations = p_calculations;
            l_instance.PathCost = 0.0f;
            return l_instance;
        }

        public static void reset(ref NavigationPath p_navigationPath)
        {
            p_navigationPath.NavigationNodes.Clear();
            p_navigationPath.NavigationNodesTraversalCalculations.Clear();
            p_navigationPath.PathCost = 0.0f;
        }

        public static void limitPathByMaximumPathCost(ref NavigationPath p_navigationPath, float p_maxPathCost)
        {
            if (p_maxPathCost >= 0.00f)
            {
                int l_lastIndex = -1;
                for (int i = 0; i < p_navigationPath.NavigationNodes.Count; i++)
                {
                    NavigationNode l_navigationNode = p_navigationPath.NavigationNodes[i];
                    float l_calculatedpathScore = p_navigationPath.NavigationNodesTraversalCalculations[l_navigationNode].PathScore;
                    if (l_calculatedpathScore > p_maxPathCost)
                    {
                        if (l_lastIndex <= 0)
                        {
                            p_navigationPath.NavigationNodes.Clear();
                            p_navigationPath.PathCost = 0.0f;
                        }
                        else
                        {
                            p_navigationPath.NavigationNodes.RemoveRange(i, p_navigationPath.NavigationNodes.Count - i);
                            p_navigationPath.PathCost = p_navigationPath.NavigationNodesTraversalCalculations[p_navigationPath.NavigationNodes[i - 1]].PathScore;
                        }
                        return;
                    }
                    l_lastIndex = i;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class NavigationGraphAlgorithm
    {
        /// <summary>
        /// Path calculation is a recursive algorithm that try all possibilities of <see cref="NavigationLink"/> from the <paramref name="p_beginNode"/> to reach the <paramref name="p_endNode"/>.
        /// These possibilities are ordered by a score (see <see cref="NavigationNodePathTraversalCalculations"/> for details) that represents the "difficulty" to reach the destination.
        /// The combinaison of <see cref="NavigationLink"/> that leads to the lower score is the resulting path.
        /// </summary>
        public static void CalculatePath(CalculatePathRequest p_request)
        {
            if (p_request.BeginNode != null && p_request.EndNode != null)
            {
                if (p_request.BeginNode == p_request.EndNode)
                {
                    p_request.ResultPath.NavigationNodesTraversalCalculations[p_request.BeginNode] = NavigationNodePathTraversalCalculations.build();
                    return;
                }

                NavigationNode l_currentEvaluatedNode = p_request.BeginNode;
                bool l_isFirstTimeInLoop = true;

                while (l_currentEvaluatedNode != null && l_currentEvaluatedNode != p_request.EndNode)
                {

                    // If this is not the start, we find the next current node to pick
                    if (!l_isFirstTimeInLoop)
                    {
                        l_currentEvaluatedNode = pickNextCurrentNodeToCalculate(p_request.ResultPath.NavigationNodesTraversalCalculations, p_request.NodesElligibleForNextCurrent);
                    }
                    else
                    {
                        l_isFirstTimeInLoop = false;
                        NavigationNodePathTraversalCalculations l_pathTraversalCaluclation = NavigationNodePathTraversalCalculations.build();
                        l_pathTraversalCaluclation.CalculationMadeFrom = l_currentEvaluatedNode;
                        p_request.ResultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode] = l_pathTraversalCaluclation;
                        p_request.NodesElligibleForNextCurrent.Add(l_currentEvaluatedNode);
                    }

                    if (l_currentEvaluatedNode != null)
                    {
                        // For the current node, we evaluate the score of it's neighbors

                        // The current evaluated node is no more ellisible because it has just been traversed by the algorithm
                        p_request.NodesElligibleForNextCurrent.Remove(l_currentEvaluatedNode);

                        // If the current evaluated node has links
                        if (p_request.NavigationGraph.NodeLinksIndexedByStartNode.ContainsKey(l_currentEvaluatedNode))
                        {
                            List<NavigationLink> l_evaluatedNavigationLinks = p_request.NavigationGraph.NodeLinksIndexedByStartNode[l_currentEvaluatedNode];
                            for (int i = 0; i < l_evaluatedNavigationLinks.Count; i++)
                            {
                                NavigationLink l_link = l_evaluatedNavigationLinks[i];

                                // We calculate the score as if the linked node is traversed.
                                NavigationNodePathTraversalCalculations l_currentCalculation = p_request.ResultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode];
                                float l_calculatedPathScore = simulateNavigationLinkTraversal(ref l_currentCalculation, ref l_link);

                                // If the neighbor has not already been calculated
                                if (!p_request.ResultPath.NavigationNodesTraversalCalculations.ContainsKey(l_link.EndNode))
                                {
                                    NavigationNode l_linkEndNode = l_link.EndNode;

                                    NavigationNodePathTraversalCalculations l_linkNodeCalculations = NavigationNodePathTraversalCalculations.build();
                                    updatePathScore(ref l_linkNodeCalculations, l_calculatedPathScore, l_currentEvaluatedNode);
                                    calculateHeuristicScore(ref l_linkNodeCalculations, l_linkEndNode, p_request.EndNode, p_request.PathCalculationParameters.HeurisitcDistanceWeight);
                                    p_request.ResultPath.NavigationNodesTraversalCalculations[l_linkEndNode] = l_linkNodeCalculations;

                                    p_request.NodesElligibleForNextCurrent.Add(l_linkEndNode);
                                }
                                // Else, we update score calculations only if the current calculated score is lower than the previous one.
                                // This means we have found a less costly path.
                                else
                                {
                                    NavigationNode l_linkEndNode = l_link.EndNode;

                                    NavigationNodePathTraversalCalculations l_linkNodeCalculations = p_request.ResultPath.NavigationNodesTraversalCalculations[l_linkEndNode];
                                    updatePathScore(ref l_linkNodeCalculations, l_calculatedPathScore, l_currentEvaluatedNode);
                                    p_request.ResultPath.NavigationNodesTraversalCalculations[l_linkEndNode] = l_linkNodeCalculations;
                                }
                            }
                        }
                    }
                }

                // calculating final path by going to the start by taking "CalculationMadeFrom" nodes
                if (l_currentEvaluatedNode != null)
                {
                    p_request.ResultPath.NavigationNodes.Add(l_currentEvaluatedNode);

                    NavigationNodePathTraversalCalculations l_navigationNodePathTraversalCalculations = p_request.ResultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode];
                    p_request.ResultPath.PathCost = NavigationNodePathTraversalCalculations.calculateTotalScore(ref l_navigationNodePathTraversalCalculations);


                    while (l_currentEvaluatedNode != p_request.BeginNode)
                    {
                        NavigationNode l_parent = p_request.ResultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode].CalculationMadeFrom;
                        if (l_parent != null)
                        {
                            p_request.ResultPath.NavigationNodes.Add(l_parent);
                            l_currentEvaluatedNode = l_parent;
                        }
                        else
                        {
                            l_currentEvaluatedNode = p_request.BeginNode;
                        }
                    }

                    p_request.ResultPath.NavigationNodes.Reverse();
                }
            }
        }

        /**
             The picked NavigationNode with the lowest total score (retrieved from NavigationNodePathTraversalCalculations) is returned
        */
        private static NavigationNode pickNextCurrentNodeToCalculate(
                 Dictionary<NavigationNode, NavigationNodePathTraversalCalculations> l_pathScoreCalculations,
                 List<NavigationNode> l_pathNodesElligibleForNextCurrent)
        {
            NavigationNode l_currentSelectedNode = null;
            float l_currentTotalScore = 0.0f;

            for (int i = 0; i < l_pathNodesElligibleForNextCurrent.Count; i++)
            {
                NavigationNode l_currentComparedNavigationnode = l_pathNodesElligibleForNextCurrent[i];

                if (l_currentSelectedNode == null)
                {
                    l_currentSelectedNode = l_currentComparedNavigationnode;
                    NavigationNodePathTraversalCalculations l_navigationNodePathTraversalCalculations = l_pathScoreCalculations[l_currentComparedNavigationnode];
                    l_currentTotalScore = NavigationNodePathTraversalCalculations.calculateTotalScore(ref l_navigationNodePathTraversalCalculations);
                }
                else
                {
                    NavigationNodePathTraversalCalculations l_navigationNodePathTraversalCalculations = l_pathScoreCalculations[l_currentComparedNavigationnode];
                    float l_currentComparedTotalScore = NavigationNodePathTraversalCalculations.calculateTotalScore(ref l_navigationNodePathTraversalCalculations);
                    if (l_currentComparedTotalScore < l_currentTotalScore)
                    {
                        l_currentTotalScore = l_currentComparedTotalScore;
                        l_currentSelectedNode = l_currentComparedNavigationnode;
                    }
                }
            }

            return l_currentSelectedNode;
        }

        /**
            Calculates the path score as if the p_traversedNavigationLink_ptr has been traversed.
        */
        private static float simulateNavigationLinkTraversal(ref NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations, ref NavigationLink p_navigationLink)
        {
            return p_navigationNodePathTraversalCalculations.PathScore + p_navigationLink.TravelCost;
        }

        /** 
            When the caller wants to update the m_pathScore with an already calculated Path score (p_newPathScore) adn from which NavigationNode this calculation has been done (p_calculatedFrom_ptr).
            p_newPathScore andp_calculatedFrom_ptr is keeped only if the p_newPathScore is lower than the current one.
        */
        private static void updatePathScore(
                ref NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations,
                float p_newPathScore,
                NavigationNode p_calculatedFrom)
        {
            if (p_navigationNodePathTraversalCalculations.CalculationMadeFrom == null || p_newPathScore < p_navigationNodePathTraversalCalculations.PathScore)
            {
                p_navigationNodePathTraversalCalculations.PathScore = p_newPathScore;
                p_navigationNodePathTraversalCalculations.CalculationMadeFrom = p_calculatedFrom;
            }
        }

        /**
            The Heuristic score is the distance between the associated NavigationNode and the target of the path algorithm.
        */
        private static void calculateHeuristicScore(
           ref NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations,
           NavigationNode p_startNode,
           NavigationNode p_endNode,
           float p_heurisitcDistanceMultiplier)
        {
            p_navigationNodePathTraversalCalculations.HeuristicScore =
                math.distance(p_startNode.LocalPosition, p_endNode.LocalPosition) * p_heurisitcDistanceMultiplier;
        }



        public static NavigationNode pickRandomNode(NavigationGraph p_navigationGraph)
        {
            return p_navigationGraph.NavigationNodes[MyRandom.Random.NextInt(0, p_navigationGraph.NavigationNodes.Count)];
        }

        public static NavigationNode getNearestNode(NavigationGraph p_navigationGraph, float3 p_localPosition)
        {
            NavigationNode l_nearestNavigationNode = p_navigationGraph.NavigationNodes[0];
            for (int i = 1; i < p_navigationGraph.NavigationNodes.Count; i++)
            {
                if (math.distance(l_nearestNavigationNode.LocalPosition, p_localPosition) >
                        math.distance(l_nearestNavigationNode.LocalPosition, p_navigationGraph.NavigationNodes[i].LocalPosition))
                {
                    l_nearestNavigationNode = p_navigationGraph.NavigationNodes[i];
                }
            }
            return l_nearestNavigationNode;
        }


        public class CalculatePathRequest : IPoolable, IDisposable
        {
            public static MemoryBufferStack<CalculatePathRequest> CalculatePathRequestPool;
            static CalculatePathRequest()
            {
                CalculatePathRequestPool = MemoryBufferStack<CalculatePathRequest>.alloc(() => { return CalculatePathRequest.alloc(); });
            }

            public NavigationGraph NavigationGraph;
            public NavigationNode BeginNode;
            public NavigationNode EndNode;
            public PathCalculationParameters PathCalculationParameters;

            // This list are all nodes available to be picked for the next current node.
            // This means that they all haven't been traversed by the algorithm but not calculated yet
            public List<NavigationNode> NodesElligibleForNextCurrent;

            public NavigationPath ResultPath;

            public static CalculatePathRequest alloc()
            {
                CalculatePathRequest l_instance = new CalculatePathRequest();
                l_instance.NodesElligibleForNextCurrent = new List<NavigationNode>();
                l_instance.ResultPath = NavigationPath.build(new List<NavigationNode>(), new Dictionary<NavigationNode, NavigationNodePathTraversalCalculations>());
                return l_instance;
            }


            public static void prepareForCalculation(CalculatePathRequest p_request, NavigationGraph p_navigationGraph, NavigationNode p_beginNode, NavigationNode p_endNode,
                        PathCalculationParameters p_pathCalculationParameters)
            {
                p_request.NavigationGraph = p_navigationGraph;
                p_request.BeginNode = p_beginNode;
                p_request.EndNode = p_endNode;
                p_request.PathCalculationParameters = p_pathCalculationParameters;
            }

            public void ClearForNewInstance()
            {
                NavigationGraph = null;
                BeginNode = null;
                EndNode = null;
                NodesElligibleForNextCurrent.Clear();
                NavigationPath.reset(ref ResultPath);
            }

            public void Dispose()
            {
                CalculatePathRequestPool.push(this);
            }
        }


        /*
            Find all NavigationNodes that the p_requestedNode can travel to and have direct NavigationLink between them.
                - "Reachable" : Means that it exists a path between the p_requestedNode and the returned NavigationNode
                - "Neighbor" : Means that the p_requestedNode and the returned NavigationNode have a direct NavigationLink between them.
        */
        public static IEnumerable<NavigationNode> getReachableNeighborNavigationNodes(NavigationGraph p_navigationGraph, NavigationNode p_requestedNode, NavigationGraphFlag p_navigationGraphFlag)
        {
            var l_navigationLinksGoingFromTheRequestedNode = NavigationGraph.get_nodeLinksIndexedByStartNode(p_navigationGraph, p_navigationGraphFlag)[p_requestedNode];

            var l_navigationLinksGoingFromTheRequestedNodeEnumerator = l_navigationLinksGoingFromTheRequestedNode.GetEnumerator();
            while (l_navigationLinksGoingFromTheRequestedNodeEnumerator.MoveNext())
            {
                yield return l_navigationLinksGoingFromTheRequestedNodeEnumerator.Current.EndNode;
            }
        }

        /// <summary>
        /// Checks if the <paramref name="p_requestedNode1"/> and <paramref name="p_requestedNode2"/> have a direct <see cref="NavigationLink"/> connection.
        /// </summary>
        /// <returns></returns>
        public static bool areNavigationNodesNeighbors(NavigationGraph p_navigationGraph, NavigationNode p_requestedNode1, NavigationNode p_requestedNode2)
        {
            foreach (NavigationNode l_reachableNavigationNode in getReachableNeighborNavigationNodes(p_navigationGraph, p_requestedNode2, NavigationGraphFlag.SNAPSHOT))
            {
                if (l_reachableNavigationNode == p_requestedNode1)
                {
                    return true;
                }
            }
            return false;
        }


    }

    /// <summary>
    /// Store path traversal calculated data for path calculation.
    /// Associated to a <see cref="NavigationNode"/> by the path calculation algorithm, the <see cref="NavigationNodePathTraversalCalculations"/> stores two type of score :
    ///    - <see cref="PathScore"/> : This score represents the sum of <see cref="NavigationLink"/>s travel cost to reach the associated node.
    ///    - <see cref="HeuristicScore"/> : This score is entierly defined by the associated <see cref="NavigationNode"/>. It influences the choice of the next <see cref="NavigationNode"/> picked for calculation
    ///           by the path algorithm.The higher the heuristic score is, the likelier the path algorithm will take the next NavigationNode as the asscoiated node.
    ///
    /// Whatever the score, all values have the same scale. Thus 1.0f of score is 1.0f unit distance of the game engine.
    /// </summary>    
    public struct NavigationNodePathTraversalCalculations
    {
        public float PathScore;
        public float HeuristicScore;
        public bool IsAlreadyEvaluatedByTheAlgorithm;
        public NavigationNode CalculationMadeFrom;

        public static NavigationNodePathTraversalCalculations build()
        {
            NavigationNodePathTraversalCalculations l_instance = new NavigationNodePathTraversalCalculations();
            l_instance.PathScore = 0.0f;
            l_instance.HeuristicScore = 0.0f;
            l_instance.IsAlreadyEvaluatedByTheAlgorithm = false;
            l_instance.CalculationMadeFrom = null;
            return l_instance;
        }

        /** The total score is the sum of m_pathScore and m_heuristicScore. */
        public static float calculateTotalScore(ref NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations)
        {
            return p_navigationNodePathTraversalCalculations.PathScore + p_navigationNodePathTraversalCalculations.HeuristicScore;
        }
    };


    public static class NavigationLinkAlteration
    {
        /*
        Alter NavigationNodeLinks by removing the NavigationLinks following the rule dictated by p_navigationLinkAlterationMethod (see ENavigationLinkRemovalMethod for more).
    */
        public static void removeNavigationLinks(NavigationGraph p_navigationGraph, ENavigationLinkAlterationMethod p_navigationLinkAlterationMethod, NavigationNode p_involvedNode)
        {
            switch (p_navigationLinkAlterationMethod)
            {
                case ENavigationLinkAlterationMethod.TO:
                    // We have to :
                    //    * gets NavigationLinks going to the p_involvedNode
                    //    * clear NavigationLinks coming from NavigationNodes that goes to the p_involvedNode
                    //    * clear NavigationLinks going to the p_involvedNode
                    List<NavigationLink> l_nodesToList = p_navigationGraph.NodeLinksIndexedByEndNode[p_involvedNode];
                    for (int i = 0; i < l_nodesToList.Count; i++)
                    {
                        NavigationLink l_linkTo = l_nodesToList[i];
                        List<NavigationLink> l_targetList = p_navigationGraph.NodeLinksIndexedByStartNode[l_linkTo.StartNode];
                        l_targetList.Remove(l_linkTo);
                    }
                    p_navigationGraph.NodeLinksIndexedByEndNode[p_involvedNode].Clear();
                    break;
            }
        }

        /*
            Alter NavigationNodeLinks by restoring NavigationLinks following the rule dictated by p_navigationLinkAlterationMethod (see ENavigationLinkRemovalMethod for more).
            - NavigationNodeLinks are restored from the NavigationGraphSnapshot.
        */
        public static void restoreNavigationLinksFromSnapshot(NavigationGraph p_navigationGraph, ENavigationLinkAlterationMethod p_navigationLinkAlterationMethod, NavigationNode p_involvedNode)
        {
            switch (p_navigationLinkAlterationMethod)
            {
                case ENavigationLinkAlterationMethod.TO:

                    // We have to :
                    //    * copy NavigationLinks going to the p_involvedNode (retrieved from snapshot)
                    //    * add NavigationLinks coming from NavigationNodes that goes to the p_involvedNode
                    List<NavigationLink> l_linksThatPointsTowardsInvolvedNode = p_navigationGraph.NodeLinksIndexedByEndNode[p_involvedNode];
                    l_linksThatPointsTowardsInvolvedNode.Clear();
                    l_linksThatPointsTowardsInvolvedNode.AddRange(p_navigationGraph.NavigationGraphSnapshot.NodeLinksIndexedByEndNode[p_involvedNode]);

                    for (int i = 0; i < l_linksThatPointsTowardsInvolvedNode.Count; i++)
                    {
                        NavigationLink l_linkTo = l_linksThatPointsTowardsInvolvedNode[i];

                        var l_linksFrom_snapshot_enumerator = p_navigationGraph.NavigationGraphSnapshot.NodeLinksIndexedByStartNode[l_linkTo.StartNode].GetEnumerator();
                        while (l_linksFrom_snapshot_enumerator.MoveNext())
                        {
                            NavigationLink l_referenceFromLink = l_linksFrom_snapshot_enumerator.Current;

                            if (l_referenceFromLink.EndNode == p_involvedNode)
                            {
                                p_navigationGraph.NodeLinksIndexedByStartNode[l_referenceFromLink.StartNode].Add(l_referenceFromLink);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// This class must be combined with a NavigationNode to work. It describes how the NavigationLinks of the Navigation graph will be altered.
        /// </summary>
        public enum ENavigationLinkAlterationMethod : ushort
        {
            /// <summary>
            /// All NavigationLinks where the input NavigationNode is the EndNode will be involved.
            /// </summary>
            FROM = 0,
            /// <summary>
            /// All NavigationLinks where the input NavigationNode is the StartNode will be involved.
            /// </summary>
            TO = 1,
            /// <summary>
            /// All NavigationLinks where the input NavigationNode is the EndNode or StartNode will be involved.
            /// </summary>
            FROM_TO = 2
        };
    }
}
