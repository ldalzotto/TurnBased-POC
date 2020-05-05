using _Entity;
using _Entity._Events;
using _EventQueue;
using _Functional;
using _Navigation._Modifier;
using _NavigationGraph;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public static class NavigationEngineContainer
    {
        public static NavigationEngine UniqueNavigationEngine;
    }

    /// <summary>
    /// The <see cref="NavigationEngine"/> is responsible of logic executed when an <see cref="Entity"/> has it's <see cref="Entity.CurrentNavigationNode"/> changed.
    /// When this occurs, the <see cref="NavigationEngine"/> triggers event based on the which <see cref="Entity"/> there is at the next <see cref="NavigationNode"/>.
    /// </summary>
    public class NavigationEngine
    {
        public EntitiesIndexedByNavigationNodes EntitiesIndexedByNavigationNodes;

        public static NavigationEngine alloc()
        {
            NavigationEngine l_instance = new NavigationEngine();
            l_instance.EntitiesIndexedByNavigationNodes = EntitiesIndexedByNavigationNodes.build(l_instance);

            MyEvent<AEntityComponent>.IEventCallback l_onNavigationModifierComponentDetachedCallback = new ObstacleStep.OnNavigationModifierComponentDetached();
            EntityComponentContainer.registerComponentRemovedEvent<NavigationModifier>(ref l_onNavigationModifierComponentDetachedCallback);

            NavigationEngineContainer.UniqueNavigationEngine = l_instance;
            return l_instance;
        }

        public static void free(NavigationEngine p_navigationEngine)
        {
            EntitiesIndexedByNavigationNodes.free(ref p_navigationEngine.EntitiesIndexedByNavigationNodes);
            if (NavigationEngineContainer.UniqueNavigationEngine == p_navigationEngine) { NavigationEngineContainer.UniqueNavigationEngine = null; };
        }

        public static void ResolveEntityNavigationNodeChange(NavigationEngine p_navigationEngine,
                                    Entity p_entity, NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            ObstacleStep.ResolveNavigationObstacleAlterations(p_entity, p_oldNavigationNode, p_newNavigationNode);
        }
    }

    public struct EntitiesIndexedByNavigationNodes
    {
        public Dictionary<NavigationNode, List<Entity>> Entities;

        private MyEvent<Entity>.IEventCallback OnEntityCreatedListener;
        private OnEntityDestroyed OnEntityDestroyedListener;

        public static EntitiesIndexedByNavigationNodes build(NavigationEngine p_navigationEngine)
        {
            EntitiesIndexedByNavigationNodes l_instance = new EntitiesIndexedByNavigationNodes();
            l_instance.Entities = new Dictionary<NavigationNode, List<Entity>>();
            l_instance.OnEntityCreatedListener = OnEntityCreated.build(p_navigationEngine);
            initializeEntities(ref l_instance);

            MyEvent<Entity>.register(ref EntityContainer.OnEntityCreated, ref l_instance.OnEntityCreatedListener);

            l_instance.OnEntityDestroyedListener = OnEntityDestroyed.alloc(p_navigationEngine);
            EventQueueListener.registerEvent(EventQueueContainer.EventQueueListener, l_instance.OnEntityDestroyedListener);

            return l_instance;
        }

        public static void free(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes)
        {
            MyEvent<Entity>.unRegister(ref EntityContainer.OnEntityCreated, p_entitiesIndexedByNavigationNodes.OnEntityCreatedListener.Handle);
            EventQueueListener.registerEvent(EventQueueContainer.EventQueueListener, p_entitiesIndexedByNavigationNodes.OnEntityDestroyedListener);
            p_entitiesIndexedByNavigationNodes.Entities.Clear();
        }

        public static void initializeEntities(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes)
        {
            for (int i = 0; i < EntityContainer.Entities.Count; i++)
            {
                Entity l_entity = EntityContainer.Entities[i];
                addEntityToNavigationNode(ref p_entitiesIndexedByNavigationNodes, l_entity, l_entity.CurrentNavigationNode);
            }
        }

        private struct OnEntityCreated : MyEvent<Entity>.IEventCallback
        {
            public int Handle { get; set; }

            public NavigationEngine NavigationEngine;

            public static OnEntityCreated build(NavigationEngine p_navigationEngine)
            {
                OnEntityCreated l_instance = new OnEntityCreated();
                l_instance.NavigationEngine = p_navigationEngine;
                return l_instance;
            }

            public EventCallbackResponse Execute(ref Entity p_entity)
            {
                return EventCallbackResponse.OK;
            }
        }

        private class OnEntityDestroyed : AEventListener<EntityDestroyEvent>
        {
            public NavigationEngine NavigationEngine;
            public static OnEntityDestroyed alloc(NavigationEngine p_navigationEngine)
            {
                OnEntityDestroyed l_instance = new OnEntityDestroyed();
                l_instance.NavigationEngine = p_navigationEngine;
                return l_instance;
            }

            public override void OnEventExecuted(EventQueue p_eventQueue, EntityDestroyEvent p_event)
            {
                removeEntityToNavigationNode(ref NavigationEngine.EntitiesIndexedByNavigationNodes, p_event.EntityToDestroy, p_event.EntityToDestroy.CurrentNavigationNode);
            }
        }

        private static void addEntityToNavigationNode(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes, Entity p_entity, NavigationNode p_navigationNode)
        {
            if (!p_entitiesIndexedByNavigationNodes.Entities.ContainsKey(p_navigationNode))
            {
                p_entitiesIndexedByNavigationNodes.Entities.Add(p_navigationNode, new List<Entity>());
            }

            List<Entity> l_navigationNodeEntities = p_entitiesIndexedByNavigationNodes.Entities[p_navigationNode];
            if (!l_navigationNodeEntities.Contains(p_entity))
            {
                l_navigationNodeEntities.Add(p_entity);
            }
        }

        private static void removeEntityToNavigationNode(ref EntitiesIndexedByNavigationNodes p_entitiesIndexedByNavigationNodes, Entity p_entity, NavigationNode p_navigationNode)
        {
            if (p_entitiesIndexedByNavigationNodes.Entities.ContainsKey(p_navigationNode))
            {
                p_entitiesIndexedByNavigationNodes.Entities[p_navigationNode].Remove(p_entity);
            }
        }

    }
}
