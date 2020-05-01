using _Entity._Turn;
using _ExecutionTree;
using _Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    private void Update()
    {
        using (NavigationGraphAlgorithm.CalculatePathRequest l_calculatePathRequest = NavigationGraphAlgorithm.CalculatePathRequest.CalculatePathRequestPool.popOrCreate())
        {
            NavigationGraphAlgorithm.CalculatePathRequest.prepareForCalculation(
              l_calculatePathRequest,
              NavigationGraphContainer.UniqueNavigationGraph,
              NavigationGraphAlgorithm.pickRandomNode(NavigationGraphContainer.UniqueNavigationGraph),
              NavigationGraphAlgorithm.pickRandomNode(NavigationGraphContainer.UniqueNavigationGraph),
              PathCalculationParameters.build(2.0f)
          );

            NavigationGraphAlgorithm.CalculatePath(l_calculatePathRequest);
        }
    }
}