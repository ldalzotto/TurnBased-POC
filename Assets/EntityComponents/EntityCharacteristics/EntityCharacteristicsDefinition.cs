using _Entity;
using _RuntimeObject;
using System;
using UnityEngine;

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
