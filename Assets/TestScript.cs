using _Navigation;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public NavigationGraphAsset NavigationGraphAsset;

    NavigationGraph l_navigationGraph;

    void Start()
    {
        l_navigationGraph = NavigationGraphAsset.InstanciateNavigationGraph(out NavigationNode[] p_nodes);
    }

    private void Update()
    {
        NavigationPath p_calcualtedPath = NavigationGrpahAlgorithm.CalculatePath(l_navigationGraph, l_navigationGraph.NavigationNodes[0],
            l_navigationGraph.NavigationNodes[l_navigationGraph.NavigationNodes.Count - 1], new PathCalculationParameters() { HeurisitcDistanceWeight = 2.0f });
    }
}
