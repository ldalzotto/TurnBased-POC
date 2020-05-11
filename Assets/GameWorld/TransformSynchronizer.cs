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
        public TransformComponentSynchronizer TransformComponentSynchronizer;

        public static TransformSynchronizer alloc(Transform p_transform, TransformComponentSynchronizer p_transformComponentSynchronizer)
        {
            TransformSynchronizer l_instance = new TransformSynchronizer();
            l_instance.Transform = p_transform;
            l_instance.TransformComponentSynchronizer = p_transformComponentSynchronizer;
            return l_instance;
        }

        public void LateTick(float d)
        {
            if (TransformComponentSynchronizer.UpdatePosition)
            {
                Transform.position = TransformComponentSynchronizer.TransformComponent.WorldPosition;
                TransformComponentSynchronizer.UpdatePosition = false;
            }
            if (TransformComponentSynchronizer.UpdateRotation)
            {
                Transform.rotation = TransformComponentSynchronizer.TransformComponent.WorldRotation;
                TransformComponentSynchronizer.UpdateRotation = false;
            }
            if (TransformComponentSynchronizer.UpdateScale)
            {
                Transform.localScale = TransformComponentSynchronizer.TransformComponent.LocalScale;
                TransformComponentSynchronizer.UpdateScale = false;
            }
        }
    }

}
