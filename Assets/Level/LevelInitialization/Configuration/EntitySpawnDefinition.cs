using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace _Level
{
    [CreateAssetMenu(fileName = "EntitySpawnDefinition", menuName = "LevelInitialization/EntitySpawnDefinition")]
    public class EntitySpawnDefinition : SerializedScriptableObject
    {
        public int RandomEntityNumber;
        public GameObject EntityPrefab;

        public static void spawnEntities(in EntitySpawnDefinition p_entitySpawnDefinition, in Transform p_parent)
        {
            for(int i = 0; i < p_entitySpawnDefinition.RandomEntityNumber; i++)
            {
                GameObject.Instantiate(p_entitySpawnDefinition.EntityPrefab, p_parent);
            }
        }

    }
}

