using _Memory;
using System;
using System.Collections.Generic;
using Unity.Mathematics;


namespace _Navigation
{
    public struct PathCalculationParameters
    {
        public float HeurisitcDistanceWeight;

        public static PathCalculationParameters build(float p_heurisitcDistanceWeight)
        {
            return new PathCalculationParameters()
            {
                HeurisitcDistanceWeight = p_heurisitcDistanceWeight
            };
        }
    }

    public struct NavigationPath : IDisposable
    {
        public PoolableList<NavigationNode> NavigationNodes;
        public PoolableDictionary<NavigationNode, NavigationNodePathTraversalCalculations> NavigationNodesTraversalCalculations;
        public float PathCost;

        public static NavigationPath build()
        {
            NavigationPath l_instance = new NavigationPath();
            l_instance.NavigationNodes = NavigationGraphAlgorithmMemoryBuffer.NavigationPath_NavigationNodes.popOrCreate();
            l_instance.NavigationNodesTraversalCalculations = NavigationGraphAlgorithmMemoryBuffer.NavigationPath_NavigationNodesTraversalCalculations.popOrCreate();
            l_instance.PathCost = 0.0f;
            return l_instance;
        }

        public void Dispose()
        {
            NavigationGraphAlgorithmMemoryBuffer.NavigationPath_NavigationNodes.push(NavigationNodes);
            NavigationGraphAlgorithmMemoryBuffer.NavigationPath_NavigationNodesTraversalCalculations.push(NavigationNodesTraversalCalculations);
        }
    }

    public static class NavigationGrpahAlgorithm
    {
        /*
                    Path calculation is a recursive algorithm that try all possibilities of NavigationLinks from the p_beginNode to reach the p_endNode.
                    These possibilities are ordered by a score (see NavigationNodePathTraversalCalculations for details) that represents the "difficulty" to reach the destination.
                    The combinaison of NavigationLinks that leads to the lower score is the resulting path.

                    * p_heurisitcDistanceWeight : This factor controls the weight of the heuristic score in to total score calculation of NavigationNodePathTraversalCalculations.
                                                  If > 1, NavigationNodes with higher heurisitc score are likely to be picked for calculation even if ovther NavigationNode's path score is higher.
        */
        public static NavigationPath CalculatePath(
                    NavigationGraph p_navigationGraph,
                    NavigationNode p_beginNode,
                    NavigationNode p_endNode,
                    in PathCalculationParameters p_pathCalculationParameters)
        {
            if (p_beginNode != null && p_endNode != null)
            {

                NavigationPath l_resultPath = NavigationPath.build();

                // This list are all nodes available to be picked for the next current node.
                // This means that they all haven't been traversed by the algorithm but not calculated yet
                PoolableList<NavigationNode> l_pathNodesElligibleForNextCurrent = NavigationGraphAlgorithmMemoryBuffer.CurrentNavigationNodes.popOrCreate();

                NavigationNode l_currentEvaluatedNode = p_beginNode;
                bool l_isFirstTimeInLoop = true;


                while (l_currentEvaluatedNode != null && l_currentEvaluatedNode != p_endNode)
                {

                    // If this is not the start, we find the next current node to pick
                    if (!l_isFirstTimeInLoop)
                    {
                        l_currentEvaluatedNode = pickNextCurrentNodeToCalculate(l_resultPath.NavigationNodesTraversalCalculations, l_pathNodesElligibleForNextCurrent);
                    }
                    else
                    {
                        l_isFirstTimeInLoop = false;
                        l_resultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode] = NavigationNodePathTraversalCalculations.build();
                        l_pathNodesElligibleForNextCurrent.Add(l_currentEvaluatedNode);
                    }

                    if (l_currentEvaluatedNode != null)
                    {
                        // For the current node, we evaluate the score of it's neighbors

                        // The current evaluated node is no more ellisible because it has just been traversed by the algorithm
                        l_pathNodesElligibleForNextCurrent.Remove(l_currentEvaluatedNode);

                        // If the current evaluated node has links
                        if (p_navigationGraph.NodeLinksIndexedByStartNode.ContainsKey(l_currentEvaluatedNode))
                        {
                            List<NavigationLink> l_evaluatedNavigationLinks = p_navigationGraph.NodeLinksIndexedByStartNode[l_currentEvaluatedNode];
                            for (int i = 0; i < l_evaluatedNavigationLinks.Count; i++)
                            {
                                NavigationLink l_link = l_evaluatedNavigationLinks[i];

                                // We calculate the score as if the linked node is traversed.
                                float l_calculatedPathScore = simulateNavigationLinkTraversal(l_resultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode], l_link);

                                // If the neighbor has not already been calculated
                                if (!l_resultPath.NavigationNodesTraversalCalculations.ContainsKey(l_link.EndNode))
                                {
                                    NavigationNode l_linkEndNode = l_link.EndNode;

                                    NavigationNodePathTraversalCalculations l_linkNodeCalculations = NavigationNodePathTraversalCalculations.build();
                                    updatePathScore(ref l_linkNodeCalculations, l_calculatedPathScore, l_currentEvaluatedNode);
                                    calculateHeuristicScore(ref l_linkNodeCalculations, l_linkEndNode, p_endNode, p_pathCalculationParameters.HeurisitcDistanceWeight);
                                    l_resultPath.NavigationNodesTraversalCalculations[l_linkEndNode] = l_linkNodeCalculations;

                                    l_pathNodesElligibleForNextCurrent.Add(l_linkEndNode);
                                }
                                // Else, we update score calculations only if the current calculated score is lower than the previous one.
                                // This means we have found a less costly path.
                                else
                                {
                                    NavigationNode l_linkEndNode = l_link.EndNode;

                                    NavigationNodePathTraversalCalculations l_linkNodeCalculations = l_resultPath.NavigationNodesTraversalCalculations[l_linkEndNode];
                                    updatePathScore(ref l_linkNodeCalculations, l_calculatedPathScore, l_currentEvaluatedNode);
                                    l_resultPath.NavigationNodesTraversalCalculations[l_linkEndNode] = l_linkNodeCalculations;
                                }
                            }
                        }
                    }
                }


