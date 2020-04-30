using UnityEngine;
using System.Collections;
using _Entity;
using _Entity._Turn;
using System;

namespace _TurnTimeline
{
    public class TurnTimelineComponent : MonoBehaviour
    {
        public TurnTimeline TurnTimeline;

        private void Awake()
        {
            TurnTimeline = TurnTimeline.alloc();
        }

        private void OnDestroy()
        {
            TurnTimeline.free(TurnTimeline);
        }

        /*
        public static IEnumerator waitOneFrameBeforeStartingTheNextTurn(TurnTimelineComponent p_turnTiemlineComponent)
        {
            yield return null;
            startTurnTimeline(p_turnTiemlineComponent);
        }
        */

    }

}
