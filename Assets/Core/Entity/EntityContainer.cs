﻿

using _Functional;
using System.Collections.Generic;

namespace _Entity
{
    public static class EntityContainer
    {
        public static List<Entity> Entities;

        static EntityContainer()
        {
            Entities = new List<Entity>();
        }

        public static void AddEntity(Entity p_entity)
        {
            Entities.Add(p_entity);
        }
    }
}

