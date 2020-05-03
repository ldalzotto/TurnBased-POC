using _Functional;
using _GameLoop;
using System.Collections;

namespace _EventQueue._Events
{
    public class WaitForNextFrame : AAsyncEvent
    {
        public static WaitForNextFrame alloc()
        {
            WaitForNextFrame l_instance = new WaitForNextFrame();
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            MyEvent.register(ref ExternalHooks.OnTickStartEvent, OnNextFrame.alloc(this));
            Start();
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
                WaitForNextFrame.Complete();
                return EventCallbackResponse.REMOVE;
            }
        }

    }
}

