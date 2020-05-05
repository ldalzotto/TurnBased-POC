using Sirenix.OdinInspector;
using UnityEngine;

namespace _Level
{
    [CreateAssetMenu(fileName = "EntitySpawnDefinition", menuName = "LevelInitialization/EntitySpawnDefinition")]
    public class EntitySpawnDefinition : SerializedScriptableObject
    {
        public int RandomEntityNumber;
        public GameObject EntityPrefab;

        public static void spawnEntities(EntitySpawnDefinition p_entitySpawnDefinition, Transform p_parent)
        {
            for (int i = 0; i < p_entitySpawnDefinition.RandomEntityNumber; i++)
            {
                GameObject.Instantiate(p_entitySpawnDefinition.EntityPrefab, p_parent);
            }
        }

    }
}

