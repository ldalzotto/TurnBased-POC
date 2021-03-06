﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace _RuntimeObject
{
    [ExecuteAfter(typeof(RuntimeObjectRootComponent))]
    public abstract class RuntimeComponent : SerializedMonoBehaviour
    {
        public RuntimeObject RuntimeObject;

        public virtual void Awake()
        {
            RuntimeObject = RuntimeObjectRootComponent.FindRootRuntimeObjectComponent(this).m_InstanciatedRuntimeObject;
            RuntimeObject.AddChildComponent(this);
        }

        public virtual void OnDestroy()
        {
            RuntimeObject.RemoveChildComponent(this);
        }

    }


    public struct CachedComponent<T> where T : Component
    {
        public static CachedComponent<T> New(GameObject p_gameObject)
        {
            CachedComponent<T> l_instanciatedCachedRuntimeComponent = new CachedComponent<T>();
            l_instanciatedCachedRuntimeComponent.m_gameObject = p_gameObject;
            return l_instanciatedCachedRuntimeComponent;
        }

        private GameObject m_gameObject;
        private T m_runtimeComponent;

        public T Get()
        {
            if (m_runtimeComponent == null)
            {
                m_runtimeComponent = m_gameObject.GetComponent<T>();
            }
            return m_runtimeComponent;
        }
    }

    public struct CachedRuntimeComponent<T> where T : RuntimeComponent
    {
        public static CachedRuntimeComponent<T> New(RuntimeObject p_runtimeObject)
        {
            CachedRuntimeComponent<T> l_instanciatedCachedRuntimeComponent = new CachedRuntimeComponent<T>();
            l_instanciatedCachedRuntimeComponent.m_runtimeObject = p_runtimeObject;
            return l_instanciatedCachedRuntimeComponent;
        }

        private RuntimeObject m_runtimeObject;
        private T m_runtimeComponent;

        public T Get()
        {
            if (m_runtimeComponent == null)
            {
                m_runtimeComponent = m_runtimeObject.FindComponent<T>();
            }
            return m_runtimeComponent;
        }
    }
}

