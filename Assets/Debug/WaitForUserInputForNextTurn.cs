using _Entity._Turn;
using _EventQueue;
using UnityEngine;

public class WaitForUserInputForNextTurn : MonoBehaviour
{
    private void Awake()
    {
       EventQueueListener.registerEvent(EventQueueListener.UniqueInstance, new OnStartTurnEvent());
    }

    public class OnStartTurnEvent : AEventListener<StartTurnEvent>
    {
        public override void OnEventExecuted(EventQueue p_eventQueue, StartTurnEvent p_event)
        {
            EventQueue.insertBefore(p_eventQueue, p_event.FutureEndTurnEvent, WaitForUserInput.alloc());
        }
    }

    public class WaitForUserInput : AAsyncEvent
    {
        private bool Pressed;

        private WaitForUserInput() { }

        public static WaitForUserInput alloc()
        {
            WaitForUserInput l_instance = new WaitForUserInput();
            l_instance.Pressed = false;
            return l_instance;
        }

        public override void ExecuteEveryIteration()
        {
            base.ExecuteEveryIteration();
            Pressed = Input.GetKeyDown(KeyCode.RightArrow);
        }

        public override bool IsCompleted()
        {
            return Pressed;
        }
    }
}
