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
    }
}

