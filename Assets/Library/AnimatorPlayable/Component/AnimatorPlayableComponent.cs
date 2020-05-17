using System.Collections.Generic;
using UnityEngine;

namespace _AnimatorPlayable
{
    public static class AnimatorPlayableComponentContainer
    {
        public static List<AnimatorPlayableComponent> AnimatorPlayableComponents;

        static AnimatorPlayableComponentContainer()
        {
            AnimatorPlayableComponents = new List<AnimatorPlayableComponent>();
        }

        public static void LateTick(float d)
        {
            for (int i = 0; i < AnimatorPlayableComponents.Count; i++)
            {
                AnimatorPlayableComponents[i].AnimatorPlayable.Tick(d);
            }
        }
    }

    /// <summary>
    /// A wrapper for instancing <see cref="AnimatorPlayable"/> from game world. 
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorPlayableComponent : MonoBehaviour
    {
        public AnimatorPlayable AnimatorPlayable;

        private void Awake()
        {
            AnimatorPlayable = AnimatorPlayable.alloc(gameObject.name, GetComponent<Animator>());
            AnimatorPlayable.Play();
            AnimatorPlayableComponentContainer.AnimatorPlayableComponents.Add(this);
        }

        private void OnDestroy()
        {
            AnimatorPlayable.free(AnimatorPlayable);
            AnimatorPlayableComponentContainer.AnimatorPlayableComponents.Remove(this);
        }
    }

}
