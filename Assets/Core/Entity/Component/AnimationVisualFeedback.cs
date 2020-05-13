using _AnimatorPlayable._Interface;
using System.Collections.Generic;

namespace _Entity._Animation
{
    /// <summary>
    /// This component act as a communication layer between the <see cref="Entity"/> and the <see cref="IAnimatorPlayable"/>.
    /// </summary>
    public class AnimationVisualFeedback : AEntityComponent
    {
        public AnimationVisualFeedbackData AnimationVisualFeedbackData;

        /// <summary>
        /// The instanced <see cref="IAnimatorPlayable"/> retrieved from the game world.
        /// </summary>
        public IAnimatorPlayable AnimatorPlayable;

        public static AnimationVisualFeedback alloc(ref AnimationVisualFeedbackData p_animationVisualFeedbackData, IAnimatorPlayable p_animatorPlayable)
        {
            AnimationVisualFeedback l_instance = new AnimationVisualFeedback();
            l_instance.AnimationVisualFeedbackData = p_animationVisualFeedbackData;
            l_instance.AnimatorPlayable = p_animatorPlayable;
            return l_instance;
        }
    }

    public struct AnimationVisualFeedbackData
    {
        public Dictionary<AnimationLookupTag, IAnimationPlayableDefinition> AnimationsContainer;

        public IAnimationPlayableDefinition GetAnimation(AnimationLookupTag p_animationLookupTag)
        {
            return AnimationsContainer[p_animationLookupTag];
        }
    }

    public enum AnimationLayers : int
    {
        BASE = -100,
        LOCOMOTION = -99,
        CONTEXT_ACTION = -98
    }

    public enum AnimationLookupTag : uint
    {
        /// <summary>
        /// The animation played when the <see cref="Entity"/> is moving around.
        /// See <see cref="EntityTurnIterationEvent"/> when processing the <see cref="MoveToNavigationNodeNode"/>.
        /// </summary>
        LOCOMOTION = 0,
        ATTACK = 1,
        IDLE = 2
    }
}
