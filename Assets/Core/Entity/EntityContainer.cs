

using System;
using System.Collections.Generic;

namespace _Entity
{
    public static class EntityContainer
    {
        public static event Action<Entity> OnEntityCreated;
        public static List<Entity> Entities;

        static EntityContainer()
        {
            Entities = new List<Entity>();
        }

        public static void AddEntity(Entity p_entity)
        {
            Entities.Add(p_entity);
            OnEntityCreated?.Invoke(p_entity);
        }
    }

    public static class EntityDestructionContainer
    {
        public static List<Entity> EntitiesMarkedForDestruction;

        static EntityDestructionContainer()
        {
            EntitiesMarkedForDestruction = new List<Entity>();
        }
    }
}

