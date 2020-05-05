

using _Functional;
using System.Collections.Generic;

namespace _Entity
{
    public static class EntityContainer
    {
        public static MyEvent<Entity> OnEntityCreated;
        public static List<Entity> Entities;

        static EntityContainer()
        {
            Entities = new List<Entity>();
            OnEntityCreated = MyEvent<Entity>.build();
        }

        public static void AddEntity(Entity p_entity)
        {
            Entities.Add(p_entity);
            MyEvent<Entity>.broadcast(ref OnEntityCreated, ref p_entity);
        }
    }
}

