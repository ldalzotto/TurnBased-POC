using _EventQueue;
using _NavigationGraph;
using System.Collections;
using UnityEngine;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                restartLevel();
            }
        }

        private void restartLevel()
        {
            GameObject.Destroy(this.gameObject);

            var l_nextLevelTemporaryObject = new GameObject("NextLevel_TemporaryObject");
            LevelInitializationComponent_PostMorted l_instanciatedLevelInitielizationFSMComponent_PostMortem = l_nextLevelTemporaryObject.AddComponent<LevelInitializationComponent_PostMorted>();
            l_instanciatedLevelInitielizationFSMComponent_PostMortem.LevelInitializationDefinition = LevelInitializationDefinition;
        }

        private void OnDestroy()
        {
            EventQueue.clearAll(EventQueueContainer.TurnTimelineQueue);
            EventQueue.clearAll(EventQueueContainer.EntityActionQueue);
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

    public class LevelInitializationComponent_PostMorted : MonoBehaviour
    {
        public LevelInitializationDefinition LevelInitializationDefinition;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            yield return new WaitForEndOfFrame();
            if (LevelInitializationDefinition != null)
            {
                GameObject l_baseLevelInitializationPrefab = GameObject.Instantiate(LevelInitializationDefinition.BaseLevelInitializationPrefab);
                LevelInitializationComponent l_levelInitializationFSMComponent = l_baseLevelInitializationPrefab.GetComponent<LevelInitializationComponent>();
                l_levelInitializationFSMComponent.LevelInitializationDefinition = LevelInitializationDefinition;
                l_baseLevelInitializationPrefab.SetActive(true);
            }

            GameObject.Destroy(this.gameObject);
        }
    }
}