                // calculating final path by going to the start by taking "CalculationMadeFrom" nodes
                if (l_currentEvaluatedNode != null)
                {
                    l_resultPath.NavigationNodes.Add(l_currentEvaluatedNode);
                    l_resultPath.PathCost = 0.0f;

                    if (l_resultPath.NavigationNodesTraversalCalculations.ContainsKey(l_currentEvaluatedNode))
                    {
                        l_resultPath.PathCost = NavigationNodePathTraversalCalculations.calculateTotalScore(l_resultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode]);
                    }


                    while (l_currentEvaluatedNode != p_beginNode)
                    {
                        NavigationNode l_parent = l_resultPath.NavigationNodesTraversalCalculations[l_currentEvaluatedNode].CalculationMadeFrom;
                        if (l_parent != null)
                        {
                            l_resultPath.NavigationNodes.Add(l_parent);
                            l_currentEvaluatedNode = l_parent;
                        }
                        else
                        {
                            l_currentEvaluatedNode = p_beginNode;
                        }
                    }

                    l_resultPath.NavigationNodes.Reverse();
                }

                NavigationGraphAlgorithmMemoryBuffer.CurrentNavigationNodes.push(l_pathNodesElligibleForNextCurrent);

                return l_resultPath;
            }
            else
            {
                return new NavigationPath();
            }

        }

        /**
        The picked NavigationNode with the lowest total score (retrieved from NavigationNodePathTraversalCalculations) is returned
        */
        private static NavigationNode pickNextCurrentNodeToCalculate(
                 in Dictionary<NavigationNode, NavigationNodePathTraversalCalculations> l_pathScoreCalculations,
                 in List<NavigationNode> l_pathNodesElligibleForNextCurrent)
        {
            NavigationNode l_currentSelectedNode = null;
            float l_currentTotalScore = 0.0f;

            for (int i = 0; i < l_pathNodesElligibleForNextCurrent.Count; i++)
            {
                NavigationNode l_currentComparedNavigationnode = l_pathNodesElligibleForNextCurrent[i];

                if (l_currentSelectedNode == null)
                {
                    l_currentSelectedNode = l_currentComparedNavigationnode;
                    l_currentTotalScore = NavigationNodePathTraversalCalculations.calculateTotalScore(l_pathScoreCalculations[l_currentComparedNavigationnode]);
                }
                else
                {
                    float l_currentComparedTotalScore = NavigationNodePathTraversalCalculations.calculateTotalScore(l_pathScoreCalculations[l_currentComparedNavigationnode]);
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
        private static float simulateNavigationLinkTraversal(in NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations, in NavigationLink p_navigationLink)
        {
            return p_navigationNodePathTraversalCalculations.PathScore + p_navigationLink.TravelCost;
        }

        /** 
            When the caller wants to update the m_pathScore with an already calculated Path score (p_newPathScore) adn from which NavigationNode this calculation has been done (p_calculatedFrom_ptr).
            p_newPathScore andp_calculatedFrom_ptr is keeped only if the p_newPathScore is lower than the current one.
        */
        private static void updatePathScore(
                ref NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations,
                in float p_newPathScore,
                in NavigationNode p_calculatedFrom)
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
           in NavigationNode p_startNode,
           in NavigationNode p_endNode,
           in float p_heurisitcDistanceMultiplier)
        {
            p_navigationNodePathTraversalCalculations.HeuristicScore =
                math.distance(p_startNode.LocalPosition, p_endNode.LocalPosition) * p_heurisitcDistanceMultiplier;
        }



        public static NavigationNode pickRandomNode(in NavigationGraph p_navigationGraph)
        {
            return p_navigationGraph.NavigationNodes[MyRandom.Random.NextInt(0, p_navigationGraph.NavigationNodes.Count)];
        }

    }


    /**
        Store path traversal calculated data for path calculation.
        Associated to a NavigationNode by the path calculation algorithm, the NavigationNodePathTraversalCalculations stores two type of score :
            - Path score : This score represents the sum of @ref NavigationLink "NavigationLinks" travel cost to reach the associated node.
            - Heuristic socre : This score is entierly defined by the associated NavigationNode. It influences the choice of the next NavigationNode picked for calculation
                by the path algorithm. The higher the heuristic score is, the likelier the path algorithm will take the next NavigationNode as the asscoiated node.

        Whatever the score, all values have the same scale. Thus 1.0f of score is 1.0f unit distance of the game engine.
*/
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
        public static float calculateTotalScore(in NavigationNodePathTraversalCalculations p_navigationNodePathTraversalCalculations)
        {
            return p_navigationNodePathTraversalCalculations.PathScore + p_navigationNodePathTraversalCalculations.HeuristicScore;
        }
    };
    
    public static class NavigationGraphAlgorithmMemoryBuffer
    {
        public static MemoryBufferStack<PoolableList<NavigationNode>> CurrentNavigationNodes = MemoryBufferStack<PoolableList<NavigationNode>>.alloc(() => { return new PoolableList<NavigationNode>(); });
        public static MemoryBufferStack<PoolableList<NavigationNode>> NavigationPath_NavigationNodes = MemoryBufferStack<PoolableList<NavigationNode>>.alloc(() => { return new PoolableList<NavigationNode>(); });
        public static MemoryBufferStack<PoolableDictionary<NavigationNode, NavigationNodePathTraversalCalculations>> NavigationPath_NavigationNodesTraversalCalculations = MemoryBufferStack<PoolableDictionary<NavigationNode, NavigationNodePathTraversalCalculations>>.alloc(() => { return new PoolableDictionary<NavigationNode, NavigationNodePathTraversalCalculations>(); });
    }


}
