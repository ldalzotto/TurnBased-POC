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
            Locomotion l_locomotion = Locomotion.alloc();
            EntityComponent.add_component<Locomotion>(p_entity, ref l_locomotion);

            LocomotionSystemComponent l_locomotionSystemComponent = p_runtimeObjectRootComponent.GetInstanciatedComponentsGameObject().AddComponent<LocomotionSystemComponent>();
            l_locomotionSystemComponent.AssociatedEntity = p_entity;
            l_locomotionSystemComponent.TravelSpeed = TravelSpeed;
            l_locomotion.MoveToNavigationNode = l_locomotionSystemComponent.HeadTowardsNode;
            l_locomotion.WarpTo = l_locomotionSystemComponent.warp;
        }
    }
}

