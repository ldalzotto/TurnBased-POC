using _NavigationGraph;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace _Level
{
    [CreateAssetMenu(fileName = "LevelInitializationDefinition", menuName = "LevelInitialization/LevelInitializationDefinition")]
    public class LevelInitializationDefinition : SerializedScriptableObject
    {
        public GameObject BaseLevelInitializationPrefab;
        public List<NavigationGraphAsset> NavigationGraphAssetsPick;
        public GameObject NavigationGraphObjectPrefab;
        public EntitySpawnDefinition EntitySpawnDefinition;
        public GameObject TurnTimelinePrefab;
    }
}

