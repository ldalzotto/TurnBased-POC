using System.Collections;
using System;
using _Health;
using _Navigation;
using _ActionPoint;
using _Locomotion;

namespace _Entity._Action
{

    /// <summary>
    /// An <see cref="IEntityAction"/> is an indivisible event that occurs during the turn of an <see cref="Entity"/>.
    /// The logic of an <see cref="IEntityAction"/> is a unique function that can can write to any <see cref="Entity"/>.
    /// The entry point of an <see cref="IEntityAction"/> is a unique function.
    /// When the <see cref="IEntityAction"/> is ended (that may not be instant because of async operations or animations), the <see cref="Action{EntityActionResultAction}"/> callback
    /// is called.
    /// </summary>
    public interface IEntityAction
    {
        Action<Action<EntityActionResultAction>> GetEntityAction();
    }

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
                if (l_actionPoint.ActionPointData.CurrentActionPoints  >= l_costToMove)
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


    public struct EntityDestroyAction : IEntityAction
    {
        public static EntityDestroyAction build(Entity p_entityToDestroy)
        {
            EntityDestroyAction l_instance = new EntityDestroyAction();
            l_instance.EntityToDestroy = p_entityToDestroy;
            return l_instance;
        }

        public Entity EntityToDestroy;

        public Action<Action<EntityActionResultAction>> GetEntityAction()
        {
            EntityDestroyAction thiz = this;
            return (Action<EntityActionResultAction> p_onEndCallback) =>
            {
                Entity.destroyEntity(thiz.EntityToDestroy);
                p_onEndCallback.Invoke(EntityActionResultAction.OK);
            };
        }
    }

    public struct HealthReductionTestAction : IEntityAction
    {
        public static HealthReductionTestAction build(Entity p_entity)
        {
            HealthReductionTestAction l_instance = new HealthReductionTestAction();
            l_instance.Entity = p_entity;
            return l_instance;
        }

        public Entity Entity;

        public Action<Action<EntityActionResultAction>> GetEntityAction()
        {
            HealthReductionTestAction thiz = this;
            return (Action<EntityActionResultAction> p_onEndCallback) =>
            {
                Health.addToCurrentHealth(
                    EntityComponent.get_component<Health>(thiz.Entity),
                    -1.0f
                );
                p_onEndCallback.Invoke(EntityActionResultAction.OK);
            };
        }
    }

}