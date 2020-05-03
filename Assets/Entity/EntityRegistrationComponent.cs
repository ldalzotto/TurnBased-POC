using _Entity._Turn;
using _EventQueue;
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
    public static class EntityRegistrationComponentContainer
    {
        public static Dictionary<Entity, EntityRegistrationComponent> EntityRegistrationComponentByEntities;
        
        static EntityRegistrationComponentContainer()
        {
            EntityRegistrationComponentByEntities = new Dictionary<Entity, EntityRegistrationComponent>();
        }
        
    }

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
            NavigationNode l_randomNavigationNode = NavigationGraphAlgorithm.pickRandomNode(NavigationGraphComponentContainer.UniqueNavigationGraphComponent.NavigationGraph);
            EventQueue.insertEventAt(EventQueue.UniqueInstance, 0, _Entity._Events.NavigationNodeWarpEntityEvent.alloc(AssociatedEntity, l_randomNavigationNode));
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

        public override void OnDestroy()
        {
            base.OnDestroy();

            if(AssociatedEntity != null && !AssociatedEntity.MarkedForDestruction)
            {
                Entity.destroyEntity(AssociatedEntity);
            }
        }

        struct OnEntityDestroyed : MyEvent<Entity>.IEventCallback
        {
            public int Handle { get; set; }
            public EntityRegistrationComponent EntityRegistrationComponent;

            public EventCallbackResponse Execute(ref Entity p_param1)
            {
                GameObject.Destroy(EntityRegistrationComponent.RuntimeObject.RuntimeObjectRootComponent.gameObject);
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
