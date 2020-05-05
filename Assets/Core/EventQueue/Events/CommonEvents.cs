using _Functional;
using _GameLoop;

namespace _EventQueue._Events
{
    public class WaitForNextFrame : AAsyncEvent
    {
        public bool Completed;

        public static WaitForNextFrame alloc()
        {
            WaitForNextFrame l_instance = new WaitForNextFrame();
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            MyEvent.register(ref ExternalHooks.OnTickStartEvent, OnNextFrame.alloc(this));
            Completed = false;
        }

        public override bool IsCompleted()
        {
            return Completed;
        }

        class OnNextFrame : MyEvent.IEventCallback
        {
            public WaitForNextFrame WaitForNextFrame;
            public int Handle { get; set; }

            public static OnNextFrame alloc(WaitForNextFrame p_waitForNextFrame)
            {
                OnNextFrame l_instance = new OnNextFrame();
                l_instance.WaitForNextFrame = p_waitForNextFrame;
                return l_instance;
            }

            public EventCallbackResponse Execute()
            {
                WaitForNextFrame.Completed = true;
                return EventCallbackResponse.REMOVE;
            }
        }

    }

    public class IterateAndWaitForEmptyQueue : AAsyncEvent
    {
        public EventQueue EventQueue;

        public static IterateAndWaitForEmptyQueue alloc(EventQueue p_awaitedEventQueue)
        {
            IterateAndWaitForEmptyQueue l_instance = new IterateAndWaitForEmptyQueue();
            l_instance.EventQueue = p_awaitedEventQueue;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
        }

        public override void ExecuteEveryIteration()
        {
            base.ExecuteEveryIteration();
            EventQueue.iterate(EventQueue);
        }

        public override bool IsCompleted()
        {
            return EventQueue.Events.Count == 0;
        }
    }
}

