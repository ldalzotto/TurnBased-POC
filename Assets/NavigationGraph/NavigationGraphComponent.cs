﻿using _NavigationEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace _NavigationGraph
{
    /// <summary>
    /// The entry point for <see cref="_NavigationGraph.NavigationGraph"/> operations.
    /// Instanciate the <see cref="_NavigationGraph.NavigationGraph"/> object.
    /// </summary>
    public class NavigationGraphComponent : MonoBehaviour
    {

        public NavigationGraphAsset NavigationGraphAsset;

        [FoldoutGroup("Algorithm parameters")]
        public PathCalculationParameters NavigationGraphAlgorithmParameters = PathCalculationParameters.build(2.0f);

        /// <summary>
        /// Prefab instanciated for every <see cref="NavigationNode"/> instanciated.
        /// </summary>
        [FoldoutGroup("NavigationNode definition")]
        [AssetsOnly]
        public GameObject NavigationNodeComponentPrefab;

        public NavigationGraph NavigationGraph;
        public Dictionary<NavigationNode, GameObject> NavigationNodeGameRepresentation = new Dictionary<NavigationNode, GameObject>();

        public NavigationEngine NavigationEngine;

        private void Awake()
        {
            NavigationGraph = NavigationGraphAsset.InstanciateNavigationGraph();
            NavigationGraphComponentContainer.UniqueNavigationGraphComponent = this;

            for (int i = 0; i < NavigationGraph.NavigationNodes.Count; i++)
            {
                NavigationNodeGameRepresentation[NavigationGraph.NavigationNodes[i]] = NavigationNodeComponent.instanciateFromPrefab(transform, NavigationGraph.NavigationNodes[i], NavigationNodeComponentPrefab);
            }

            NavigationEngine = NavigationEngine.alloc();
        }

        private void OnDestroy()
        {
            if (this == NavigationGraphComponentContainer.UniqueNavigationGraphComponent)
            {
                NavigationGraphComponentContainer.UniqueNavigationGraphComponent = null;
            }
            NavigationGraph.free(NavigationGraph);
            NavigationEngine.free(NavigationEngine);
        }

        /// <summary>
        /// Because <see cref="NavigationNode.LocalPosition"/> is in <see cref="NavigationGraph"/> local space, all positions must be converted to world space from this component.
        /// As the <see cref="NavigationGraphComponent"/> is the holder of the <see cref="m_navigationGraph"/> it is his role.
        /// </summary>
        /// <returns> <paramref name="p_navigationNode"/> position in world space.</returns>
        public static float3 get_WorldPositionFromNavigationNode(NavigationGraphComponent p_navigationGraphComponent, NavigationNode p_navigationNode)
        {
            return p_navigationGraphComponent.transform.localToWorldMatrix.MultiplyPoint(p_navigationNode.LocalPosition);
        }

    }

    public static class NavigationGraphComponentContainer
    {
        public static NavigationGraphComponent UniqueNavigationGraphComponent;
    }

}

