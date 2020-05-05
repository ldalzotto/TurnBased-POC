using System.Collections.Generic;
using UnityEngine;

namespace _RuntimeObject
{
    public static class RuntimeObjectContainer
    {
        public static List<RuntimeObject> RuntimeObjects = new List<RuntimeObject>();
        public static Dictionary<Collider, RuntimeObject> RuntimeObjectsByCollider = new Dictionary<Collider, RuntimeObject>();
        static RuntimeObjectContainer() { }

        public static void Dispose()
        {
            RuntimeObjects.Clear();
            RuntimeObjectsByCollider.Clear();
        }
    }

}
