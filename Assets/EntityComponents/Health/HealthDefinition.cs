using UnityEngine;
using System.Collections;
using System;
using _Entity;
using _RuntimeObject;

namespace _Health
{
    [Serializable]
    [CreateAssetMenu(fileName = "HealthDefinition", menuName = "EntityComponents/HealthDefinition")]
    public class HealthDefinition : EntityDefinitionSubObject
    {
        public HealthData HealthData;
        public HealthGUIComponent HealthGUIComponentPrefab;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            Health l_health = Health.alloc(ref HealthData);
            Health.resetHealth(l_health);
            EntityComponent.add_component<Health>(p_entity, l_health); 
            p_runtimeObjectRootComponent.GetComponentInChildren<EntityGaugeContainerComponent>().InstanciateHealthGUI(HealthGUIComponentPrefab);
        }
    }

}
