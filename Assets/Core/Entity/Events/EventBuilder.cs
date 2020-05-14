using _Attack;
using _Entity._Animation;
using _EventQueue;
using _NavigationGraph;
using System.Collections.Generic;

namespace _Entity._Events
{
    public static class EventBuilder
    {

        public static void moveToNavigationNode(Entity p_entity, EventQueue p_eventQueue, List<NavigationNode> p_path)
        {
            AnimationVisualFeedback l_animationVisualFeedback = EntityComponent.get_component<AnimationVisualFeedback>(p_entity);
            if (l_animationVisualFeedback != null)
            {
                EventQueue.enqueueEvent(p_eventQueue, AnimationVisualFeedbackPlayAsyncEvent.alloc(l_animationVisualFeedback,
                       AnimationLayers.LOCOMOTION, l_animationVisualFeedback.AnimationVisualFeedbackData.GetAnimation(AnimationLookupTag.LOCOMOTION).GetAnimationInput()));
            }

            var l_pathEnumerator = p_path.GetEnumerator();
            while (l_pathEnumerator.MoveNext())
            {
                EventQueue.enqueueEvent(p_eventQueue, NavigationNodeMoveEvent.alloc(p_entity, l_pathEnumerator.Current));
            }

            if (l_animationVisualFeedback != null)
            {
                EventQueue.enqueueEvent(p_eventQueue, AnimationVisualFeedbackDestroyLayerEvent.alloc(l_animationVisualFeedback, AnimationLayers.LOCOMOTION));
            }
        }

        public static void attackEvent(Entity p_entity, EventQueue p_eventQueue,
                    Entity p_sourceEntity, Entity p_targetEntity, Attack p_attack)
        {
            EventQueue.enqueueEvent(p_eventQueue, AttackEntityEvent.alloc(p_sourceEntity, p_targetEntity, p_attack));
        }
    }
}
