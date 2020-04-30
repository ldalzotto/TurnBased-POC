using System.Collections;
using _Entity;
using System.Collections.Generic;
using _Functional;

namespace _TurnTimeline
{

    public class TurnTimelineElligibility : AEntityComponent
    {
        public bool IsElligibleForTimeline;

        public static TurnTimelineElligibility alloc(bool isElligibleForTimeline)
        {
            TurnTimelineElligibility l_instance = new TurnTimelineElligibility();
            l_instance.IsElligibleForTimeline = isElligibleForTimeline;
            return l_instance;
        }
    }
}

