using _AnimatorPlayable._Interface;

namespace _Entity._Animation
{
    /// <summary>
    /// This component act as a communication layer between the <see cref="Entity"/> and the <see cref="IAnimatorPlayable"/>.
    /// </summary>
    public class AnimationVisualFeedback : AEntityComponent
    {
        public AnimationVisualFeedbackData AnimationVisualFeedbackData;

        public static AnimationVisualFeedback alloc(ref AnimationVisualFeedbackData p_animationVisualFeedbackData)
        {
            AnimationVisualFeedback l_instance = new AnimationVisualFeedback();
            l_instance.AnimationVisualFeedbackData = p_animationVisualFeedbackData;
            return l_instance;
        }
    }

    public struct AnimationVisualFeedbackData
    {
        /// <summary>
        /// The instanced <see cref="IAnimatorPlayable"/> retrieved from the game world.
        /// </summary>
        public IAnimatorPlayable AnimatorPlayable;

        /// <summary>
        /// The animation played when the <see cref="Entity"/> is moving around.
        /// See <see cref="EntityTurnIterationEvent"/> when processing the <see cref="MoveToNavigationNodeNode"/>.
        /// </summary>
        public IAnimationPlayableDefinition LocomotionAnimation;

        public IAnimationPlayableDefinition AttackAnimation;
    }

    public enum AnimationLayers : int
    {
        LOCOMOTION = -100,
        CONTEXT_ACTION = -99
    }
}
