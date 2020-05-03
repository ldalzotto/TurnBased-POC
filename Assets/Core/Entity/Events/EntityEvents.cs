using System.Collections;
using _ActionPoint;
using _EventQueue;
using _Health;
using _Locomotion;
using _Navigation;

namespace _Entity._Events
{
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


    public class HealthReductionEvent : AEvent
    {
        public Entity Entity;
        public static HealthReductionEvent alloc(Entity p_entity)
        {
            HealthReductionEvent l_instance = new HealthReductionEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }
        public override void Execute(EventQueue p_eventQueue)
        {
            Health.addToCurrentHealth(
                   EntityComponent.get_component<Health>(Entity),
                   -1.0f
               );
        }
    }


    /// <summary>
    /// The <see cref="NavigationNodeMoveEntityEvent"/> moves the <see cref="SourceEntity"/> to the <see cref="NavigationNode"/> <see cref="TargetNavigationNode"/>.
    /// Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public class NavigationNodeMoveEntityEvent : AAsyncEvent
    {
        public static NavigationNodeMoveEntityEvent alloc(Entity p_sourceEntity, NavigationNode p_navigationNode)
        {
            NavigationNodeMoveEntityEvent l_instance = new NavigationNodeMoveEntityEvent();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetNavigationNode = p_navigationNode;
            return l_instance;
        }

        public Entity SourceEntity;
        public NavigationNode TargetNavigationNode;

        public override void Execute(EventQueue p_eventQueue)
        {
            Start();
            float l_costToMove = _ActionPoint.Calculations.actionPointBetweenNavigationNodes(SourceEntity.CurrentNavigationNode, TargetNavigationNode);
            ActionPoint l_actionPoint = EntityComponent.get_component<ActionPoint>(SourceEntity);
            if (l_actionPoint.ActionPointData.CurrentActionPoints >= l_costToMove)
            {
                EntityComponent.get_component<Locomotion>(SourceEntity).MoveToNavigationNode.Invoke(TargetNavigationNode, (p_startNavigationNode, p_endNavigationNode) =>
                {
                    ActionPoint.add(l_actionPoint, -1 * l_costToMove);
                    Entity.set_currentNavigationNode(SourceEntity, p_endNavigationNode);
                    Complete();
                });
            }
            else
            {
                Complete();
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
            Entity.set_currentNavigationNode(Entity, TargetNavigationNode);
        }
    }
}
