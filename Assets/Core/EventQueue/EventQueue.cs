using System;
using System.Collections;
using System.Collections.Generic;

namespace _EventQueue
{
    public class EventQueue
    {
        public static EventQueue UniqueInstance;
        static EventQueue()
        {
            EventQueue.UniqueInstance = EventQueue.alloc();
        }

       // public static Action<AEvent> OnEventExecuted;

        public static EventQueue alloc()
        {
            EventQueue l_instance = new EventQueue();
            l_instance.Events = new List<AEvent>(16);
            l_instance.EventQueueListener = EventQueueListener.alloc();
            return l_instance;
        }

        public List<AEvent> Events;
        public EventQueueListener EventQueueListener;

        public static void insertEventAt(EventQueue p_eventQueue, int p_insersionIndex, AEvent p_event)
        {
            p_eventQueue.Events.Insert(p_insersionIndex, p_event);
        }
                
        public static void iterate(EventQueue p_eventQueue)
        {
            while(p_eventQueue.Events.Count > 0)
            {
                //Pop the first element
                AEvent l_firstEvent = p_eventQueue.Events[0];
                p_eventQueue.Events.RemoveAt(0);
                l_firstEvent.Execute(p_eventQueue);

                EventQueueListener.onEventExecuted(p_eventQueue, l_firstEvent);
                // OnEventExecuted?.Invoke(l_firstEvent);
            }
        }

    }

    public abstract class AEvent
    {
        public virtual void Execute(EventQueue p_eventQueue) { }
    }

}
