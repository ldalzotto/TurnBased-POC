

using System;
using System.Collections.Generic;
using _Entity._Events;
using _EventQueue;
using _Functional;
using _Navigation;

namespace _Entity
{
    /// <summary>
    /// An Entity is any object that can be placed in the NavigationGraph and can interact with it's environment.
	/// To avoid any undefined behavior, Entity destruction is never instant. To destroy the Entity, the flag <see cref="MarkedForDestruction"/>  must be setted
    /// to true. Then, an <see cref="EntityDestroyEvent"/> is sended.
    /// </summary>
    public class Entity
    {
        public bool MarkedForDestruction;
        public NavigationNode CurrentNavigationNode { get; private set; }
        public RefDictionary<Type, AEntityComponent> Components;

        #region events
        public MyEvent<Entity> OnEntityDestroyed;
        #endregion

        public static Entity alloc()
        {
            Entity l_instance = new Entity();
            l_instance.MarkedForDestruction = false;
            l_instance.CurrentNavigationNode = null;
            l_instance.Components = new RefDictionary<Type, AEntityComponent>();
            l_instance.OnEntityDestroyed = MyEvent<Entity>.build();

            EntityContainer.AddEntity(l_instance);

            return l_instance;
        }

        public static void markForDestruction(Entity p_entity)
        {
            p_entity.MarkedForDestruction = true;
            EventQueue.insertEventAt(EventQueueContainer.EntityActionQueue, 0, EntityDestroyEvent.alloc(p_entity));
        }

        public static void set_currentNavigationNode(Entity p_entity, NavigationNode p_newNavigationNode)
        {
            p_entity.CurrentNavigationNode = p_newNavigationNode;
        }

        public static void destroyEntity(Entity p_entity)
        {
            MyEvent<Entity>.broadcast(ref p_entity.OnEntityDestroyed, ref p_entity);

            var l_componentIterator = p_entity.Components.GetRefEnumerator();
            while (l_componentIterator.MoveNext())
            {
                EntityComponentContainer.onComponentRemoved(l_componentIterator.GetCurrentRef().value);
            }

            EntityContainer.Entities.Remove(p_entity);
        }
    }

    #region Entity Component

    public abstract class AEntityComponent
    {
        public Entity AssociatedEntity;
    }

    public static class EntityComponent
    {
        public static void add_component<COMPONENT>(Entity p_entity, COMPONENT p_component) where COMPONENT : AEntityComponent
        {
            p_component.AssociatedEntity = p_entity;
            p_entity.Components[typeof(COMPONENT)] = p_component;
            EntityComponentContainer.onComponentAdded(p_component);
        }

        public static COMPONENT get_component<COMPONENT>(Entity p_entity) where COMPONENT : AEntityComponent
        {
            if (p_entity.Components.ContainsKey(typeof(COMPONENT)))
            {
                return (COMPONENT)p_entity.Components[typeof(COMPONENT)];
            }

            return default(COMPONENT);
        }
    }

    public static class EntityComponentContainer
    {
        public static RefDictionary<Type, List<AEntityComponent>> Components;

        public static RefDictionary<Type, MyEvent<AEntityComponent>> ComponentAddedEvents;
        public static RefDictionary<Type, MyEvent<AEntityComponent>> ComponentRemovedEvents;

        static EntityComponentContainer()
        {
            Components = new RefDictionary<Type, List<AEntityComponent>>();
            ComponentAddedEvents = new RefDictionary<Type, MyEvent<AEntityComponent>>();
            ComponentRemovedEvents = new RefDictionary<Type, MyEvent<AEntityComponent>>();
        }

        public static int registerComponentAddedEvent<COMPONENT>(ref MyEvent<AEntityComponent>.IEventCallback p_callback) where COMPONENT : AEntityComponent
        {
            if (!ComponentAddedEvents.ContainsKey(typeof(COMPONENT)))
            {
                ComponentAddedEvents[typeof(COMPONENT)] = MyEvent<AEntityComponent>.build();
            }

            return MyEvent<AEntityComponent>.register(
                    ref ComponentAddedEvents.ValueRef(typeof(COMPONENT)),
                    ref p_callback
            );
        }

        public static void unRegisterComponentAddedEvent<COMPONENT>(int p_handler) where COMPONENT : AEntityComponent
        {
            if (!ComponentAddedEvents.ContainsKey(typeof(COMPONENT)))
            {
                MyEvent<AEntityComponent>.unRegister(ref ComponentAddedEvents.ValueRef(typeof(COMPONENT)), p_handler);
            }
        }

        public static void onComponentAdded(AEntityComponent p_component)
        {
            Type l_componentType = p_component.GetType();
            if (!Components.ContainsKey(l_componentType))
            {
                Components[l_componentType] = new List<AEntityComponent>();
            }

            Components[l_componentType].Add(p_component);

            if (ComponentAddedEvents.ContainsKey(l_componentType))
            {
                MyEvent<AEntityComponent>.broadcast(
                          ref ComponentAddedEvents.ValueRef(l_componentType),
                          ref p_component
                    );
            }
        }

        public static int registerComponentRemovedEvent<COMPONENT>(ref MyEvent<AEntityComponent>.IEventCallback p_callback) where COMPONENT : AEntityComponent
        {
            if (!ComponentRemovedEvents.ContainsKey(typeof(COMPONENT)))
            {
                ComponentRemovedEvents[typeof(COMPONENT)] = MyEvent<AEntityComponent>.build();
            }

            return MyEvent<AEntityComponent>.register(
                    ref ComponentRemovedEvents.ValueRef(typeof(COMPONENT)),
                    ref p_callback
            );
        }

        public static void unRegisterComponentRemovedEvent<COMPONENT>(int p_handler) where COMPONENT : AEntityComponent
        {
            if (!ComponentRemovedEvents.ContainsKey(typeof(COMPONENT)))
            {
                MyEvent<AEntityComponent>.unRegister(ref ComponentRemovedEvents.ValueRef(typeof(COMPONENT)), p_handler);
            }
        }

        public static void onComponentRemoved(AEntityComponent p_component)
        {
            Type l_componentType = p_component.GetType();
            Components[l_componentType].Remove(p_component);

            if (ComponentRemovedEvents.ContainsKey(l_componentType))
            {
                MyEvent<AEntityComponent>.broadcast(
                           ref ComponentRemovedEvents.ValueRef(l_componentType),
                           ref p_component
                     );
            }
        }

    }

    #endregion
}

