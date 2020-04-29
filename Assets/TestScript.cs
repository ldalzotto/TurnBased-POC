using _ExecutionTree;
using _Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    void Start()
    {
        ExecutionTree l_executionTree = ExecutionTree.build();
        ExecutionTree.addNode(ref l_executionTree, ExecutionNode.build(new StartAction()));
        ExecutionTree.iterate(ref l_executionTree);
    }

    private void Update()
    {
        ExecutionTree l_executionTree = ExecutionTree.build();
        ExecutionTree.addNode(ref l_executionTree, ExecutionNode.build(new StartAction()));
        ExecutionTree.iterate(ref l_executionTree);
    }


    struct StartAction : IExecutionNodeAction
    {
        public void Execute(ref List<ExecutionNode> p_newExecutionNodes)
        {
            // Debug.Log("Hello from StartAction");
            p_newExecutionNodes.Add(ExecutionNode.build(IncrementAction.build(0, 10)));
        }
    }

    struct IncrementAction : IExecutionNodeAction
    {
        public int CurrentValue;
        public int MaxValue;

        public void Execute(ref List<ExecutionNode> p_newExecutionNodes)
        {
            CurrentValue += 1;
            // Debug.Log(CurrentValue);
            if (CurrentValue >= MaxValue)
            {
                p_newExecutionNodes.Add(ExecutionNode.build(new EndAction()));
            }
            else
            {
                if (CurrentValue == 5)
                {
                    p_newExecutionNodes.Add(ExecutionNode.build(new FiveReachedAction()));
                }

                p_newExecutionNodes.Add(ExecutionNode.build(IncrementAction.build(CurrentValue, MaxValue)));
            }
        }

        public static IncrementAction build(in int p_currentValue, in int p_maxValue)
        {
            IncrementAction l_instance = new IncrementAction();
            l_instance.CurrentValue = p_currentValue;
            l_instance.MaxValue = p_maxValue;
            return l_instance;
        }
    }

    struct FiveReachedAction : IExecutionNodeAction
    {
        public void Execute(ref List<ExecutionNode> p_newExecutionNodes)
        {
            //  Debug.Log("Hello from FiveReachedAction");
            // p_newExecutionNodes.Add(ExecutionNode.build(new EndAction()));
        }
    }

    struct EndAction : IExecutionNodeAction
    {
        public void Execute(ref List<ExecutionNode> p_newExecutionNodes)
        {
            //  Debug.Log("Hello from EndAction");
            // p_newExecutionNodes.Add(ExecutionNode.build(new EndAction()));
        }
    }

}