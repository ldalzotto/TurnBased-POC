using _ActionPoint;
using _Entity;
using _Entity._Events;
using _EventQueue;
using _GameLoop;
using _Locomotion;
using _NavigationEngine;
using _NavigationGraph;
using _TurnTimeline;
using _Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;

public class NavigationTest
{
    public static EventQueue TestEventQueue;

    [SetUp]
    public void Before()
    {
        TestEventQueue = EventQueue.alloc();
        ExternalHooks.LogDebug = (string s) => { };
        TurnTimeline.alloc();
        NavigationEngine.alloc();
        NavigationGraph l_navigationGraph = NavigationGraph.alloc();
        NavigationGraphContainer.UniqueNavigationGraph = l_navigationGraph;
        NavigationGraphBuild.CreateGridNavigation(l_navigationGraph, 4, 4);
        NavigationGraph.takeSnapshot(l_navigationGraph);
    }

    [TearDown]
    public void After()
    {
        foreach (Entity l_entity in EntityContainer.Entities)
        {
            Entity.markForDestruction(l_entity);
            EventQueue.enqueueEvent(TestEventQueue, EntityDestroyEvent.alloc(l_entity));
        }

        EventQueue.iterate(TestEventQueue);
        EventQueue.clearAll(TestEventQueue);
        _TurnTimeline.TurnTimeline.free(_TurnTimeline.TurnTimelineContainer.UniqueTurnTimeline);
        NavigationEngine.free(NavigationEngineContainer.UniqueNavigationEngine);
        NavigationGraph.free(NavigationGraphContainer.UniqueNavigationGraph);
        TestEventQueue = null;
    }

    class NavigationEngineTestTriggerComponent : AEntityComponent, INavigationTriggerComponent
    {
        public bool IsTriggered;
        public Entity TriggeredEntity;
        public Action<Entity, List<AEvent>> OnTriggerEnterHook; 
        public static NavigationEngineTestTriggerComponent alloc(Action<Entity, List<AEvent>> p_onTriggerEnterHook = null)
        {
            NavigationEngineTestTriggerComponent l_instance = new NavigationEngineTestTriggerComponent();
            l_instance.IsTriggered = false;
            l_instance.OnTriggerEnterHook = p_onTriggerEnterHook;
            return l_instance;
        }

        public void OnTriggerEnter(Entity p_other, List<AEvent> p_producedEventsStack)
        {
            IsTriggered = true;
            TriggeredEntity = p_other;
            OnTriggerEnterHook?.Invoke(p_other, p_producedEventsStack);
        }
    }

    [Test]
    public void TriggerComponentTest_warp()
    {
        // Initializing trigger Entity
        Entity l_testTriggerEntity = Entity.alloc();

        EntityComponent.add_component(l_testTriggerEntity, NavigationEngineTestTriggerComponent.alloc());
        EntityComponent.add_component(l_testTriggerEntity, Locomotion.alloc(Locomotion.EMPTY_MOVE_TO_NAVIGATION_NODE, Locomotion.EMPTY_WARP));

        // We warp the l_testTriggeredEntity to a random NavigationNode
        EventQueue.enqueueEvent(
          TestEventQueue,
          NavigationNodeWarpEntityEvent.alloc(
              l_testTriggerEntity,
              NavigationGraphAlgorithm.pickRandomNode(NavigationGraphContainer.UniqueNavigationGraph)
          )
        );

        EventQueue.iterate(TestEventQueue);

        // We create the Entity that will be involved in the trigger
        Entity l_entity = Entity.alloc();
        EntityComponent.add_component(l_entity, Locomotion.alloc(Locomotion.EMPTY_MOVE_TO_NAVIGATION_NODE, Locomotion.EMPTY_WARP));

        EventQueue.enqueueEvent(
            TestEventQueue,
            NavigationNodeWarpEntityEvent.alloc(
                l_entity,
                l_testTriggerEntity.CurrentNavigationNode
            )
        );

        EventQueue.iterate(TestEventQueue);


        NavigationEngineTestTriggerComponent l_triggerComponent = EntityComponent.get_component<NavigationEngineTestTriggerComponent>(l_testTriggerEntity);

        Assert.IsTrue(l_triggerComponent.IsTriggered);
        Assert.IsTrue(l_triggerComponent.TriggeredEntity == l_entity);
    }

    [Test]
    public void TriggerComponentTest_navigationNodeMove()
    {
        // Initializing trigger Entity
        Entity l_testTriggerEntity = Entity.alloc();

        EntityComponent.add_component(l_testTriggerEntity, NavigationEngineTestTriggerComponent.alloc());
        EntityComponent.add_component(l_testTriggerEntity, Locomotion.alloc(Locomotion.EMPTY_MOVE_TO_NAVIGATION_NODE, Locomotion.EMPTY_WARP));

        // We warp the l_testTriggeredEntity to a random NavigationNode
        EventQueue.enqueueEvent(
          TestEventQueue,
          NavigationNodeWarpEntityEvent.alloc(
              l_testTriggerEntity,
              NavigationGraphAlgorithm.pickRandomNode(NavigationGraphContainer.UniqueNavigationGraph)
          )
        );

        EventQueue.iterate(TestEventQueue);

        // We create the Entity that will be involved in the trigger
        Entity l_entity = Entity.alloc();
        EntityComponent.add_component(l_entity, Locomotion.alloc(Locomotion.EMPTY_MOVE_TO_NAVIGATION_NODE, Locomotion.EMPTY_WARP));

        ActionPointData l_actionPointdata = new ActionPointData() { InitialActionPoints = 999f, CurrentActionPoints = 999f };
        EntityComponent.add_component(l_entity, ActionPoint.alloc(ref l_actionPointdata));

        var l_entityNavigationNodeEnumerator =
              NavigationGraphAlgorithm.getReachableNeighborNavigationNodes(NavigationGraphContainer.UniqueNavigationGraph, l_testTriggerEntity.CurrentNavigationNode, NavigationGraphFlag.CURRENT).GetEnumerator();
        l_entityNavigationNodeEnumerator.MoveNext();

        EventQueue.enqueueEvent(
           TestEventQueue,
           NavigationNodeWarpEntityEvent.alloc(
               l_entity,
               l_entityNavigationNodeEnumerator.Current
           )
         );


        NavigationNodeMoveEvent l_navigationNodeMoveEvent = NavigationNodeMoveEvent.alloc(l_entity, l_testTriggerEntity.CurrentNavigationNode);
        EventQueue.enqueueEvent(
            TestEventQueue,
            l_navigationNodeMoveEvent
        );

        while (!l_navigationNodeMoveEvent.IsCompleted())
        {
            EventQueue.iterate(TestEventQueue);
        }

        NavigationEngineTestTriggerComponent l_triggerComponent = EntityComponent.get_component<NavigationEngineTestTriggerComponent>(l_testTriggerEntity);

        Assert.IsTrue(l_triggerComponent.IsTriggered);
        Assert.IsTrue(l_triggerComponent.TriggeredEntity == l_entity);
    }

}
