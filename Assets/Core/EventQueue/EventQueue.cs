using System;
using System.Collections.Generic;

namespace _EventQueue
{
    public class EventQueue
    {
        public static Action<AEvent> OnEventExecuted;

        public static EventQueue alloc()
        {
            EventQueue l_instance = new EventQueue();
            l_instance.Events = new List<AEvent>(8);
            return l_instance;
        }

        public List<AEvent> Events;

        public static void insertEventAt(EventQueue p_eventQueue, int p_insersionIndex, AEvent p_insertedEvent)
        {
            p_eventQueue.Events.Insert(p_insersionIndex, p_insertedEvent);
        }

        public static void enqueueEvent(EventQueue p_eventQueue, AEvent p_event)
        {
            p_eventQueue.Events.Add(p_event);
        }

        public static void insertBefore(EventQueue p_eventQueue, AEvent p_comparisonEvent, AEvent p_insertedEvent)
        {
            int l_index = p_eventQueue.Events.IndexOf(p_comparisonEvent);

            // If the event has been found in the EventQueue.
            if (l_index >= 0)
            {
                insertEventAt(p_eventQueue, l_index, p_insertedEvent);
            }
        }

        public static void insertAfter(EventQueue p_eventQueue, AEvent p_comparisonEvent, AEvent p_insertedEvent)
        {
            int l_index = p_eventQueue.Events.IndexOf(p_comparisonEvent);

            // If the event has been found in the EventQueue.
            if (l_index >= 0)
            {
                insertEventAt(p_eventQueue, l_index + 1, p_insertedEvent);
            }
        }

        public static IEnumerator<EVENT> get_allEvents<EVENT>(EventQueue p_eventQueue) where EVENT : AEvent
        {
            for (int i = 0; i < p_eventQueue.Events.Count; i++)
            {
                AEvent l_event = p_eventQueue.Events[i];
                if (l_event is EVENT)
                {
                    yield return l_event as EVENT;
                }
            }
        }

        public static void clearAll(EventQueue p_eventQueue)
        {
            p_eventQueue.Events.Clear();
        }

        public static void iterate(EventQueue p_eventQueue)
        {
            while (p_eventQueue.Events.Count > 0)
            {
                //Pop the first element
                AEvent l_firstEvent = p_eventQueue.Events[0];

                AAsyncEvent l_firstEventAsAsync = l_firstEvent as AAsyncEvent;
                if (l_firstEventAsAsync != null)
                {
                    if (!l_firstEventAsAsync.IsRunning && !l_firstEventAsAsync.IsCompleted())
                    {
                        l_firstEventAsAsync.IsRunning = true;
                        l_firstEventAsAsync.Execute(p_eventQueue);
                    }

                    if (l_firstEventAsAsync.IsRunning)
                    {
                        l_firstEventAsAsync.ExecuteEveryIteration();

                        if (l_firstEventAsAsync.IsCompleted())
                        {
                            p_eventQueue.Events.RemoveAt(0);
                            EventQueueListener.onEventExecuted(EventQueueListener.UniqueInstance, p_eventQueue, l_firstEvent);
                            l_firstEventAsAsync.IsRunning = false;
                            l_firstEventAsAsync.OnCompleted(p_eventQueue);
                            OnEventExecuted?.Invoke(l_firstEvent);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    p_eventQueue.Events.RemoveAt(0);
                    l_firstEvent.Execute(p_eventQueue);
                    EventQueueListener.onEventExecuted(EventQueueListener.UniqueInstance, p_eventQueue, l_firstEvent);
                    OnEventExecuted?.Invoke(l_firstEvent);
                }
            }
        }

    }

    public abstract class AEvent
    {
        public virtual void Execute(EventQueue p_eventQueue) { }
    }

    /// <summary>
    /// <see cref="AAsyncEvent"/> are <see cref="AEvent"/> that are removed from the <see cref="EventQueue"/> when the condition
    /// <see cref="IsCompleted()"/> returns true.
    /// </summary>
    public abstract class AAsyncEvent : AEvent
    {
        public bool IsRunning;


        public virtual void ExecuteEveryIteration() { }


        /// <summary>
        /// Used by the <see cref="EventQueue"/> to check if the <see cref="AAsyncEvent"/> is completed.
        /// When true, the <see cref="AAsyncEvent"/> is discarded by calling <see cref="OnCompleted"/>
        /// </summary>
        /// <returns></returns>
        public abstract bool IsCompleted();

        /// <summary>
        /// The last function called by the <see cref="EventQueue"/> before the <see cref="AAsyncEvent"/> is discarded.
        /// </summary>
        public virtual void OnCompleted(EventQueue p_eventQueue) { }

    }

    /// <summary>
    /// This interface is a convenient way to allow an <see cref="AEvent"/> to insert additional Events at some particular position provided by 
    /// external sources.
    /// <see cref="IEventHook"/> purpose is to be created as game assets and easily switchable.
    /// Exemple : Before applying damages. Some attacks may trigger animations, other will add particles, ...
    /// </summary>
    public interface IEventHook
    {
        void FeedEventQueue(EventQueue p_eventQueue, int p_insertionIndex);
    }
}
