using _ActionPoint;
using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _AI._DecisionTree._Builder;
using _Entity._Animation;
using _Entity._Events;
using _EventQueue;

namespace _Entity._Turn
{

    /// <summary>
    /// Performs an iteration loop on the <see cref="Entity"/> to check if there is <see cref="AEvent"/> to performs in relation of the <see cref="Entity"/> turn.
    /// When there is no more <see cref="AEvent"/>, the <see cref="EntityTurnIterationEndEvent"/> is queued.
    /// </summary>
    public class EntityTurnIterationEvent : AEvent
    {
        public Entity Entity;

        public static EntityTurnIterationEvent alloc(Entity p_entity)
        {
            EntityTurnIterationEvent l_instance = new EntityTurnIterationEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }

        /// <summary>
        /// The iteration loop is :
        ///     * Making a choice over a builded <see cref="_AI._DecisionTree.DecisionTree"/>.
        ///     * Queueing <see cref="AEvent"/> based on AI choice.
        /// </summary>
        /// <param name="p_eventQueue"></param>
        public override void Execute(EventQueue p_eventQueue)
        {
            int l_eventQueueSizeBeforeInsersion = p_eventQueue.Events.Count;

            if (EntityComponent.get_component<ActionPoint>(Entity).ActionPointData.CurrentActionPoints > 0.00f)
            {

                DecisionTree l_decisionTree = DecisionTree.alloc();
                TreeBuilder.buildAggressiveTree(l_decisionTree, Entity);
                var l_choice = Algorithm.traverseDecisionTree(l_decisionTree, Entity);

                // TODO -> this piece of logic must be elsewhere.
                for (int i = 0; i < l_choice.PickedChoice.DecisionNodesChoiceOrdered.Length; i++)
                {
                    ADecisionNode l_decisionNode = l_choice.PickedChoice.DecisionNodesChoiceOrdered[i];
                    if (l_decisionNode.DecisionNodeConsumerAction == EDecisionNodeConsumerAction.EXECUTE)
                    {
                        switch (l_decisionNode)
                        {
                            // Push to the event queue the will of moving along a path
                            case MoveToNavigationNodeNode l_moveToNavigationNode:
                                {
                                    AnimationVisualFeedback l_animationVisualFeedback = EntityComponent.get_component<AnimationVisualFeedback>(Entity);
                                    if (l_animationVisualFeedback != null)
                                    {
                                        EventQueue.enqueueEvent(p_eventQueue, AnimationVisualFeedbackPlayAsyncEvent.alloc(l_animationVisualFeedback,
                                               (int)AnimationLayers.LOCOMOTION, l_animationVisualFeedback.AnimationVisualFeedbackData.GetAnimation(AnimationLookupTag.LOCOMOTION).GetAnimationInput()));
                                    }

                                    var l_pathEnumerator = l_moveToNavigationNode.CalculatedPath.GetEnumerator();
                                    while (l_pathEnumerator.MoveNext())
                                    {
                                        EventQueue.enqueueEvent(p_eventQueue, NavigationNodeMoveEvent.alloc(Entity, l_pathEnumerator.Current));
                                    }

                                    if (l_animationVisualFeedback != null)
                                    {
                                        EventQueue.enqueueEvent(p_eventQueue, AnimationVisualFeedbackDestroyLayerEvent.alloc(l_animationVisualFeedback, (int)AnimationLayers.LOCOMOTION));
                                    }
                                }
                                break;

                            case AttackNode l_attackNode:

                                {
                                    AnimationVisualFeedback l_animationVisualFeedback = EntityComponent.get_component<AnimationVisualFeedback>(Entity);

                                    for (int j = 0; j < l_attackNode.NumberOfAttacks; j++)
                                    {
                                        if (l_animationVisualFeedback != null)
                                        {
                                            EventQueue.enqueueEvent(p_eventQueue, AnimationVisualFeedbackPlaySyncEvent.alloc(l_animationVisualFeedback, (int)AnimationLayers.CONTEXT_ACTION,
                                                   l_animationVisualFeedback.AnimationVisualFeedbackData.GetAnimation(AnimationLookupTag.ATTACK).GetAnimationInput()));
                                        }

                                        EventQueue.enqueueEvent(p_eventQueue, AttackEntityEvent.alloc(l_attackNode.SourceEntity, l_attackNode.TargetEntity, l_attackNode.Attack));
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            // This means that at least one action is performed.
            // Thus, we try to re-evaluate action choice to be sure that there is nothing else to do for the associated Entity.
            if (l_eventQueueSizeBeforeInsersion != p_eventQueue.Events.Count)
            {
                EventQueue.enqueueEvent(p_eventQueue, EntityTurnIterationEvent.alloc(Entity));
            }
            else
            {
                EventQueue.enqueueEvent(p_eventQueue, EntityTurnIterationEndEvent.alloc(Entity));
            }
        }
    }

    /// <summary>
    /// This event is used to have a visual representation of when the <see cref="Entity"/> has no more <see cref="AEvent"/> to perform.
    /// Queued when there is no more <see cref="AEvent"/> to perform.
    /// </summary>
    public class EntityTurnIterationEndEvent : AEvent
    {
        public Entity Entity;

        public static EntityTurnIterationEndEvent alloc(Entity p_entity)
        {
            EntityTurnIterationEndEvent l_instance = new EntityTurnIterationEndEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }
    }
}