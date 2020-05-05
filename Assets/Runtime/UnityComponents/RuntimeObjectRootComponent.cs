using UnityEngine;

namespace _RuntimeObject
{
    public class RuntimeObjectRootComponent : MonoBehaviour
    {
        public void Awake()
        {
            m_InstanciatedRuntimeObject = new RuntimeObject(this);
            m_instanciatedComponentsGameObject = new GameObject("InstanciatedComponents");
            m_instanciatedComponentsGameObject.transform.SetParent(transform);
            m_instanciatedComponentsGameObject.transform.localPosition = Vector3.zero;
            m_instanciatedComponentsGameObject.transform.localRotation = Quaternion.identity;
            m_instanciatedComponentsGameObject.transform.localScale = Vector3.one;
        }

        public void OnDestroy()
        {
            m_InstanciatedRuntimeObject.OnDestroy();
        }

        public RuntimeObject m_InstanciatedRuntimeObject { get; private set; }
        private GameObject m_instanciatedComponentsGameObject;
        public GameObject GetInstanciatedComponentsGameObject() { return m_instanciatedComponentsGameObject; }

        public static RuntimeObjectRootComponent FindRootRuntimeObjectComponent(Component p_startComponent)
        {
            RuntimeObjectRootComponent l_rootComponent = p_startComponent.GetComponent<RuntimeObjectRootComponent>();
            if (l_rootComponent == null)
            {
                l_rootComponent = p_startComponent.GetComponentInParent<RuntimeObjectRootComponent>();
            }

            return l_rootComponent;
        }

    }

}

