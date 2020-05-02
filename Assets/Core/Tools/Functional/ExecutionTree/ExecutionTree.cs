
using System;
using System.Collections.Generic;

namespace _ExecutionTree
{
    public struct ExecutionTree
    {
        public List<IExecutionNode> ExecutionNodes;

        public static ExecutionTree build()
        {
            ExecutionTree l_instance = new ExecutionTree();
            l_instance.ExecutionNodes = new List<IExecutionNode>();
            return l_instance;
        }

        #region Tree creation/alteration

        public static void addNode(ExecutionTree p_executiontree, IExecutionNode p_executionNode)
        {
            p_executiontree.ExecutionNodes.Add(p_executionNode);
        }

        #endregion

        public static void iterate(ExecutionTree p_executionTree)
        {
            while (p_executionTree.ExecutionNodes.Count > 0)
            {
                List<IExecutionNode> l_newExecutionNodes = new List<IExecutionNode>();
                
                for (int i = 0; i < p_executionTree.ExecutionNodes.Count; i++)
                {
                    p_executionTree.ExecutionNodes[i].Execute(l_newExecutionNodes);
                }

                p_executionTree.ExecutionNodes.Clear();
                p_executionTree.ExecutionNodes.AddRange(l_newExecutionNodes);
            }
        }
    }
    
    public interface IExecutionNode
    {
        void Execute(List<IExecutionNode> p_newExecutionNodes);
    }


}
