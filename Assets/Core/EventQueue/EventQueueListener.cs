using System.Collections;
using System.Collections.Generic;
using System;

namespace _EventQueue
{
    public class EventQueueListener 
    {
        public Dictionary<Type, List<IEventListener>> EventsListener;

        public static EventQueueListener alloc() {
            EventQueueListener l_instance = new EventQueueListener();
            l_instance.EventsListener = new Dictionary<Type, List<IEventListener>>(4);
            return l_instance;
        }

        public static void onEventExecuted(EventQueueListener p_eventQueueListener, EventQueue p_eventQueue, AEvent p_event)
        {
            Type l_eventType = p_event.GetType();
            if (p_eventQueueListener.EventsListener.ContainsKey(l_eventType))
            {
                List<IEventListener> l_eventListeners = p_eventQueueListener.EventsListener[l_eventType];
                for(int i = l_eventListeners.Count - 1; i >= 0; i--)
                {
                    l_eventListeners[i].I_OnEventExecuted(p_eventQueue, p_event);
                }
            }
        }

        public static void registerEvent(EventQueueListener p_eventQueueListener, IEventListener p_eventListener)
        {
            Type l_eventType = p_eventListener.get_eventType();
            if (!p_eventQueueListener.EventsListener.ContainsKey(l_eventType))
            {
                p_eventQueueListener.EventsListener.Add(l_eventType, new List<IEventListener>(4));
            }

            p_eventListener.buildUniqueKey();
            p_eventQueueListener.EventsListener[l_eventType].Add(p_eventListener);
        }

        public static void unRegisterEvent(EventQueueListener p_eventQueueListener, IEventListener p_eventListener)
        {
            Type l_eventType = p_eventListener.get_eventType();
            if (p_eventQueueListener.EventsListener.ContainsKey(l_eventType))
            {
                List<IEventListener> l_eventListeners = p_eventQueueListener.EventsListener[l_eventType];

                for (int i = 0; i < l_eventListeners.Count; i++)
                {
                    if (l_eventListeners[i].compareUniqueKey(p_eventListener))
                    {
                        l_eventListeners.RemoveAt(i);
                        break;
                    }
                }
            }
        }

    }

    public interface IEventListener
    {
        void buildUniqueKey();
        ref EventListenerKey get_uniqueKey();
        bool compareUniqueKey(IEventListener p_other);
        Type get_eventType();
        void I_OnEventExecuted(EventQueue p_eventQueue, AEvent p_event);
    }

    public abstract class AEventListener<EVENT> : IEventListener where EVENT : AEvent
    {
        public EventListenerKey EventListenerKey;
        public static int EventListenerIDCounter;
        public abstract void OnEventExecuted(EventQueue p_eventQueue, EVENT p_event);

        public void I_OnEventExecuted(EventQueue p_eventQueue, AEvent p_event)
        {
            OnEventExecuted(p_eventQueue, (EVENT)p_event);
        }

        public void buildUniqueKey()
        {
            EventListenerIDCounter += 1;
            EventListenerKey = new EventListenerKey() { EventListenerID = EventListenerIDCounter };
        }

        public Type get_eventType()
        {
            return typeof(EVENT);
        }

        public ref EventListenerKey get_uniqueKey()
        {
            return ref EventListenerKey;
        }

        public bool compareUniqueKey(IEventListener p_other)
        {
            return EventListenerKey.EventListenerID == p_other.get_uniqueKey().EventListenerID;
        }
    }

    public struct EventListenerKey
    {
        public int EventListenerID;
    }
}


