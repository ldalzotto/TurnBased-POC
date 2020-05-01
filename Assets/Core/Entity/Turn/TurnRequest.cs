using System.Collections;
using System;

namespace _Entity._Turn
{
    /// <summary>
    /// Structure passed when asking to start a new turn <see cref="TurnTimelineSequencer.startNewTurn(_TurnTimeline.TurnTimeline, ITurnRequest)"/>.
    /// </summary>
    public interface ITurnRequest
    {
        /// <summary>
        /// The <see cref="Entity"/> asked for a new turn.
        /// </summary>
        Entity Entity { get; set; }
        TurnTimelineSequencerStartRequest TurnTimelineSequencerStartRequest { get; set; }

        /// <summary>
        /// Called when <see cref="Entity"/> turn has started.
        /// </summary>
        void StartTurn(Action<ITurnRequest, TurnResponse, TurnTimelineSequencerStartRequest> p_onTurnEnded);
    }

    /// <summary>
    /// Flag returned when <see cref="ITurnRequest"/> have been consumed.
    /// </summary>
    public enum TurnResponse : ushort
    {
        OK = 0,

        /// <summary>
        /// Execution of <see cref="ITurnRequest"/> have resulted to a request to end the current level.
        /// </summary>
        LEVEL_ENDED = 1
    }

}