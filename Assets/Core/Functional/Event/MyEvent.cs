using System.Collections;
using System.Collections.Generic;
using System;

namespace _Functional
{
    public struct MyEvent<T1>
    {
        public Dictionary<int, IEventCallback> Callbacks;
        public int HandleCounter;

        public static MyEvent<T1> build()
        {
            MyEvent<T1> l_instance = new MyEvent<T1>();
            l_instance.Callbacks = new Dictionary<int, IEventCallback>();
            l_instance.HandleCounter = 1;
            return l_instance;
        }

        public static int register(ref MyEvent<T1> p_event, ref IEventCallback p_callback)
        {
            p_event.Callbacks[p_event.HandleCounter] = p_callback;
            p_event.HandleCounter += 1;
            return p_event.HandleCounter - 1;
        }

        public static void broadcast(ref MyEvent<T1> p_event, ref T1 p_param1)
        {
            foreach (var callback in p_event.Callbacks)
            {
                callback.Value.Execute(ref p_param1);
            }
        }

        public interface IEventCallback
        {
            void Execute(ref T1 p_param1);
        }

    }
}

