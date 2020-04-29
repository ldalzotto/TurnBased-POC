using _Functional;
using _Navigation;
using _RuntimeObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Entity
{
    public class EntityRegistrationComponent : RuntimeComponent
    {
        public Entity AssociatedEntity;

        public override void Awake()
        {
            base.Awake();
            initialize(this);
        }

        private void Start()
        {
            NavigationNode l_randomNavigationNode = NavigationGrpahAlgorithm.pickRandomNode(NavigationGraphComponentContainer.NavigationGraphComponent.NavigationGraph);
            Entity.set_currentNavigationNode(ref AssociatedEntity, l_randomNavigationNode);
            /*
            NavigationGraphComponent.get_WorldPositionFromNavigationNode(
                NavigationGraphComponentContainer.NavigationGraphComponent,
                l_randomNavigationNode
            );
            */
        }

        public static void initialize(in EntityRegistrationComponent p_entityRegistrationComponent)
        {
            p_entityRegistrationComponent.AssociatedEntity = Entity.alloc();

            MyEvent<Entity>.register(
                    ref p_entityRegistrationComponent.AssociatedEntity.OnEntityDestroyed,
                    OnEntityDestroyed.build(p_entityRegistrationComponent)
                );
        }

        struct OnEntityDestroyed : MyEvent<Entity>.IEventCallback
        {
            public EntityRegistrationComponent EntityRegistrationComponent;

            public void Execute(ref Entity p_entity)
            {
                GameObject.Destroy(EntityRegistrationComponent.gameObject);
            }

            public static OnEntityDestroyed build(in EntityRegistrationComponent p_entityRegistrationComponent)
            {
                OnEntityDestroyed l_instance = new OnEntityDestroyed();
                l_instance.EntityRegistrationComponent = p_entityRegistrationComponent;
                return l_instance;
            }
        }
    }

}
