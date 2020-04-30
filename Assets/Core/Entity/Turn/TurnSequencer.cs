using System.Collections;
using _TurnTimeline;
using _Functional;
using _GameLoop;
using System;

namespace _Entity._Turn
{
    public static class TurnTimelineSequencer
    {
        public static void beginSequencingTurn()
        {
            startNewTurn(new DebugTurnRequest());
        }

        private static void startNewTurn(ITurnRequest p_entityTurnAction)
        {
            Entity l_nextTurnEntity = TurnTimelineAlgorithm.IncrementTimeline(TurnTimelineContainer.UniqueTurnTimeline);

            if (TurnGlobalEvents.OnEntityTurnStartEvent.ContainsKey(l_nextTurnEntity))
            {
                MyEvent.broadcast(ref TurnGlobalEvents.OnEntityTurnStartEvent.ValueRef(l_nextTurnEntity));
            }

            p_entityTurnAction.Entity = l_nextTurnEntity;
            p_entityTurnAction.StartTurn(endTurn);
        }

        private static void endTurn(ITurnRequest p_entityTurnAction, TurnResponse p_turnResponse)
        {
            Entity l_turnEndedEntity = p_entityTurnAction.Entity;

            if (TurnGlobalEvents.OnEntityTurnEndEvent.ContainsKey(l_turnEndedEntity))
            {
                MyEvent.broadcast(ref TurnGlobalEvents.OnEntityTurnEndEvent.ValueRef(l_turnEndedEntity));
            }

            switch (p_turnResponse)
            {
                case TurnResponse.OK:
                    MyEvent.IEventCallback l_startNextEntityTurn = new StartNextEntityTurn();
                    MyEvent.register(ref GameLoopEvents.OnTickStart, ref l_startNextEntityTurn); 
                    break;
                case TurnResponse.LEVEL_ENDED:
                    break;
            }
        }

        struct StartNextEntityTurn : MyEvent.IEventCallback
        {
            public int Handle { get ; set ; }

            public EventCallbackResponse Execute()
            {
                TurnTimelineSequencer.startNewTurn(new DebugTurnRequest());
                return EventCallbackResponse.REMOVE;
            }
        }

    }


    public struct DebugTurnRequest : ITurnRequest
    {
        public Entity Entity { get; set; }

        public void StartTurn(Action<ITurnRequest, TurnResponse> p_onTurnEnded)
        {
            ITurnRequest thiz = this;
            p_onTurnEnded.Invoke(thiz, TurnResponse.OK);
        }
    }
}