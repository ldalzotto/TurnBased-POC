using UnityEngine;

namespace _RuntimeObject
{
    [ExecuteAfter(typeof(RuntimeObjectRootComponent))]
    [RequireComponent(typeof(Collider))]
    public class RuntimeColliderRegisteringComponent : MonoBehaviour
    {
        public void Awake()
        {
            RuntimeObjectRootComponent l_rootComponent = RuntimeObjectRootComponent.FindRootRuntimeObjectComponent(this);
            m_collider = GetComponent<Collider>();
            RuntimeObjectContainer.RuntimeObjectsByCollider.Add(m_collider, l_rootComponent.m_InstanciatedRuntimeObject);
        }

        public void OnDestroy()
        {
            RuntimeObjectContainer.RuntimeObjectsByCollider.Remove(m_collider);
        }

        private Collider m_collider;
    }

}

