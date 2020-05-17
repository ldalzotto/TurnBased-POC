using _AnimatorPlayable._Interface;
using Sirenix.OdinInspector;
using System;

namespace _AnimatorPlayable
{
    [Serializable]
    public class SequencedAnimationPlayableDefinition : SerializedScriptableObject, IAnimationPlayableDefinition
    {
        public SequencedAnimationInput SequencedAnimationInput;

        public IAnimationInput GetAnimationInput()
        {
            return this.SequencedAnimationInput;
        }
    }
}