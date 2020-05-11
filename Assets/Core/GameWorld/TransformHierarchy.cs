
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _TrasformHierarchy
{
    [Serializable]
    public class TransformComponent
    {
        public bool HasChanged;
        public TransformComponentSynchronizer TransformComponentSynchronizer;

        private TransformComponent parent;

        public TransformComponent Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                {
                    parent.Childs.Remove(this);
                }
                parent = value;
                parent.Childs.Add(this);
            }
        }
        public List<TransformComponent> Childs;

        private float3 localPosition;
        public float3 LocalPosition
        {
            get { return localPosition; }
            set
            {
                if (!localPosition.Equals(value))
                {
                    localPosition = value;
                    invalidate(this);
                    if (TransformComponentSynchronizer != null) { TransformComponentSynchronizer.UpdatePosition = true; }
                }
            }
        }

        public float3 WorldPosition
        {
            get
            {
                updateMatrices(this);
                return math.mul(this.LocalToWorldMatrix, new float4(this.LocalPosition, 1.0f)).xyz;
            }
            set
            {
                updateMatrices(this);
                LocalPosition = math.mul(this.WorldToLocalMatrix, new float4(value, 1.0f)).xyz;
            }
        }

        private quaternion localRotation;
        public quaternion LocalRotation
        {
            get { return localRotation; }
            set
            {
                if (!localRotation.Equals(value))
                {
                    localRotation = value;
                    invalidate(this);
                    if (TransformComponentSynchronizer != null) { TransformComponentSynchronizer.UpdateRotation = true; }
                }
            }
        }

        public quaternion WorldRotation
        {
            get { return get_worldRotation(this); }
            set { set_worldRotation(this, value); }
        }

        private float3 localScale;
        public float3 LocalScale
        {
            get { return localScale; }
            set
            {
                if (!localScale.Equals(value))
                {
                    localScale = value;
                    invalidate(this);
                    if (TransformComponentSynchronizer != null) { TransformComponentSynchronizer.UpdateScale = true; }
                }
            }
        }

        public float4x4 WorldToLocalMatrix
        {
            get
            {
                return math.inverse(LocalToWorldMatrix);
            }
        }

        private float4x4 localToWorldMatrix;
        public float4x4 LocalToWorldMatrix
        {
            get
            {
                updateMatrices(this);
                return localToWorldMatrix;
            }
        }

        public static TransformComponent alloc()
        {
            TransformComponent l_instance = new TransformComponent();
            l_instance.Childs = new List<TransformComponent>();
            Zeroing(l_instance);
            return l_instance;
        }

        public static void Zeroing(TransformComponent p_transformComponent)
        {
            p_transformComponent.LocalPosition = float3.zero;
            p_transformComponent.LocalRotation = quaternion.identity;
            p_transformComponent.LocalScale = new float3(1.0f, 1.0f, 1.0f);
        }

        private static void updateMatrices(TransformComponent p_transformComponent)
        {
            if (p_transformComponent.HasChanged)
            {
                p_transformComponent.localToWorldMatrix = TransformComponent.get_localToWorldMatrix(p_transformComponent, false);
                p_transformComponent.HasChanged = false;
            }
        }

        private static void invalidate(TransformComponent p_transformComponent)
        {
            p_transformComponent.HasChanged = true;
            for (int i = 0; i < p_transformComponent.Childs.Count; i++)
            {
                invalidate(p_transformComponent.Childs[i]);
            }
        }

        private static float4x4 get_localToWorldMatrix(TransformComponent p_transformComponent, bool p_includeSelf)
        {
            TransformComponent l_currentParentTransformComponent = null;
            if (p_includeSelf)
            {
                l_currentParentTransformComponent = p_transformComponent;
            }
            else
            {
                l_currentParentTransformComponent = p_transformComponent.Parent;
            }

            float4x4 l_currentLocalToWorldMatrix = float4x4.identity;

            while (l_currentParentTransformComponent != null)
            {
                l_currentLocalToWorldMatrix = math.mul(float4x4.TRS(l_currentParentTransformComponent.LocalPosition, l_currentParentTransformComponent.LocalRotation, l_currentParentTransformComponent.LocalScale), l_currentLocalToWorldMatrix);
                l_currentParentTransformComponent = l_currentParentTransformComponent.Parent;
            }

            return l_currentLocalToWorldMatrix;
        }

        private static quaternion get_worldRotation(TransformComponent p_transformComponent)
        {
            quaternion l_returnWorldQuaternion = quaternion.identity;
            if (p_transformComponent.parent != null)
            {
                l_returnWorldQuaternion = math.mul(get_worldRotation(p_transformComponent.parent), l_returnWorldQuaternion);
            }

            l_returnWorldQuaternion = math.mul(l_returnWorldQuaternion, p_transformComponent.LocalRotation);
            return l_returnWorldQuaternion;
        }

        private static void set_worldRotation(TransformComponent p_transformComponent, quaternion p_worldQuaternion)
        {
            quaternion l_returnLocalQuaternion = p_worldQuaternion;
            if (p_transformComponent.parent != null)
            {
                l_returnLocalQuaternion = math.mul(math.inverse(get_worldRotation(p_transformComponent.parent)), l_returnLocalQuaternion);
            }

            p_transformComponent.LocalRotation = l_returnLocalQuaternion;
        }

    }

    public class TransformComponentSynchronizer
    {
        public TransformComponent TransformComponent;

        public bool UpdatePosition;
        public bool UpdateRotation;
        public bool UpdateScale;

        public static TransformComponentSynchronizer alloc(TransformComponent p_transformComponent)
        {
            TransformComponentSynchronizer l_instance = new TransformComponentSynchronizer();
            l_instance.TransformComponent = p_transformComponent;
            p_transformComponent.TransformComponentSynchronizer = l_instance;

            l_instance.UpdatePosition = false;
            l_instance.UpdateRotation = false;
            l_instance.UpdateScale = false;

            return l_instance;
        }
    }
}
