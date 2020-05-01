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
        private TurnTimelineSequencerStartRequest TurnTimelineRequest;

        private void Awake()
        {
            TurnTimeline = TurnTimeline.alloc();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (TurnTimelineRequest == null)
                {
                    TurnTimelineRequest = TurnTimelineSequencer.beginSequencingTurn(new TurnTimelineSequencerStartRequest());
                }
                else if (TurnTimelineRequest.Ended)
                {
                    TurnTimelineRequest.Reset();
                    TurnTimelineRequest = TurnTimelineSequencer.beginSequencingTurn(TurnTimelineRequest);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (TurnTimelineRequest != null)
                {
                    TurnTimelineRequest.ExternallyAborted = true;
                }
            }
        }

        private void OnDestroy()
        {
            TurnTimeline.free(TurnTimeline);
        }


    }

}
