using UnityEngine;
using System.Collections;
using System;
using _Entity;
using _RuntimeObject;

namespace _Attack
{
    [Serializable]
    [CreateAssetMenu(fileName = "AttackDefinition", menuName = "EntityComponents/AttackDefinition")]
    public class AttackDefinition : EntityDefinitionSubObject
    {
        public AttackData AttackData;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            Attack l_attack = Attack.alloc(ref AttackData);
            EntityComponent.add_component<Attack>(p_entity, l_attack);
        }
    }
}