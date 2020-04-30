

using _Functional;
using _Navigation;
using System;
using System.Collections.Generic;

namespace _Entity
{
    /*
	An Entity is any object that can be placed in the NavigationGraph and can interact with it's environment.
	To avoid any undefined behavior, Entity destruction is never instant. To destroy the Entity, the flag m_markedForDestruction must be setted
	to true. Then, when the application collect Entities marked as destroyed, it will effectively detroy the object (see EntityDestructor).
    */
    public class Entity
    {
        public bool MarkedForDestruction;
        public NavigationNode CurrentNavigationNode;
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
            EntityDestructionContainer.EntitiesMarkedForDestruction.Add(p_entity);
        }

        public static void set_currentNavigationNode(Entity p_entity, NavigationNode p_newNavigationNode)
        {
            p_entity.CurrentNavigationNode = p_newNavigationNode;
        }

        public static void add_component<COMPONENT>(Entity p_entity, ref COMPONENT p_component) where COMPONENT : AEntityComponent
        {
            p_component.AssociatedEntity = p_entity;
            p_entity.Components[typeof(COMPONENT)] = p_component;
        }

        public static COMPONENT get_component<COMPONENT>(Entity p_entity) where COMPONENT : AEntityComponent
        {
            if (p_entity.Components.ContainsKey(typeof(COMPONENT)))
            {
                return (COMPONENT)p_entity.Components[typeof(COMPONENT)];
            }

            return default(COMPONENT);
        }

        public static void destroyEntity(Entity p_entity)
        {
            MyEvent<Entity>.broadcast(ref p_entity.OnEntityDestroyed, ref p_entity);
        }
    }

    public abstract class AEntityComponent
    {
        public Entity AssociatedEntity;
    }
}

