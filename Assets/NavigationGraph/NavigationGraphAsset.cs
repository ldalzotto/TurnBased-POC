using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace _Navigation
{
    /// <summary>
    /// The NavigationGraphAsset stores all NavigationNodes and NavigationLinks in a serialized form.
    /// Content of this class will be instanciated at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "NavigationGraphAsset", menuName = "ScriptableObjects/NavigationGraph/NavigationGraphAsset", order = 1)]
    public class NavigationGraphAsset : SerializedScriptableObject
    {
        [SerializeField]
        public Dictionary<int, SerializedNavigationNode> NavigationNodes;

        [SerializeField]
        public List<SerializedNavigationLink> NavigationLinks;

        /// <summary>
        /// Intanciate a new <see cref="NavigationGraph"/> object from this asset.
        /// </summary>
        /// <param name="out_instanciatedNodes"> All <see cref="NavigationNode"/> instanciated.</param>
        /// <returns></returns>
        public NavigationGraph InstanciateNavigationGraph(out NavigationNode[] out_instanciatedNodes)
        {
            NavigationGraph l_navigationGraph = NavigationGraph.alloc();
            Dictionary<int, NavigationNode> l_serializedKeyToNavigationNode = new Dictionary<int, NavigationNode>(NavigationNodes.Count);
            out_instanciatedNodes = InstanciateNavigationNodes(l_navigationGraph, l_serializedKeyToNavigationNode);
            InstanciateNavigationLinks(l_navigationGraph, l_serializedKeyToNavigationNode);
            NavigationGraph.takeSnapshot(l_navigationGraph);
            return l_navigationGraph;
        }

        private NavigationNode[] InstanciateNavigationNodes(NavigationGraph l_navigationGraph, in Dictionary<int, NavigationNode> l_serializedKeyToNavigationNode)
        {
            NavigationNode[] out_instanciatedNodes = new NavigationNode[NavigationNodes.Count];
            int l_instanciatedNodesCounter = 0;
            foreach (var l_navigationNodeEntry in NavigationNodes)
            {
                NavigationNode l_instanciatedNavigationNode = NavigationGraph.instanciateAndAddNode(l_navigationGraph);
                l_instanciatedNavigationNode.LocalPosition = l_navigationNodeEntry.Value.LocalPosition;
                l_serializedKeyToNavigationNode[l_navigationNodeEntry.Key] = l_instanciatedNavigationNode;
                out_instanciatedNodes[l_instanciatedNodesCounter] = l_instanciatedNavigationNode;
                l_instanciatedNodesCounter += 1;
            }

            return out_instanciatedNodes;
        }

        private void InstanciateNavigationLinks(NavigationGraph l_navigationGraph, in Dictionary<int, NavigationNode> l_serializedKeyToNavigationNode)
        {
            foreach (var l_navigationLinkEntry in NavigationLinks)
            {
                NavigationGraph.createLinkBetween(l_navigationGraph, l_serializedKeyToNavigationNode[l_navigationLinkEntry.StartNode], l_serializedKeyToNavigationNode[l_navigationLinkEntry.EndNode], l_navigationLinkEntry.TravelCost);
            }
        }


    }

    [Serializable]
    public struct SerializedNavigationNode
    {
        [SerializeField]
        public Vector3 LocalPosition;
    }

    [Serializable]
    public struct SerializedNavigationLink
    {
        public static SerializedNavigationLink New(int p_startNode, int p_endNode)
        {
            SerializedNavigationLink thiz = new SerializedNavigationLink();
            thiz.StartNode = p_startNode;
            thiz.EndNode = p_endNode;
            return thiz;
        }

        [HorizontalGroup("G2")]
        public int StartNode;
        [HorizontalGroup("G2")]
        public int EndNode;

        /// <summary>
        /// The cost that indicates the difficulty to traverse the navigation link.
        /// </summary>
        public float TravelCost;
    }
}

