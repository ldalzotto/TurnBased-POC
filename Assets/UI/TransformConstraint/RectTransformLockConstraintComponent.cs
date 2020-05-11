using _GameLoop;
using System.Collections.Generic;
using UnityEngine;

namespace _UI._TransforConstraint
{

    public static class RectTransformLockConstraintContainer
    {

        public static float GAME_LOOP_PRIORITY = 0.0f;

        private static List<RectTransformLockConstraintComponent> RectTransformLockConstraintComponents = new List<RectTransformLockConstraintComponent>();

        public static void Add(RectTransformLockConstraintComponent p_rectTransformLockConstraintComponent)
        {
            if (RectTransformLockConstraintComponents.Count == 0)
            {
                GameLoop.AddGameLoopCallback(GameLoopHook.LateTick, new GameLoopCallback() { GameLoopPriority = GAME_LOOP_PRIORITY, Callback = RectTransformLockConstraintContainer.LateTick });
            }
            RectTransformLockConstraintComponents.Add(p_rectTransformLockConstraintComponent);
        }

        public static void Remove(RectTransformLockConstraintComponent p_rectTransformLockConstraintComponent)
        {
            RectTransformLockConstraintComponents.Remove(p_rectTransformLockConstraintComponent);
            if (RectTransformLockConstraintComponents.Count == 0)
            {
                GameLoop.RemoveGameLoopCallback(GameLoopHook.LateTick, new GameLoopCallback() { GameLoopPriority = GAME_LOOP_PRIORITY, Callback = RectTransformLockConstraintContainer.LateTick });
            }
        }

        private static void LateTick(float d)
        {
            for (int i = 0; i < RectTransformLockConstraintComponents.Count; i++)
            {
                RectTransformLockConstraintComponents[i].LateTick(d);
            }
        }

    }

    public class RectTransformLockConstraintComponent : MonoBehaviour
    {
        public Transform TargetTransform;

        private void OnEnable()
        {
            RectTransformLockConstraintContainer.Add(this);
        }

        private void OnDisable()
        {
            RectTransformLockConstraintContainer.Remove(this);
        }

        public void LateTick(float d)
        {
            if (TargetTransform != null)
            {
                Vector2 l_targetPosition = Camera.main.WorldToScreenPoint(TargetTransform.position);
                (transform as RectTransform).position = l_targetPosition;
            }
        }
    }

}
