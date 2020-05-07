using _ActionPoint;
using _Attack;
using _EventQueue;
using _Health;
using _HealthRecovery;
using _Locomotion;
using _NavigationEngine;
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
                EntityComponent.get_component<Locomotion>(SourceEntity).MoveToNavigationNode.Invoke(TargetNavigationNode, (p_startNavigationNode, p_endNavigationNode) =>
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
            EntityComponent.get_component<Locomotion>(Entity).WarpTo(TargetNavigationNode);
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
                        EntityEventsComposition.pushEntityDestroyedEvents(p_eventQueue, TargetEntity);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the <see cref="Entity.CurrentNavigationNode"/> to <see cref="NavigationNode"/>.
    /// <see cref="NavigationEngine"/> is updated to take into account this change. 
    /// </summary>
    public class EntityCurrentNavigationNodeChange : AEvent
    {
        public Entity Entity;
        /// <summary>
        /// The target <see cref="NavigationNode"/> can be null. Meaning that the <see cref="Entity"/> is no more on the <see cref="NavigationGraph"/>.
        /// </summary>
        public NavigationNode NavigationNode;

        public static EntityCurrentNavigationNodeChange alloc(Entity p_entity, NavigationNode p_navigationNode)
        {
            EntityCurrentNavigationNodeChange l_instance = new EntityCurrentNavigationNodeChange();
            l_instance.Entity = p_entity;
            l_instance.NavigationNode = p_navigationNode;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            NavigationNode l_oldNavigationNode = Entity.CurrentNavigationNode;
            Entity.set_currentNavigationNode(Entity, NavigationNode);
            NavigationEngine.resolveEntityNavigationNodeChange(NavigationEngineContainer.UniqueNavigationEngine, p_eventQueue, Entity, l_oldNavigationNode, NavigationNode);
        }
    }

    public class HealthRecoveryEvent : AEvent
    {
        public Entity TargetEntity;
        public HealthRecovery HealthRecoveryComponent;

        public static HealthRecoveryEvent alloc(Entity p_targetEntity, HealthRecovery p_healthRecoveryComponent)
        {
            HealthRecoveryEvent l_instance = new HealthRecoveryEvent();
            l_instance.TargetEntity = p_targetEntity;
            l_instance.HealthRecoveryComponent = p_healthRecoveryComponent;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            Health.addToCurrentHealth(EntityComponent.get_component<Health>(TargetEntity), HealthRecoveryComponent.HealthRecoveryData.RecoveredHealth);
        }
    }
}
