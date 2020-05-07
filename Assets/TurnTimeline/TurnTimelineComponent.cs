using _Entity._Turn;
using _EventQueue;
using UnityEngine;

namespace _TurnTimeline
{
    public class TurnTimelineComponent : MonoBehaviour
    {
        public TurnTimeline TurnTimeline;

        private void Awake()
        {
            TurnTimeline = TurnTimeline.alloc();
        }

        private bool HasStarted = false;

        private void Update()
        {
            if (!HasStarted)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    EventQueue.enqueueEvent(EventQueueContainer.TurnTimelineQueue, StartTurnEvent.alloc(TurnTimeline, EventQueueContainer.EntityActionQueue));
                    HasStarted = true;
                }
            }

        }

        private void OnDestroy()
        {
            TurnTimeline.free(TurnTimeline);
        }
    }

}
