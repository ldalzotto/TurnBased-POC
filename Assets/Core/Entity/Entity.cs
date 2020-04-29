

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
        public Dictionary<EntityComponentTag, IEntityComponent> Components;

        #region events
        public event Action<Entity> OnEntityDestroyed;
        #endregion

        private Entity()
        {
            MarkedForDestruction = false;
            CurrentNavigationNode = null;
            Components = new Dictionary<EntityComponentTag, IEntityComponent>();
            OnEntityDestroyed = null;
        }

        public static Entity alloc()
        {
            Entity l_instance = new Entity();
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

        public static void destroyEntity(Entity p_entity)
        {
            p_entity.OnEntityDestroyed?.Invoke(p_entity);
        }
    }
}

