using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _Navigation;

namespace _Level
{
    public class LevelInitializationComponent : MonoBehaviour
    {
        public LevelInitializationDefinition LevelInitializationDefinition;

        private LevelInitializationGameObject LevelInitializationGameObject;

        private void Awake()
        {
            LevelInitializationGameObject = LevelInitializationGameObject.build(this);
        }

        private void Start()
        {
            GameObject l_navigationObject = GameObject.Instantiate(LevelInitializationDefinition.NavigationGraphObjectPrefab, LevelInitializationGameObject.LevelGlobalObjects.transform, false);
            NavigationGraphComponent l_navigationGraphComponent = l_navigationObject.GetComponent<NavigationGraphComponent>();
            l_navigationGraphComponent.NavigationGraphAsset = LevelInitializationDefinition.NavigationGraphAssetsPick[0];
            l_navigationObject.SetActive(true);

            EntitySpawnDefinition.spawnEntities(LevelInitializationDefinition.EntitySpawnDefinition, LevelInitializationGameObject.Entities.transform);

            GameObject.Instantiate(LevelInitializationDefinition.TurnTimelinePrefab, LevelInitializationGameObject.LevelGlobalObjects.transform, false);
        }
    }


    struct LevelInitializationGameObject
    {
        public GameObject LevelGlobalObjects;
        public GameObject Entities;
        public GameObject Other;

        public static LevelInitializationGameObject build(LevelInitializationComponent p_levelInitializationComponent)
        {
            LevelInitializationGameObject l_instance = new LevelInitializationGameObject();
            l_instance.LevelGlobalObjects = p_levelInitializationComponent.transform.Find("LevelGlobalObjects").gameObject;
            l_instance.Entities = p_levelInitializationComponent.transform.Find("Instanciated").Find("Entities").gameObject;
            l_instance.Other = p_levelInitializationComponent.transform.Find("Instanciated").Find("Other").gameObject;
            return l_instance;
        }
    }

}
