using System.Collections;
using System.Collections.Generic;
using System;

namespace _Functional
{
    public enum EventCallbackResponse : ushort
    {
        OK = 0,
        REMOVE = 1
    }


    public struct MyEvent
    {
        public RefDictionary<int, IEventCallback> Callbacks;
        public List<int> RemoveQueue;
        public RefList<IEventCallback> AddedCallbackQueue;

        public int HandleCounter;
        public bool IsBroadcasting;

        public static MyEvent build()
        {
            MyEvent l_instance = new MyEvent();
            l_instance.Callbacks = new RefDictionary<int, IEventCallback>();
            l_instance.HandleCounter = 1;
            return l_instance;
        }

        public static int register(ref MyEvent p_event, ref IEventCallback p_callback)
        {
            p_callback.Handle = p_event.HandleCounter;
            p_event.HandleCounter += 1;

            if (p_event.IsBroadcasting)
            {
                if (p_event.AddedCallbackQueue == null) { p_event.AddedCallbackQueue = new RefList<IEventCallback>(); }
                p_event.AddedCallbackQueue.AddRef(ref p_callback);
            }
            else
            {
                p_event.Callbacks[p_callback.Handle] = p_callback;
            }

            return p_event.HandleCounter - 1;
        }

        public static void unRegister(ref MyEvent p_event, int p_handler)
        {
            if (p_event.IsBroadcasting)
            {
                if (p_event.RemoveQueue == null) { p_event.RemoveQueue = new List<int>(); }
                p_event.RemoveQueue.Add(p_handler);
            }
            else
            {
                p_event.Callbacks.Remove(p_handler);
            }
        }

        public static void broadcast(ref MyEvent p_event)
        {
            p_event.IsBroadcasting = true;
            for (int i = 0; i < p_event.Callbacks.Count; i++)
            {
                if (p_event.Callbacks.entries[i].value.Execute() == EventCallbackResponse.REMOVE)
                {
                    if (p_event.RemoveQueue == null) { p_event.RemoveQueue = new List<int>(); }
                    p_event.RemoveQueue.Add(p_event.Callbacks.entries[i].value.Handle);
                }
            }
            p_event.IsBroadcasting = false;

            if (p_event.RemoveQueue != null && p_event.RemoveQueue.Count > 0)
            {
                for (int i = 0; i < p_event.RemoveQueue.Count; i++)
                {
                    unRegister(ref p_event, p_event.RemoveQueue[i]);
                }
                p_event.RemoveQueue.Clear();
            }

            if (p_event.AddedCallbackQueue != null && p_event.AddedCallbackQueue.Count > 0)
            {
                for (int i = 0; i < p_event.AddedCallbackQueue.Count; i++)
                {
                    ref IEventCallback l_callback = ref p_event.AddedCallbackQueue.ValueRef(i);
                    p_event.Callbacks[l_callback.Handle] = l_callback;
                }
                p_event.AddedCallbackQueue.Clear();
            }

        }

        public interface IEventCallback
        {
            int Handle { get; set; }
            EventCallbackResponse Execute();
        }

    }

    public struct MyEvent<T1>
    {
        public RefDictionary<int, IEventCallback> Callbacks;
        public List<int> RemoveQueue;
        public RefList<IEventCallback> AddedCallbackQueue;

        public int HandleCounter;
        public bool IsBroadcasting;

        public static MyEvent<T1> build()
        {
            MyEvent<T1> l_instance = new MyEvent<T1>();
            l_instance.Callbacks = new RefDictionary<int, IEventCallback>();
            l_instance.HandleCounter = 1;
            return l_instance;
        }

        public static int register(ref MyEvent<T1> p_event, ref IEventCallback p_callback)
        {
            p_callback.Handle = p_event.HandleCounter;
            p_event.HandleCounter += 1;

            if (p_event.IsBroadcasting)
            {
                if (p_event.AddedCallbackQueue == null) { p_event.AddedCallbackQueue = new RefList<IEventCallback>(); }
                p_event.AddedCallbackQueue.AddRef(ref p_callback);
            }
            else
            {
                p_event.Callbacks[p_callback.Handle] = p_callback;
            }

            return p_event.HandleCounter - 1;
        }

        public static void unRegister(ref MyEvent<T1> p_event, int p_handler)
        {
            if (p_event.IsBroadcasting)
            {
                if (p_event.RemoveQueue == null) { p_event.RemoveQueue = new List<int>(); }
                p_event.RemoveQueue.Add(p_handler);
            }
            else
            {
                p_event.Callbacks.Remove(p_handler);
            }
        }

        public static void broadcast(ref MyEvent<T1> p_event, ref T1 p_param1)
        {
            p_event.IsBroadcasting = true;
            for (int i = 0; i < p_event.Callbacks.Count; i++)
            {
                if (p_event.Callbacks.entries[i].value.Execute(ref p_param1) == EventCallbackResponse.REMOVE)
                {
                    if (p_event.RemoveQueue == null) { p_event.RemoveQueue = new List<int>(); }
                    p_event.RemoveQueue.Add(p_event.Callbacks.entries[i].value.Handle);
                }
            }
            p_event.IsBroadcasting = false;

            if (p_event.RemoveQueue != null && p_event.RemoveQueue.Count > 0)
            {
                for (int i = 0; i < p_event.RemoveQueue.Count; i++)
                {
                    unRegister(ref p_event, p_event.RemoveQueue[i]);
                }
                p_event.RemoveQueue.Clear();
            }

            if (p_event.AddedCallbackQueue != null && p_event.AddedCallbackQueue.Count > 0)
            {
                for (int i = 0; i < p_event.AddedCallbackQueue.Count; i++)
                {
                    ref IEventCallback l_callback = ref p_event.AddedCallbackQueue.ValueRef(i);
                    p_event.Callbacks[l_callback.Handle] = l_callback;
                }
                p_event.AddedCallbackQueue.Clear();
            }

        }

        public interface IEventCallback
        {
            int Handle { get; set; }
            EventCallbackResponse Execute(ref T1 p_param1);
        }

    }




}

