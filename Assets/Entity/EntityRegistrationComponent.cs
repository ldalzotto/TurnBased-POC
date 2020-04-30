using _Entity._Turn;
using _Functional;
using _Locomotion;
using _Navigation;
using _RuntimeObject;
using System;
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




            TurnGlobalEvents.OnEntityTurnStartEvent.Add(p_entityRegistrationComponent.AssociatedEntity, MyEvent.build());
            TurnGlobalEvents.OnEntityTurnEndEvent.Add(p_entityRegistrationComponent.AssociatedEntity, MyEvent.build());

            MyEvent.IEventCallback l_callback = new Test() { Entity = p_entityRegistrationComponent.AssociatedEntity };
            MyEvent.register(ref TurnGlobalEvents.OnEntityTurnStartEvent.ValueRef(p_entityRegistrationComponent.AssociatedEntity), ref l_callback);
            

        }

        struct Test : MyEvent.IEventCallback
        {
            public Entity Entity;
            public int Handle { get ; set ; }

            public EventCallbackResponse Execute()
            {
             //   Debug.Log("Start turn : " + Entity.ToString());
                return EventCallbackResponse.OK;
            }
        }

        struct OnEntityDestroyed : MyEvent<Entity>.IEventCallback
        {
            public int Handle { get; set; }
            public EntityRegistrationComponent EntityRegistrationComponent;

            public EventCallbackResponse Execute(ref Entity p_param1)
            {
                GameObject.Destroy(EntityRegistrationComponent.gameObject);
                return EventCallbackResponse.OK;
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
