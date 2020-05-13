using _AnimatorPlayable;
using _GameLoop;
using _RuntimeObject;
using System;
using UnityEngine;

namespace _Entity._Animation
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationVisualFeedbackDefinition", menuName = "EntityComponents/AnimationVisualFeedbackDefinition")]
    public class AnimationVisualFeedbackDefinition : EntityDefinitionSubObject
    {
        public AnimationVisualFeedbackData AnimationVisualFeedbackData;

        static AnimationVisualFeedbackDefinition()
        {
            GameLoop.AddGameLoopCallback(GameLoopHook.LateTick, new GameLoopCallback() { GameLoopPriority = 0.0f, Callback = AnimatorPlayableComponentContainer.LateTick });
        }

        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            AnimatorPlayableComponent l_animatorPlayableComponent
                   = p_runtimeObjectRootComponent.m_InstanciatedRuntimeObject.RuntimeObjectRootComponent.gameObject.GetComponentInChildren<AnimatorPlayableComponent>();
            if (l_animatorPlayableComponent != null)
            {
                EntityComponent.add_component<AnimationVisualFeedback>(p_entity, AnimationVisualFeedback.alloc(ref AnimationVisualFeedbackData, l_animatorPlayableComponent.AnimatorPlayable));
            }
        }
    }

}
