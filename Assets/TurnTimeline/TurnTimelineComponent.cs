using UnityEngine;
using System.Collections;
using _Entity;

namespace _TurnTimeline
{
    public class TurnTimelineComponent : MonoBehaviour
    {
        public TurnTimeline TurnTimeline;

        private void Awake()
        {
            TurnTimeline = TurnTimeline.alloc();
            // Entity l_nextEntity = TurnTimelineAlgorithm.IncrementTimeline(TurnTimeline);
        }

        private void OnDestroy()
        {
            TurnTimeline.free(TurnTimeline);
        }
    }

}
