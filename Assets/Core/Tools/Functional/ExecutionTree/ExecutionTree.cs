
using System;
using System.Collections.Generic;

namespace _ExecutionTree
{
    public struct ExecutionTree
    {
        public List<ExecutionNode> ExecutionNodes;

        public static ExecutionTree build()
        {
            ExecutionTree l_instance = new ExecutionTree();
            l_instance.ExecutionNodes = new List<ExecutionNode>();
            return l_instance;
        }

        #region Tree creation/alteration

        public static void addNode(ExecutionTree p_executiontree, ExecutionNode p_executionNode)
        {
            p_executiontree.ExecutionNodes.Add(p_executionNode);
        }

        #endregion

        public static void iterate(ExecutionTree p_executionTree)
        {
            while (p_executionTree.ExecutionNodes.Count > 0)
            {
                List<ExecutionNode> l_newExecutionNodes = new List<ExecutionNode>();
                
                for (int i = 0; i < p_executionTree.ExecutionNodes.Count; i++)
                {
                    p_executionTree.ExecutionNodes[i].ExecutionNodeAction.Execute(l_newExecutionNodes);
                }

                p_executionTree.ExecutionNodes.Clear();
                p_executionTree.ExecutionNodes.AddRange(l_newExecutionNodes);
            }
        }
    }

    public struct ExecutionNode
    {
        public IExecutionNodeAction ExecutionNodeAction;
        public static ExecutionNode build(IExecutionNodeAction p_executionNodeAction)
        {
            ExecutionNode l_instance = new ExecutionNode();
            l_instance.ExecutionNodeAction = p_executionNodeAction;
            return l_instance;
        }
    }

    public interface IExecutionNodeAction
    {
        void Execute(List<ExecutionNode> p_newExecutionNodes);
    }


}
