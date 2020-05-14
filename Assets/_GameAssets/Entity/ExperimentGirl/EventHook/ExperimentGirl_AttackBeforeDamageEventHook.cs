using _AnimatorPlayable._Interface;
using _Attack;
using _Entity;
using _Entity._Animation;
using _Entity._Events;
using _EventQueue;
using Sirenix.OdinInspector;

namespace _GameAssets._Entity._ExperimentGirl
{
    public class ExperimentGirl_AttackBeforeDamageEventHook : SerializedScriptableObject, IAttackBeforeDamageEventHook
    {
        public IAnimationPlayableDefinition AttackAnimation;

        public Entity SourceEntity { get; set; }

        public void FeedEventQueue(EventQueue p_eventQueue, int p_insertionIndex)
        {
            EventQueue.insertEventAt(p_eventQueue, p_insertionIndex,
                    AnimationVisualFeedbackPlaySyncEvent.alloc(
                            EntityComponent.get_component<AnimationVisualFeedback>(SourceEntity),
                            AnimationLayers.CONTEXT_ACTION,
                            AttackAnimation.GetAnimationInput()
                        )
           );
        }
    }

}

