using UnityEngine;
using System.Collections;
using _AnimatorPlayable._Interface;

namespace _Entity._Animation
{
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
        public IAnimatorPlayable AnimatorPlayable;
        public IAnimationPlayableDefinition LocomotionAnimation;
    }
}
