using _AnimatorPlayable._Interface;
using Sirenix.OdinInspector;
using System;

namespace _AnimatorPlayable
{
    [Serializable]
    public class TwoDBlendTreePlayableDefinition : SerializedScriptableObject, IAnimationPlayableDefinition
    {
        public TwoDAnimationInput TwoDAnimationInput;

        public IAnimationInput GetAnimationInput()
        {
            return this.TwoDAnimationInput;
        }
    }
}