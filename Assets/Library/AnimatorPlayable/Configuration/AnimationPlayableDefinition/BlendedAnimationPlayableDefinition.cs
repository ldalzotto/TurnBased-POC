using _AnimatorPlayable._Interface;
using Sirenix.OdinInspector;
using System;
namespace _AnimatorPlayable
{
    [Serializable]
    public class BlendedAnimationPlayableDefinition : SerializedScriptableObject, IAnimationPlayableDefinition
    {
        public BlendedAnimationInput BlendedAnimationInput;

        public IAnimationInput GetAnimationInput()
        {
            return this.BlendedAnimationInput;
        }
    }
}