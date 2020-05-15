using UnityEngine;
using System.Collections;
using _Entity;
using System;
using _RuntimeObject;
using Sirenix.OdinInspector;

namespace _AI._Behavior
{
    [Serializable]
    [CreateAssetMenu(fileName = "AIBehaviorDefinition", menuName = "EntityComponents/AIBehaviorDefinition")]
    public class AIBehaviorDefinition : EntityDefinitionSubObject
    {
        [AssetSelector]
        public IAIBehaviorProvider IAIBehaviorProvider;

        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            AIBehavior l_aiBehavior = AIBehavior.alloc(IAIBehaviorProvider);
            EntityComponent.add_component<AIBehavior>(p_entity, l_aiBehavior);
        }
    }
}

