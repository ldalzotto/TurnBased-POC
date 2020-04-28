using System;
using System.Collections.Generic;

#if comment
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

    public static T getOrCreate(MemoryBufferStack<T> p_memoryBufferStack)
    {
        if (p_memoryBufferStack.Stack.Count == 0)
        {
            return p_memoryBufferStack.AllocationCallback.Invoke();
        }
        else
        {
            return p_memoryBufferStack.Stack.Pop();
        }
    }

}

#endif