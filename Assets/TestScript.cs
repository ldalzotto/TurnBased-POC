using _Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public NavigationGraphAsset NavigationGraphAsset;

    NavigationGraph l_navigationGraph;

    void Start()
    {
        l_navigationGraph = NavigationGraphAsset.InstanciateNavigationGraph();
    }

    private void Update()
    {
        NavigationNode p_startNode = NavigationGrpahAlgorithm.pickRandomNode(l_navigationGraph);
        NavigationNode p_endNode = NavigationGrpahAlgorithm.pickRandomNode(l_navigationGraph);
        using (NavigationPath p_calcualtedPath = NavigationGrpahAlgorithm.CalculatePath(
               l_navigationGraph,
               p_startNode,
               p_endNode,
               new PathCalculationParameters() { HeurisitcDistanceWeight = 2.0f }))
        {

        }
    }

}
