using UnityEngine;
using System.Collections;
using System;
using _Entity;
using _RuntimeObject;

namespace _Locomotion
{
    [Serializable]
    [CreateAssetMenu(fileName = "EntityLocomotionDefinition", menuName = "EntityComponents/EntityLocomotionDefinition")]
    public class EntityLocomotionDefinition : EntityDefinitionSubObject
    {
        public float TravelSpeed;

        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            LocomotionSystemComponent l_locomotionSystemComponent = p_runtimeObjectRootComponent.GetInstanciatedComponentsGameObject().AddComponent<LocomotionSystemComponent>();
            l_locomotionSystemComponent.TravelSpeed = TravelSpeed;
        }
    }
}

