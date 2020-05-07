using _Entity;
using _RuntimeObject;
using System;
using UnityEngine;

namespace _HealthRecovery
{
    [Serializable]
    [CreateAssetMenu(fileName = "HealthRecoveryDefinition", menuName = "EntityComponents/HealthRecoveryDefinition")]
    public class HealthRecoveryDefinition : EntityDefinitionSubObject
    {
        public HealthRecoveryData HealthRecoveryData;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            EntityComponent.add_component<HealthRecovery>(p_entity, HealthRecovery.alloc(HealthRecoveryData));
        }
    }

}
