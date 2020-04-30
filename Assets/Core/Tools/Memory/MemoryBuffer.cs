using System;
using System.Collections.Generic;

namespace _Memory
{
    public interface IPoolable
    {
        void ClearForNewInstance();
    }

    public class MemoryBufferStack<T> where T : IPoolable
    {
        public Stack<T> Stack;
        public Func<T> AllocationCallback;

        private MemoryBufferStack() { }

        public static MemoryBufferStack<T> alloc(Func<T> p_allocationCallback)
        {
            MemoryBufferStack<T> l_instance = new MemoryBufferStack<T>();
            l_instance.Stack = new Stack<T>();
            l_instance.AllocationCallback = p_allocationCallback;
            return l_instance;
        }

        public T popOrCreate()
        {
            if (Stack.Count == 0)
            {
                return AllocationCallback.Invoke();
            }
            else
            {
                return Stack.Pop();
            }
        }

        public void push(T p_val)
        {
            p_val.ClearForNewInstance();
            Stack.Push(p_val);
        }

    }


    #region Specialization

    public class PoolableList<T> : List<T>, IPoolable
    {
        public void ClearForNewInstance()
        {
            Clear();
        }
    }

    public class PoolableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IPoolable
    {
        public void ClearForNewInstance()
        {
            Clear();
        }
    }

    #endregion
}
