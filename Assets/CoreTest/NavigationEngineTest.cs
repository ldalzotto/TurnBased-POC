﻿using _ActionPoint;
using _Attack;
using _Entity;
using _Entity._Events;
using _EntityCharacteristics;
using _EventQueue;
using _GameLoop;
using _Health;
using _HealthRecovery;
using _Locomotion;
using _NavigationEngine;
using _NavigationGraph;
using _TurnTimeline;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NavigationTest
{
    public static EventQueue TestEventQueue;

    [SetUp]
    public void Before()
    {
        TestEventQueue = EventQueue.alloc();
        ExternalHooks.LogDebug = (string s) => { };
        _TurnTimeline.TurnTimeline.alloc();
        NavigationEngine.alloc();
        NavigationGraph l_navigationGraph = NavigationGraph.alloc();
        NavigationGraphContainer.UniqueNavigationGraph = l_navigationGraph;
        CreateGridNavigation(l_navigationGraph, 4, 4);
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

    private void CreateGridNavigation(NavigationGraph p_navigationGraph, int XNumber, int ZNumber)
    {
        CreateGridNavigationStructure(XNumber, ZNumber, out Dictionary<int, Vector3> l_navigationNodes, out List<GridNavigationLink> l_navigationLinks);

        List<NavigationNode> l_instancedNavigationNodes = new List<NavigationNode>(l_navigationNodes.Count);

        for (int i = 0; i < l_navigationNodes.Count; i++)
        {
            NavigationNode l_instancedNavigationNode = NavigationGraph.instanciateAndAddNode(p_navigationGraph);
            l_instancedNavigationNode.LocalPosition = l_navigationNodes[i];
            l_instancedNavigationNodes.Add(l_instancedNavigationNode);
        }

        foreach (GridNavigationLink l_link in l_navigationLinks)
        {
            NavigationGraph.createLinkBetween(p_navigationGraph, l_instancedNavigationNodes[l_link.StartNode], l_instancedNavigationNodes[l_link.EndNode],
                    math.distance(l_instancedNavigationNodes[l_link.StartNode].LocalPosition, l_instancedNavigationNodes[l_link.EndNode].LocalPosition));
        }
    }

    struct GridNavigationLink
    {
        public int StartNode;
        public int EndNode;
    }

    private void CreateGridNavigationStructure(int XNumber, int ZNumber, out Dictionary<int, Vector3> p_navigationNodes, out List<GridNavigationLink> p_navigationLinks)
    {
        int currentKey = 0;

        Dictionary<int, Dictionary<int, int>> l_XandZtoKeyLookupTable = new Dictionary<int, Dictionary<int, int>>();
        p_navigationNodes = new Dictionary<int, Vector3>();
        p_navigationLinks = new List<GridNavigationLink>();

        // Node creation
        for (int x = 0; x < XNumber; x++)
        {
            if (!l_XandZtoKeyLookupTable.ContainsKey(x)) { l_XandZtoKeyLookupTable[x] = new Dictionary<int, int>(); }

            for (int z = 0; z < ZNumber; z++)
            {
                p_navigationNodes.Add(currentKey, new Vector3(x, 0, z));
                l_XandZtoKeyLookupTable[x][z] = currentKey;
                currentKey += 1;
            }
        }

        for (int x = 0; x < XNumber; x++)
        {
            for (int z = 0; z < ZNumber; z++)
            {
                if (l_XandZtoKeyLookupTable.ContainsKey(x + 1))
                {
                    p_navigationLinks.Add(new GridNavigationLink()
                    {
                        StartNode = l_XandZtoKeyLookupTable[x][z],
                        EndNode = l_XandZtoKeyLookupTable[x + 1][z]
                    });
                    p_navigationLinks.Add(new GridNavigationLink()
                    {
                        StartNode = l_XandZtoKeyLookupTable[x + 1][z],
                        EndNode = l_XandZtoKeyLookupTable[x][z]
                    });
                }

                if (l_XandZtoKeyLookupTable[x].ContainsKey(z - 1))
                {
                    p_navigationLinks.Add(new GridNavigationLink()
                    {
                        StartNode = l_XandZtoKeyLookupTable[x][z],
                        EndNode = l_XandZtoKeyLookupTable[x][z - 1]
                    });
                    p_navigationLinks.Add(new GridNavigationLink()
                    {
                        StartNode = l_XandZtoKeyLookupTable[x][z - 1],
                        EndNode = l_XandZtoKeyLookupTable[x][z]
                    });
                }
            }
        }
    }

    [Test]
    public void NavigationTriggerComponentTest()
    {
        // Health trigger
        Entity l_healthTrigger = Entity.alloc();

        HealthRecoveryData l_healthRecoveryData = new HealthRecoveryData() { RecoveredHealth = 10.0f };
        EntityComponent.add_component(l_healthTrigger, HealthRecoveryTrigger.alloc(l_healthRecoveryData));
        EntityComponent.add_component(l_healthTrigger, Locomotion.alloc(Locomotion.EMPTY_MOVE_TO_NAVIGATION_NODE, Locomotion.EMPTY_WARP));

        EventQueue.enqueueEvent(
          TestEventQueue,
          NavigationNodeWarpEntityEvent.alloc(
              l_healthTrigger,
              NavigationGraphAlgorithm.getNearestNode(
                  NavigationGraphContainer.UniqueNavigationGraph,
                  new float3(1.0f, 0.0f, 1.0f)
                  )
          )
      );

        EventQueue.iterate(TestEventQueue);

        //

        Entity l_entity1 = Entity.alloc();
        EntityCharacteristicsData l_entityCharacteristicsData = new EntityCharacteristicsData() { Speed = 20.0f };
        TurnTimelineElligibilityData l_turnTimelineElligibilityData = TurnTimelineElligibilityData.build(true);
        ActionPointData l_actionPointData = new ActionPointData() { InitialActionPoints = 3.0f };
        AttackData l_attackData = new AttackData() { APCost = 1.0f, Damage = 1.0f };
        HealthData l_healthData = new HealthData() { MaxHealth = 100.0f, CurrentHealth = 100.0f };

        EntityComponent.add_component(l_entity1, EntityCharacteristics.alloc(ref l_entityCharacteristicsData));
        EntityComponent.add_component(l_entity1, TurnTimelineElligibility.alloc(ref l_turnTimelineElligibilityData));
        EntityComponent.add_component(l_entity1, ActionPoint.alloc(ref l_actionPointData));
        EntityComponent.add_component(l_entity1, Locomotion.alloc(Locomotion.EMPTY_MOVE_TO_NAVIGATION_NODE, Locomotion.EMPTY_WARP));
        EntityComponent.add_component(l_entity1, Attack.alloc(ref l_attackData));
        EntityComponent.add_component(l_entity1, Health.alloc(ref l_healthData));

        EventQueue.enqueueEvent(
            TestEventQueue,
            NavigationNodeWarpEntityEvent.alloc(
                l_entity1,
                l_healthTrigger.CurrentNavigationNode
            )
        );

        EventQueue.iterate(TestEventQueue);

        Assert.IsTrue(EntityComponent.get_component<Health>(l_entity1).HealthData.CurrentHealth == 110.0f);
        Assert.IsTrue(!EntityContainer.Entities.Contains(l_healthTrigger));
    }

}