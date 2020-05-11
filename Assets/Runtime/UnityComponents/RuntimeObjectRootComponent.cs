using UnityEngine;

namespace _RuntimeObject
{
    public class RuntimeObjectRootComponent : MonoBehaviour
    {
        public void Awake()
        {
            m_InstanciatedRuntimeObject = new RuntimeObject(this);
        }

        public void OnDestroy()
        {
            m_InstanciatedRuntimeObject.OnDestroy();
        }

        public RuntimeObject m_InstanciatedRuntimeObject { get; private set; }

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

