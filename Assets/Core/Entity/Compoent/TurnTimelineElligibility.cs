using _Entity;

namespace _TurnTimeline
{

    public class TurnTimelineElligibility : AEntityComponent
    {
        public TurnTimelineElligibilityData TurnTimelineElligibilityData;

        public static TurnTimelineElligibility alloc(ref TurnTimelineElligibilityData p_turnTimelineElligibilityData)
        {
            TurnTimelineElligibility l_instance = new TurnTimelineElligibility();
            l_instance.TurnTimelineElligibilityData = p_turnTimelineElligibilityData;
            return l_instance;
        }
    }

    public struct TurnTimelineElligibilityData
    {
        public bool IsElligibleForTimeline;

        public static TurnTimelineElligibilityData build(bool p_isElligibleForTimeline)
        {
            TurnTimelineElligibilityData l_instance = new TurnTimelineElligibilityData();
            l_instance.IsElligibleForTimeline = p_isElligibleForTimeline;
            return l_instance;
        }
    }
}

