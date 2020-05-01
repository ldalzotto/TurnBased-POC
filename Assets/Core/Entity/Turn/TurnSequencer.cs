using System.Collections;
using _TurnTimeline;
using _Functional;
using _GameLoop;
using System;
using _Entity._Action;
using _ActionPoint;
using _Navigation;

namespace _Entity._Turn
{

    public static class TurnTimelineSequencer
    {
        public static TurnTimelineSequencerStartRequest beginSequencingTurn(TurnTimelineSequencerStartRequest p_request)
        {
            startNewTurn(new DebugTurnRequest(), p_request);
            return p_request;
        }

        private static void startNewTurn(ITurnRequest p_entityTurnAction, TurnTimelineSequencerStartRequest p_turnTimelineSequencerStartRequest)
        {
            Entity l_nextTurnEntity = TurnTimelineAlgorithm.IncrementTimeline(TurnTimelineContainer.UniqueTurnTimeline);

            if (l_nextTurnEntity != null)
            {
                ActionPoint.resetActionPoints(EntityComponent.get_component<ActionPoint>(l_nextTurnEntity));

                if (TurnGlobalEvents.OnEntityTurnStartEvent.ContainsKey(l_nextTurnEntity))
                {
                    MyEvent.broadcast(ref TurnGlobalEvents.OnEntityTurnStartEvent.ValueRef(l_nextTurnEntity));
                }

                p_entityTurnAction.Entity = l_nextTurnEntity;
                p_entityTurnAction.TurnTimelineSequencerStartRequest = p_turnTimelineSequencerStartRequest;
                p_entityTurnAction.StartTurn(endTurn);
            }

        }

        private static void endTurn(ITurnRequest p_entityTurnAction, TurnResponse p_turnResponse, TurnTimelineSequencerStartRequest p_turnTimelineSequencerStartRequest)
        {
            Entity l_turnEndedEntity = p_entityTurnAction.Entity;

            if (TurnGlobalEvents.OnEntityTurnEndEvent.ContainsKey(l_turnEndedEntity))
            {
                MyEvent.broadcast(ref TurnGlobalEvents.OnEntityTurnEndEvent.ValueRef(l_turnEndedEntity));
            }

            if (p_turnTimelineSequencerStartRequest.ExternallyAborted)
            {
                p_turnTimelineSequencerStartRequest.OnTurnTimelineSequencerEnded(TurnTimelineSequencerStartResponse.LEVEL_ENDED);
            }
            else
            {
                switch (p_turnResponse)
                {
                    case TurnResponse.OK:
                        MyEvent.IEventCallback l_startNextEntityTurn = StartNextEntityTurn.build(p_turnTimelineSequencerStartRequest);
                        MyEvent.register(ref ExternalHooks.OnTickStartEvent, ref l_startNextEntityTurn);
                        break;
                    case TurnResponse.LEVEL_ENDED:
                        p_turnTimelineSequencerStartRequest.OnTurnTimelineSequencerEnded(TurnTimelineSequencerStartResponse.LEVEL_ENDED);
                        break;
                }
            }
        }

        struct StartNextEntityTurn : MyEvent.IEventCallback
        {
            public int Handle { get; set; }
            public TurnTimelineSequencerStartRequest TurnTimelineSequencerStartRequest;

            public EventCallbackResponse Execute()
            {
                TurnTimelineSequencer.startNewTurn(new DebugTurnRequest(), TurnTimelineSequencerStartRequest);
                return EventCallbackResponse.REMOVE;
            }

            public static StartNextEntityTurn build(TurnTimelineSequencerStartRequest p_turnTimelineSequencerStartRequest)
            {
                StartNextEntityTurn l_instance = new StartNextEntityTurn();
                l_instance.TurnTimelineSequencerStartRequest = p_turnTimelineSequencerStartRequest;
                return l_instance;
            }
        }

    }

    #region Turn Timeline Sequencer Request

    public class TurnTimelineSequencerStartRequest
    {
        public bool Ended = false;
        public bool ExternallyAborted = false;
        public void OnTurnTimelineSequencerEnded(TurnTimelineSequencerStartResponse p_turnTimelineSequencerStartResponse)
        {
            switch (p_turnTimelineSequencerStartResponse)
            {
                case TurnTimelineSequencerStartResponse.LEVEL_ENDED:
                    ExternalHooks.LogDebug("Turn timeline aborted");
                    break;
            }
            Ended = true;
        }

        public void Reset()
        {
            Ended = false;
            ExternallyAborted = false;
        }
    }

    public enum TurnTimelineSequencerStartResponse : ushort
    {
        NO_MORE_ENTITIES = 0,
        LEVEL_ENDED = 1
    }

    #endregion

    #region EntityTurn request
    public struct DebugTurnRequest : ITurnRequest
    {
        public Entity Entity { get; set; }
        public TurnTimelineSequencerStartRequest TurnTimelineSequencerStartRequest { get; set; }

        public void StartTurn(Action<ITurnRequest, TurnResponse, TurnTimelineSequencerStartRequest> p_onTurnEnded)
        {
            ITurnRequest thiz = this;
            EntityActionStack.UniqueInstance.EnqueueEntityAction(new NavigationNodeMoveEntityAction(Entity, NavigationGraphAlgorithm.pickRandomNode(NavigationGraphContainer.UniqueNavigationGraph)));
            EntityActionStack.UniqueInstance.EnqueueEntityAction(HealthReductionTestAction.build(Entity));
            EntityActionStack.UniqueInstance.ExecuteAllEntityActions((EntityActionStackConsumeResponse p_response) =>
            {
                switch (p_response)
                {
                    case EntityActionStackConsumeResponse.OK:
                        p_onTurnEnded.Invoke(thiz, TurnResponse.OK, thiz.TurnTimelineSequencerStartRequest);
                        break;
                    case EntityActionStackConsumeResponse.LEVEL_ENDED:
                        p_onTurnEnded.Invoke(thiz, TurnResponse.LEVEL_ENDED, thiz.TurnTimelineSequencerStartRequest);
                        break;
                }
            });
        }
    }

    #endregion
}