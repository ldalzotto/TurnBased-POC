using UnityEngine;
using System.Collections;
using _Entity;
using System;
using _RuntimeObject;

namespace _TurnTimeline
{
    [Serializable]
    [CreateAssetMenu(fileName = "TurnTimelineEllibilityDefinition", menuName = "EntityComponents/TurnTimelineEllibilityDefinition")]
    public class TurnTimelineEllibilityDefinition : EntityDefinitionSubObject
    {
        public TurnTimelineElligibilityData TurnTimelineElligibilityData;

        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            TurnTimelineElligibility l_turnTimelineElligiblity = TurnTimelineElligibility.alloc(ref TurnTimelineElligibilityData);
            EntityComponent.add_component<TurnTimelineElligibility>(p_entity, l_turnTimelineElligiblity);
        }
    }

}
