using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _Navigation
{
    public static class NavigationGraphContainer
    {
        public static NavigationGraph UniqueNavigationGraph;
    }

    public class NavigationGraph
    {
        public List<NavigationNode> NavigationNodes;
        public Dictionary<NavigationNode, List<NavigationLink>> NodeLinksIndexedByStartNode;
        public Dictionary<NavigationNode, List<NavigationLink>> NodeLinksIndexedByEndNode;
        public NavigationGraphSnapshot NavigationGraphSnapshot;

        private NavigationGraph() { }

        public static NavigationGraph alloc()
        {
            NavigationGraph l_instanciatedNavigationGraph = new NavigationGraph();
            l_instanciatedNavigationGraph.NavigationNodes = new List<NavigationNode>();
            l_instanciatedNavigationGraph.NodeLinksIndexedByStartNode = new Dictionary<NavigationNode, List<NavigationLink>>();
            l_instanciatedNavigationGraph.NodeLinksIndexedByEndNode = new Dictionary<NavigationNode, List<NavigationLink>>();
            NavigationGraphContainer.UniqueNavigationGraph = l_instanciatedNavigationGraph;
            return l_instanciatedNavigationGraph;
        }

        public static void free(NavigationGraph p_navigationGraph)
        {
            if (NavigationGraphContainer.UniqueNavigationGraph == p_navigationGraph)
            {
                NavigationGraphContainer.UniqueNavigationGraph = null;
            }
        }

        public static NavigationNode instanciateAndAddNode(NavigationGraph p_navigationGraph)
        {
            NavigationNode l_navigationNode = NavigationNode.alloc();
            p_navigationGraph.NavigationNodes.Add(l_navigationNode);
            return l_navigationNode;
        }

        public static void createLinkBetween(NavigationGraph p_navigationGraph, NavigationNode p_startNode, NavigationNode p_endNode, float p_travelCost)
        {
            NavigationLink l_instanciatedNavigationLink = NavigationLink.alloc(p_startNode, p_endNode, p_travelCost);
            if (!p_navigationGraph.NodeLinksIndexedByStartNode.ContainsKey(p_startNode))
            {
                p_navigationGraph.NodeLinksIndexedByStartNode.Add(p_startNode, new List<NavigationLink>());
            }
            if (!p_navigationGraph.NodeLinksIndexedByEndNode.ContainsKey(p_endNode))
            {
                p_navigationGraph.NodeLinksIndexedByEndNode.Add(p_endNode, new List<NavigationLink>());
            }

            p_navigationGraph.NodeLinksIndexedByStartNode[p_startNode].Add(l_instanciatedNavigationLink);
            p_navigationGraph.NodeLinksIndexedByEndNode[p_endNode].Add(l_instanciatedNavigationLink);
        }

        public static void takeSnapshot(NavigationGraph p_navigationGraph)
        {
            p_navigationGraph.NavigationGraphSnapshot = NavigationGraphSnapshot.alloc(
                    new Dictionary<NavigationNode, List<NavigationLink>>(p_navigationGraph.NodeLinksIndexedByStartNode),
                    new Dictionary<NavigationNode, List<NavigationLink>>(p_navigationGraph.NodeLinksIndexedByEndNode)
            );
        }

        public static Dictionary<NavigationNode, List<NavigationLink>> get_nodeLinksIndexedByStartNode(NavigationGraph p_navigationGraph, NavigationGraphFlag p_navigationGraphFlag)
        {
            if (p_navigationGraphFlag == NavigationGraphFlag.CURRENT)
            {
                return p_navigationGraph.NodeLinksIndexedByStartNode;
            }
            else if (p_navigationGraphFlag == NavigationGraphFlag.SNAPSHOT)
            {
                return p_navigationGraph.NavigationGraphSnapshot.NodeLinksIndexedByStartNode;
            }
            return null;
        }
    }

    public class NavigationGraphSnapshot
    {
        public Dictionary<NavigationNode, List<NavigationLink>> NodeLinksIndexedByStartNode;
        public Dictionary<NavigationNode, List<NavigationLink>> NodeLinksIndexedByEndNode;

        private NavigationGraphSnapshot() { }

        public static NavigationGraphSnapshot alloc(
                Dictionary<NavigationNode, List<NavigationLink>> p_nodeLinksIndexedByStartNode,
                Dictionary<NavigationNode, List<NavigationLink>> p_nodeLinksIndexedByEndNode
            )
        {
            NavigationGraphSnapshot l_instance = new NavigationGraphSnapshot();
            l_instance.NodeLinksIndexedByStartNode = p_nodeLinksIndexedByStartNode;
            l_instance.NodeLinksIndexedByEndNode = p_nodeLinksIndexedByEndNode;
            return l_instance;
        }
    }

    public class NavigationNode
    {
        public float3 LocalPosition;

        private NavigationNode() { }

        public static NavigationNode alloc()
        {
            return new NavigationNode();
        }
    }

    public class NavigationLink
    {
        public float TravelCost;
        public NavigationNode StartNode;
        public NavigationNode EndNode;

        private NavigationLink() { }

        public static NavigationLink alloc(NavigationNode p_startNode, NavigationNode p_endNode, float p_travelCost)
        {
            NavigationLink l_instance = new NavigationLink();
            l_instance.StartNode = p_startNode;
            l_instance.EndNode = p_endNode;
            l_instance.TravelCost = p_travelCost;
            return l_instance;
        }
    }

    /// <summary>
    /// This flag is used when requesting the <see cref="NavigationGraph"/> to sepcify if the request is done in the current <see cref="NavigationGraph"/> (the graph that is modfied at rutime)
    /// or on the snapshot.
    /// </summary>
    public enum NavigationGraphFlag : ushort
    {
        CURRENT = 0,
        SNAPSHOT = 1
    };
}
