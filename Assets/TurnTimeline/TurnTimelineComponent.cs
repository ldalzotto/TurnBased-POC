using UnityEngine;
using System.Collections;
using _Entity;
using _Entity._Turn;
using System;
using _EventQueue;

namespace _TurnTimeline
{
    public class TurnTimelineComponent : MonoBehaviour
    {
        public TurnTimeline TurnTimeline;

        private OnEntityTurnEndEventListener OnEntityTurnEndEventListener;

        private void Awake()
        {
            TurnTimeline = TurnTimeline.alloc();
            OnEntityTurnEndEventListener = OnEntityTurnEndEventListener.alloc(TurnTimeline);
            EventQueueListener.registerEvent(ref EventQueue.UniqueInstance.EventQueueListener, OnEntityTurnEndEventListener);
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Return))
            {
                EventQueue.insertEventAt(EventQueue.UniqueInstance, 0, StartTurnEvent.alloc(TurnTimeline));
            }
        }

        private void OnDestroy()
        {
            TurnTimeline.free(TurnTimeline);
            EventQueueListener.unRegisterEvent(ref EventQueue.UniqueInstance.EventQueueListener, OnEntityTurnEndEventListener);
        }


    }

}
