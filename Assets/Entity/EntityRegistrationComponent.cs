using _Entity._Events;
using _EventQueue;
using _Functional;
using _NavigationGraph;
using _RuntimeObject;
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

        public static void initialize(EntityRegistrationComponent p_entityRegistrationComponent)
        {
            EventQueue.enqueueEvent(EventQueueContainer.TurnTimelineQueue, EntityCreateEvent.alloc(p_entityRegistrationComponent.onEntityCreated));
        }

        private void onEntityCreated(Entity p_entity)
        {
            AssociatedEntity = Entity.alloc();
            MyEvent<Entity>.IEventCallback l_onEntityDestroyed = OnEntityDestroyed.build(this);
            MyEvent<Entity>.register(
                    ref this.AssociatedEntity.OnEntityDestroyed,
                    ref l_onEntityDestroyed);

            EntityDefinition.Initialize(
                    ref this.EntityDefinition,
                    ref this.AssociatedEntity,
                    this.RuntimeObject.RuntimeObjectRootComponent);

            NavigationNode l_randomNavigationNode = NavigationGraphAlgorithm.pickRandomNode(NavigationGraphComponentContainer.UniqueNavigationGraphComponent.NavigationGraph);
            EventQueue.enqueueEvent(EventQueueContainer.TurnTimelineQueue, NavigationNodeWarpEntityEvent.alloc(AssociatedEntity, l_randomNavigationNode));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (AssociatedEntity != null && !AssociatedEntity.MarkedForDestruction)
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
