using System.Collections;
using _EventQueue;
using _Health;

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

#if comment

    /// <summary>
    /// The <see cref="NavigationNodeMoveEntityAction"/> moves the <see cref="SourceEntity"/> to the <see cref="NavigationNode"/> <see cref="TargetNavigationNode"/>.
    /// Consumes <see cref="ActionPoint"/>.
    /// </summary>
    public struct NavigationNodeMoveEntityAction : IEntityAction
    {
        public NavigationNodeMoveEntityAction(Entity p_sourceEntity, NavigationNode p_navigationNode)
        {
            SourceEntity = p_sourceEntity;
            TargetNavigationNode = p_navigationNode;
        }

        public Entity SourceEntity;
        public NavigationNode TargetNavigationNode;

        public Action<Action<EntityActionResultAction>> GetEntityAction()
        {
            NavigationNodeMoveEntityAction thiz = this;
            return (Action<EntityActionResultAction> p_onEndCallback) =>
            {
                float l_costToMove = Calculations.actionPointBetweenNavigationNodes(thiz.SourceEntity.CurrentNavigationNode,
                            thiz.TargetNavigationNode);

                ActionPoint l_actionPoint = EntityComponent.get_component<ActionPoint>(thiz.SourceEntity);
                if (l_actionPoint.ActionPointData.CurrentActionPoints >= l_costToMove)
                {
                    EntityComponent.get_component<Locomotion>(thiz.SourceEntity).MoveToNavigationNode.Invoke(thiz.TargetNavigationNode, (p_startNavigationNode, p_endNavigationNode) =>
                    {
                        ActionPoint.add(l_actionPoint, -1 * l_costToMove);
                        p_onEndCallback.Invoke(EntityActionResultAction.OK);
                    });
                }
                else
                {
                    p_onEndCallback.Invoke(EntityActionResultAction.OK);
                }
            };
        }
    }

#endif
}
