using _Entity;
using _RuntimeObject;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace _Attack
{
    [Serializable]
    [CreateAssetMenu(fileName = "AttackDefinition", menuName = "EntityComponents/AttackDefinition")]
    public class AttackDefinition : EntityDefinitionSubObject
    {
        public AttackData AttackData;

        [AssetSelector]
        public IAttackBeforeDamageEventHook AttackBeforeDamageEventHook;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            Attack l_attack = Attack.alloc(ref AttackData, AttackBeforeDamageEventHook);
            EntityComponent.add_component<Attack>(p_entity, l_attack);
        }
    }
}