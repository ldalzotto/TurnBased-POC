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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EventQueue.enqueueEvent(EventQueueContainer.TurnTimelineQueue, StartTurnEvent.alloc(TurnTimeline));
            }
        }

        private void OnDestroy()
        {
            TurnTimeline.free(TurnTimeline);
        }
    }

}
