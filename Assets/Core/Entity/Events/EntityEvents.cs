using _ActionPoint;
using _AnimatorPlayable._Interface;
using _Attack;
using _Entity._Animation;
using _EventQueue;
using _Health;
using _Locomotion;
using _NavigationEngine._Events;
using _NavigationGraph;
using System;
using Unity.Mathematics;

namespace _Entity._Events
{
    public class EntityCreateEvent : AEvent
    {
        [NonSerialized]
        public Action<Entity> OnEntityCreatedLocalCallback;
        public Entity CreatedEntity;

        public static EntityCreateEvent alloc(Action<Entity> p_onEntityCreatedLocalCallback)
        {
            EntityCreateEvent l_instance = new EntityCreateEvent();
            l_instance.OnEntityCreatedLocalCallback = p_onEntityCreatedLocalCallback;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            CreatedEntity = Entity.alloc();
            OnEntityCreatedLocalCallback?.Invoke(CreatedEntity);
        }

    }
    public class EntityDestroyEvent : AEvent
    {
        public Entity EntityToDestroy;

        public static EntityDestroyEvent alloc(Entity p_entityToDestroy)
        {
            EntityDestroyEvent l_instance = new EntityDestroyEvent();
            l_instance.EntityToDestroy = p_entityToDestroy;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            Entity.destroyEntity(EntityToDestroy);
        }
    }

    /// <summary>
    /// The <see cref="NavigationNodeMoveEvent"/> moves the <see cref="SourceEntity"/> to the <see cref="NavigationNode"/> <see cref="TargetNavigationNode"/>.
    /// Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public class NavigationNodeMoveEvent : AAsyncEvent
    {
        public static NavigationNodeMoveEvent alloc(Entity p_sourceEntity, NavigationNode p_navigationNode)
        {
            NavigationNodeMoveEvent l_instance = new NavigationNodeMoveEvent();
            l_instance.Completed = false;
            l_instance.MovementAllowed = false;
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetNavigationNode = p_navigationNode;
            return l_instance;
        }

        public Entity SourceEntity;
        public NavigationNode TargetNavigationNode;

        public bool Completed;
        public bool MovementAllowed;

        // called only once
        public override void Execute(EventQueue p_eventQueue)
        {
            if (SourceEntity.MarkedForDestruction)
            {
                Completed = true;
                return;
            }

            float l_costToMove = _ActionPoint.Calculations.actionPointBetweenNavigationNodes(SourceEntity.CurrentNavigationNode, TargetNavigationNode);
            ActionPoint l_actionPoint = EntityComponent.get_component<ActionPoint>(SourceEntity);
            MovementAllowed = (l_actionPoint.ActionPointData.CurrentActionPoints >= l_costToMove
                && NavigationGraphAlgorithm.areNavigationNodesNeighbors(NavigationGraphContainer.UniqueNavigationGraph, SourceEntity.CurrentNavigationNode, TargetNavigationNode, NavigationGraphFlag.CURRENT));

            if (MovementAllowed)
            {
                LocomotionSystemV2.HeadTowardsNode(EntityComponent.get_component<Locomotion>(SourceEntity).LocomotionSystemV2, TargetNavigationNode,
                    (p_startNavigationNode, p_endNavigationNode) =>
                    {
                        ActionPoint.add(l_actionPoint, -1 * l_costToMove);
                        Completed = true;
                    });
            }
            else
            {
                Completed = true;
            }
        }

        // called every event queue step
        public override bool IsCompleted()
        {
            return Completed;
        }

        public override void OnCompleted(EventQueue p_eventQueue)
        {
            base.OnCompleted(p_eventQueue);
            if (MovementAllowed)
            {
                EventQueue.insertEventAt(p_eventQueue, 0, EntityCurrentNavigationNodeChange.alloc(SourceEntity, TargetNavigationNode));
            }
        }
    }

    public class NavigationNodeWarpEntityEvent : AEvent
    {
        public Entity Entity;
        public NavigationNode TargetNavigationNode;

        public static NavigationNodeWarpEntityEvent alloc(Entity p_entity, NavigationNode p_targetNavigationNode)
        {
            NavigationNodeWarpEntityEvent l_instance = new NavigationNodeWarpEntityEvent();
            l_instance.Entity = p_entity;
            l_instance.TargetNavigationNode = p_targetNavigationNode;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            LocomotionSystemV2.warp(EntityComponent.get_component<Locomotion>(Entity).LocomotionSystemV2, TargetNavigationNode);
            EventQueue.insertEventAt(p_eventQueue, 0, EntityCurrentNavigationNodeChange.alloc(Entity, TargetNavigationNode));
        }
    }

