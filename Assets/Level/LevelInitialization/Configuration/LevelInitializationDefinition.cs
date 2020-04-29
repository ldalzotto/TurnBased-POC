using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using _Navigation;
using System.Collections.Generic;

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

