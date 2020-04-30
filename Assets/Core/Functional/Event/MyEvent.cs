using System.Collections;
using System.Collections.Generic;
using System;

namespace _Functional
{
    public struct MyEvent<T1>
    {
        public RefDictionary<int, IEventCallback> Callbacks;
        public int HandleCounter;

        public static MyEvent<T1> build()
        {
            MyEvent<T1> l_instance = new MyEvent<T1>();
            l_instance.Callbacks = new RefDictionary<int, IEventCallback>();
            l_instance.HandleCounter = 1;
            return l_instance;
        }

        public static int register(ref MyEvent<T1> p_event, ref IEventCallback p_callback)
        {
            p_event.Callbacks[p_event.HandleCounter] = p_callback;
            p_event.HandleCounter += 1;
            return p_event.HandleCounter - 1;
        }

        public static void unRegister(ref MyEvent<T1> p_event, int p_handler)
        {
            p_event.Callbacks.Remove(p_handler);
        }

        public static void broadcast(ref MyEvent<T1> p_event, ref T1 p_param1)
        {
            for(int i = 0; i < p_event.Callbacks.Count; i++)
            {
                p_event.Callbacks.entries[i].value.Execute(ref p_param1);
            }
        }

        public interface IEventCallback
        {
            void Execute(ref T1 p_param1);
        }

    }
}

