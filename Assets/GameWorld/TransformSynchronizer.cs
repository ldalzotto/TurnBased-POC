using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _TrasformHierarchy;
using _GameLoop;
using _UI._TransforConstraint;
using UnityEngine.Profiling;
using Unity.Mathematics;

namespace _GameWorld
{
    public static class TransformSynchronizerContainer
    {
        public static List<TransformSynchronizer> TransformSynchronizers;

        static TransformSynchronizerContainer()
        {
            TransformSynchronizers = new List<TransformSynchronizer>();
            GameLoop.AddGameLoopCallback(GameLoopHook.LateTick, new GameLoopCallback()
            {
                GameLoopPriority = Dichotomy.EvaluatePriority(new float[] { RectTransformLockConstraintContainer.GAME_LOOP_PRIORITY }, null),
                Callback = TransformSynchronizerContainer.LateTick
            });
        }

        private static void LateTick(float d)
        {
            for (int i = 0; i < TransformSynchronizers.Count; i++)
            {
                TransformSynchronizers[i].LateTick(d);
            }
        }
    }

    public class TransformSynchronizer
    {
        public Transform Transform;
        public TransformComponent TransformComponent;

        public static TransformSynchronizer alloc(Transform p_transform, TransformComponent p_transformComponent)
        {
            TransformSynchronizer l_instance = new TransformSynchronizer();
            l_instance.Transform = p_transform;
            l_instance.TransformComponent = p_transformComponent;
            return l_instance;
        }

        public void LateTick(float d)
        {
            if (TransformComponent.TransformComponentDeltaFlags.UpdatePosition)
            {
                Transform.position = TransformComponent.WorldPosition;
                TransformComponent.TransformComponentDeltaFlags.UpdatePosition = false;
            }
            if (TransformComponent.TransformComponentDeltaFlags.UpdateRotation)
            {
                Transform.rotation = TransformComponent.WorldRotation;
                TransformComponent.TransformComponentDeltaFlags.UpdateRotation = false;
            }
            if (TransformComponent.TransformComponentDeltaFlags.UpdateScale)
            {
                Transform.localScale = TransformComponent.LocalScale;
                TransformComponent.TransformComponentDeltaFlags.UpdateScale = false;
            }
        }
    }

}
