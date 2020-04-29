using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using _RuntimeObject;
using System;
using System.Collections.Generic;

namespace _Entity
{
    [Serializable]
    [CreateAssetMenu(fileName = "EntityDefinition", menuName = "Entity/EntityDefinition")]
    public class EntityDefinition : SerializedScriptableObject
    {
        [InlineEditor]
        public List<EntityDefinitionSubObject> EntityDefinitionSubObjects;

        public static void Initialize(
                ref EntityDefinition p_entityDefinition,
                ref Entity p_entity,
                RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            for (int i = 0; i < p_entityDefinition.EntityDefinitionSubObjects.Count; i++)
            {
                p_entityDefinition.EntityDefinitionSubObjects[i].Initialize(p_entity, p_runtimeObjectRootComponent);
            }
        }
    }

    [Serializable]
    public abstract class EntityDefinitionSubObject : SerializedScriptableObject
    {
        public abstract void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent);
    }
}

