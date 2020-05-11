using _ActionPoint;
using _AnimatorPlayable._Interface;
using _Attack;
using _Entity._Animation;
using _EventQueue;
using _Locomotion;
using _NavigationEngine._Events;
using _NavigationGraph;
using System;

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
    /// May cause the <see cref="TargetEntity"/> to be destroyed.
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
                    ActionPoint.add(EntityComponent.get_component<ActionPoint>(SourceEntity), -1 * Attack.AttackData.Damage);
                    Attack.resolve(Attack, TargetEntity);

                    // TODO -> This logic must be inside an EventListener.
                    // Maybe reacting to an Interface or tag ?
                    if (TargetEntity.MarkedForDestruction)
                    {
                        EntityEventsComposition.addEntityDestroyedEvents(p_eventQueue.Events, TargetEntity, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Plays the <see cref="AnimationInput"/> at the provided <see cref="LayerID"/>.
    /// </summary>
    public class AnimationVisualFeedbackPlayAsyncEvent : AEvent
    {
        public AnimationVisualFeedback AnimationVisualFeedback;
        public int LayerID;
        public IAnimationInput AnimationInput;

        public static AnimationVisualFeedbackPlayAsyncEvent alloc(AnimationVisualFeedback p_animationVisualFeedback,
            int p_layerID, IAnimationInput p_animationInput)
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
            AnimationVisualFeedback.AnimatorPlayable.PlayAnimation(LayerID, AnimationInput);
        }
    }

    public class AnimationVisualFeedbackPlaySyncEvent : AAsyncEvent
    {
        public bool Completed;

        public AnimationVisualFeedback AnimationVisualFeedback;
        public int LayerID;
        public IAnimationInput AnimationInput;

        public static AnimationVisualFeedbackPlaySyncEvent alloc(AnimationVisualFeedback p_animationVisualFeedback,
            int p_layerID, IAnimationInput p_animationInput)
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
            AnimationVisualFeedback.AnimatorPlayable.DestroyLayer(LayerID);
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            AnimationVisualFeedback.AnimatorPlayable.PlayAnimation(LayerID, AnimationInput, OnAnimationEnd);
        }

        public override bool IsCompleted()
        {
            return Completed;
        }
    }

    public class AnimationVisualFeedbackDestroyLayerEvent : AEvent
    {
        public AnimationVisualFeedback AnimationVisualFeedback;
        public int LayerID;

        public static AnimationVisualFeedbackDestroyLayerEvent alloc(AnimationVisualFeedback p_animationVisualFeedback, int p_layerID)
        {
            AnimationVisualFeedbackDestroyLayerEvent l_instance = new AnimationVisualFeedbackDestroyLayerEvent();
            l_instance.AnimationVisualFeedback = p_animationVisualFeedback;
            l_instance.LayerID = p_layerID;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            AnimationVisualFeedback.AnimatorPlayable.DestroyLayer(LayerID);
        }
    }
}
