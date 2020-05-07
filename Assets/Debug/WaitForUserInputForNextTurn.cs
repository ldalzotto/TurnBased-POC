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
            EventQueue.insertBefore(p_eventQueue, p_event.FutureEndTurnEvent, new WaitForUserInput());
        }
    }

    public class WaitForUserInput : AAsyncEvent
    {
        public override bool IsCompleted()
        {
            return Input.GetKeyDown(KeyCode.RightArrow);
        }
    }
}