    /// <summary>
    /// Attacks the <see cref="TargetEntity"/> with the provided <see cref="Attack"/>.
    ///  - Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public class AttackEntityEvent : AEvent
    {
        public Entity SourceEntity;
        public Entity TargetEntity;
        public Attack Attack;

        public static AttackEntityEvent alloc(Entity p_sourceEntity, Entity p_targetEntity, Attack p_attack)
        {
            AttackEntityEvent l_instance = new AttackEntityEvent();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetEntity = p_targetEntity;
            l_instance.Attack = p_attack;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            if (
                !SourceEntity.MarkedForDestruction && !TargetEntity.MarkedForDestruction &&
                NavigationGraphAlgorithm.areNavigationNodesNeighbors(NavigationGraphContainer.UniqueNavigationGraph, SourceEntity.CurrentNavigationNode, TargetEntity.CurrentNavigationNode, NavigationGraphFlag.SNAPSHOT))
            {
                ActionPoint l_actionPoint = EntityComponent.get_component<ActionPoint>(SourceEntity);
                if (l_actionPoint.ActionPointData.CurrentActionPoints >= Attack.AttackData.APCost)
                {
                    ActionPoint.add(EntityComponent.get_component<ActionPoint>(SourceEntity), -1 * Attack.AttackData.APCost);

                    EntityGameWorld.orientTowards(ref SourceEntity.EntityGameWorld, TargetEntity, math.up());

                    float l_appliedDamage = Attack.resolve(Attack, TargetEntity);

                    EventQueue.insertEventAt(p_eventQueue, 0, EntityApplyDamageEvent.alloc(TargetEntity, l_appliedDamage));

                    if (Attack.AttackBeforeDamageEventHook != null)
                    {
                        Attack.AttackBeforeDamageEventHook.SourceEntity = SourceEntity;
                        Attack.AttackBeforeDamageEventHook.FeedEventQueue(p_eventQueue, 0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Apply <see cref="SignedAmountOfDamage"/> to the <see cref="TargetEntity"/>.
    /// May cause the <see cref="TargetEntity"/> to be destroyed.
    /// </summary>
    public class EntityApplyDamageEvent : AEvent
    {
        public Entity TargetEntity;
        public float SignedAmountOfDamage;

        public static EntityApplyDamageEvent alloc(Entity p_targetEntity, float p_signedAmountOfDamage)
        {
            EntityApplyDamageEvent l_instance = new EntityApplyDamageEvent();
            l_instance.TargetEntity = p_targetEntity;
            l_instance.SignedAmountOfDamage = p_signedAmountOfDamage;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);

            Health l_targetEntityHealth = EntityComponent.get_component<Health>(TargetEntity);
            Health.addToCurrentHealth(l_targetEntityHealth, SignedAmountOfDamage);

            if (TargetEntity.MarkedForDestruction)
            {
                EntityEventsComposition.addEntityDestroyedEvents(p_eventQueue.Events, TargetEntity, true);
            }
        }
    }

    /// <summary>
    /// Plays the <see cref="AnimationInput"/> at the provided <see cref="LayerID"/>.
    /// </summary>
    public class AnimationVisualFeedbackPlayAsyncEvent : AEvent
    {
        public AnimationVisualFeedback AnimationVisualFeedback;
        public AnimationLayers LayerID;
        public IAnimationInput AnimationInput;

        public static AnimationVisualFeedbackPlayAsyncEvent alloc(AnimationVisualFeedback p_animationVisualFeedback,
            AnimationLayers p_layerID, IAnimationInput p_animationInput)
        {
            AnimationVisualFeedbackPlayAsyncEvent l_instance = new AnimationVisualFeedbackPlayAsyncEvent();
            l_instance.AnimationVisualFeedback = p_animationVisualFeedback;
            l_instance.LayerID = p_layerID;
            l_instance.AnimationInput = p_animationInput;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            AnimationVisualFeedback.AnimatorPlayable.PlayAnimation((int)LayerID, AnimationInput);
        }
    }

    public class AnimationVisualFeedbackPlaySyncEvent : AAsyncEvent
    {
        public bool Completed;

        public AnimationVisualFeedback AnimationVisualFeedback;
        public AnimationLayers LayerID;
        public IAnimationInput AnimationInput;

        public static AnimationVisualFeedbackPlaySyncEvent alloc(AnimationVisualFeedback p_animationVisualFeedback,
            AnimationLayers p_layerID, IAnimationInput p_animationInput)
        {
            AnimationVisualFeedbackPlaySyncEvent l_instance = new AnimationVisualFeedbackPlaySyncEvent();
            l_instance.Completed = false;
            l_instance.AnimationVisualFeedback = p_animationVisualFeedback;
            l_instance.LayerID = p_layerID;
            l_instance.AnimationInput = p_animationInput;
            return l_instance;
        }

        private void OnAnimationEnd()
        {
            Completed = true;
            AnimationVisualFeedback.AnimatorPlayable.DestroyLayer((int)LayerID);
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            AnimationVisualFeedback.AnimatorPlayable.PlayAnimation((int)LayerID, AnimationInput, OnAnimationEnd);
        }

        public override bool IsCompleted()
        {
            return Completed;
        }
    }

    public class AnimationVisualFeedbackDestroyLayerEvent : AEvent
    {
        public AnimationVisualFeedback AnimationVisualFeedback;
        public AnimationLayers LayerID;

        public static AnimationVisualFeedbackDestroyLayerEvent alloc(AnimationVisualFeedback p_animationVisualFeedback, AnimationLayers p_layerID)
        {
            AnimationVisualFeedbackDestroyLayerEvent l_instance = new AnimationVisualFeedbackDestroyLayerEvent();
            l_instance.AnimationVisualFeedback = p_animationVisualFeedback;
            l_instance.LayerID = p_layerID;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            AnimationVisualFeedback.AnimatorPlayable.DestroyLayer((int)LayerID);
        }
    }
}
