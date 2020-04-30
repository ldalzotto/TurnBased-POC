using _Functional;
using _Locomotion;
using _Navigation;
using _RuntimeObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Entity
{
    public class EntityRegistrationComponent : RuntimeComponent
    {
        public EntityDefinition EntityDefinition;
        public Entity AssociatedEntity;

        public override void Awake()
        {
            base.Awake();
            initialize(this);
        }

        private void Start()
        {
            NavigationNode l_randomNavigationNode = NavigationGrpahAlgorithm.pickRandomNode(NavigationGraphComponentContainer.NavigationGraphComponent.NavigationGraph);
            Entity.set_currentNavigationNode(AssociatedEntity, l_randomNavigationNode);

            RuntimeObject.FindComponent<LocomotionSystemComponent>();

            LocomotionSystemComponent.warp(
                    RuntimeObject.FindComponent<LocomotionSystemComponent>(),
                    l_randomNavigationNode
                );
        }

        public static void initialize(EntityRegistrationComponent p_entityRegistrationComponent)
        {
            p_entityRegistrationComponent.AssociatedEntity = Entity.alloc();

            MyEvent<Entity>.IEventCallback l_onEntityDestroyed = OnEntityDestroyed.build(p_entityRegistrationComponent);
            MyEvent<Entity>.register(
                    ref p_entityRegistrationComponent.AssociatedEntity.OnEntityDestroyed,
                    ref l_onEntityDestroyed);

            EntityDefinition.Initialize(
                    ref p_entityRegistrationComponent.EntityDefinition,
                    ref p_entityRegistrationComponent.AssociatedEntity,
                    p_entityRegistrationComponent.RuntimeObject.RuntimeObjectRootComponent);

        }

        struct OnEntityDestroyed : MyEvent<Entity>.IEventCallback
        {
            public EntityRegistrationComponent EntityRegistrationComponent;

            public void Execute(ref Entity p_entity)
            {
                GameObject.Destroy(EntityRegistrationComponent.gameObject);
            }

            public static OnEntityDestroyed build(EntityRegistrationComponent p_entityRegistrationComponent)
            {
                OnEntityDestroyed l_instance = new OnEntityDestroyed();
                l_instance.EntityRegistrationComponent = p_entityRegistrationComponent;
                return l_instance;
            }
        }
    }

}
