using System.Collections.Generic;

namespace _RuntimeObject
{
    public class RuntimeObject
    {
        public RuntimeObject(RuntimeObjectRootComponent p_rootComponentReference)
        {
            RuntimeObjectContainer.RuntimeObjects.Add(this);
            m_childComponents = new List<RuntimeComponent>();
            RuntimeObjectRootComponent = p_rootComponentReference;
        }

        internal void OnDestroy()
        {
            RuntimeObjectContainer.RuntimeObjects.Remove(this);
        }

        public void AddChildComponent(RuntimeComponent p_runtimeComponent)
        {
            m_childComponents.Add(p_runtimeComponent);
        }

        public void RemoveChildComponent(RuntimeComponent p_runtimeComponent)
        {
            m_childComponents.Remove(p_runtimeComponent);
        }

        public T FindComponent<T>() where T : RuntimeComponent
        {
            for (int i = 0; i < m_childComponents.Count; i++)
            {
                RuntimeComponent l_runtimeComponent = m_childComponents[i];
                if (l_runtimeComponent.GetType() == typeof(T))
                {
                    return (T)l_runtimeComponent;
                }
            }
            return null;
        }

        public T FindComponentFromInterface<T>() where T : class
        {
            for (int i = 0; i < m_childComponents.Count; i++)
            {
                RuntimeComponent l_runtimeComponent = m_childComponents[i];
                if (l_runtimeComponent is T)
                {
                    return l_runtimeComponent as T;
                }
            }
            return default;
        }

        private List<RuntimeComponent> m_childComponents;
        public RuntimeObjectRootComponent RuntimeObjectRootComponent;
    }

}
