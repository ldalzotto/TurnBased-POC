using UnityEngine;
using System.Collections;
using _Entity;
using System;
using _RuntimeObject;

namespace _EntityCharacteristics
{
    [Serializable]
    [CreateAssetMenu(fileName = "EntityCharacteristicsDefinition", menuName = "EntityComponents/EntityCharacteristicsDefinition")]
    public class EntityCharacteristicsDefinition : EntityDefinitionSubObject
    {
        public EntityCharacteristicsData EntityCharacteristicsData;

        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            EntityCharacteristics l_entityCharacterstics = EntityCharacteristics.alloc(ref EntityCharacteristicsData);
            EntityComponent.add_component<EntityCharacteristics>(p_entity, l_entityCharacterstics);
        }
    }

}
