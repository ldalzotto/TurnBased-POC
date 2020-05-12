using _Entity;
using _RuntimeObject;
using System;
using UnityEngine;

namespace _HealthRecovery
{
    [Serializable]
    [CreateAssetMenu(fileName = "HealthRecoveryTriggerDefinition", menuName = "EntityComponents/HealthRecoveryTriggerDefinition")]
    public class HealthRecoveryTriggerDefinition : EntityDefinitionSubObject
    {
        public HealthRecoveryData HealthRecoveryData;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            EntityComponent.add_component<HealthRecoveryTrigger>(p_entity, HealthRecoveryTrigger.alloc(HealthRecoveryData));
        }
    }

}
