using System.Collections;
using _Entity;
using System.Collections.Generic;

namespace _TurnTimeline
{
    public class TurnTimelineAlgorithm
    {

        /// <summary>
        /// Calculates the next <see cref="Entity"/> to play from the <see cref="TurnTimeline"/>.
        /// The next <see cref="Entity"/> to play is the one that have the lowest <see cref="EntityTurnTimelineCalculationData.CurrentTimelinePosition"/>.
        /// When this <see cref="Entity"/> has been found, all <see cref="EntityTurnTimelineCalculationData"/> advances in the timeline by the distance from 0 of the founded <see cref="Entity"/>.<see cref="EntityTurnTimelineCalculationData.CurrentTimelinePosition"/>.
        /// Then the founded <see cref="Entity"/> is pushed back to the timeline.
        /// </summary>
        /// <param name="p_turnTimeline"></param>
        /// <returns></returns>
        public static Entity IncrementTimeline(TurnTimeline p_turnTimeline)
        {
            Entity l_lowerTimelineScore = null;
            float l_currentComparedScore = 0.0f;

            RefDictionary<Entity, EntityTurnTimelineCalculationData> l_entityTurnTimelineDataLookup = p_turnTimeline.TimelineOrderingDatas;

            for(int i = 0; i < l_entityTurnTimelineDataLookup.Count; i++)
            {
                var l_timelineOrderingDataEntry_ref = l_entityTurnTimelineDataLookup.GetEntryRef(i);
                if (l_lowerTimelineScore == null)
                {
                    l_lowerTimelineScore = l_timelineOrderingDataEntry_ref.key;
                    l_currentComparedScore = l_timelineOrderingDataEntry_ref.value.CurrentTimelinePosition;
                }
                else if (l_timelineOrderingDataEntry_ref.value.CurrentTimelinePosition < l_currentComparedScore)
                {
                    l_lowerTimelineScore = l_timelineOrderingDataEntry_ref.key;
                    l_currentComparedScore = l_timelineOrderingDataEntry_ref.value.CurrentTimelinePosition;
                }
            }

            if (l_lowerTimelineScore != null)
            {
                float l_removedTimelineScore = l_currentComparedScore;

                // All entities advances in the timeline
                for(int i = 0; i < l_entityTurnTimelineDataLookup.Count; i++)
                {
                    l_entityTurnTimelineDataLookup.entries[i].value.CurrentTimelinePosition += (-1 * l_removedTimelineScore);
                }

                // The next Entity is pushed back on the Timeline
                l_entityTurnTimelineDataLookup.ValueRef(l_lowerTimelineScore).CurrentTimelinePosition = EntityTurnTimelineCalculationData.calculateReferenceTurnTimelineScore(l_lowerTimelineScore);
            }

            return l_lowerTimelineScore;
        }
    }

}
