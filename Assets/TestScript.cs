using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _Entity._Turn;
using _ExecutionTree;
using _Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    private void Update()
    {
        DecisionTree l_decisionTree = DecisionTree.alloc();
        TestNode l_testNode1 = new TestNode();
        TestNode l_testNode2 = new TestNode();
        TestNode l_testNode3 = new TestNode();
        DecisionTree.linkDecisionNodes(l_decisionTree, l_decisionTree.RootNode, l_testNode1);
        DecisionTree.linkDecisionNodes(l_decisionTree, l_testNode1, l_testNode2);
        DecisionTree.linkDecisionNodes(l_decisionTree, l_testNode1, l_testNode3);
        Algorithm.traverseDecisionTree(l_decisionTree);
    }

    public interface SObject { }

    public struct MyVec3 : SObject
    {
        public float X;
        public float Y;
        public float Z;
    }

    class TestNode : ADecisionNode
    {

        public override void TreeTraversal(ADecisionNode p_sourceNode)
        {
            switch (p_sourceNode)
            {
                case TestNode l_testNode:
                    break;
            }
        }
    }

}