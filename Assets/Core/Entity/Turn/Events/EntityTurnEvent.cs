﻿using _ActionPoint;
using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _AI._DecisionTree._Builder;
using _Entity._Events;
using _EventQueue;
using System.Threading.Tasks;
using static _AI._DecisionTree._Algorithm.Algorithm;

namespace _Entity._Turn
{

    public class StartEntityTurnEvent : AEvent
    {
        public Entity Entity;

        public static StartEntityTurnEvent alloc(Entity p_entity)
        {
            StartEntityTurnEvent l_instance = new StartEntityTurnEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {


            int l_eventQueueSizeBeforeInsersion = p_eventQueue.Events.Count;
            if (EntityComponent.get_component<ActionPoint>(Entity).ActionPointData.CurrentActionPoints > 0.00f)
            {

                DecisionTree l_decisionTree = DecisionTree.alloc();
                TreeBuilder.buildAggressiveTree(l_decisionTree, Entity);
                var l_choice = Algorithm.traverseDecisionTree(l_decisionTree, Entity);


                for (int i = 0; i < l_choice.PickedChoice.DecisionNodesChoiceOrdered.Length; i++)
                {
                    ADecisionNode l_decisionNode = l_choice.PickedChoice.DecisionNodesChoiceOrdered[i];
                    if (l_decisionNode.DecisionNodeConsumerAction == EDecisionNodeConsumerAction.EXECUTE)
                    {
                        switch (l_decisionNode)
                        {
                            case MoveToNavigationNodeNode l_moveToNavigationNode:

                                var l_pathEnumerator = l_moveToNavigationNode.CalculatedPath.GetEnumerator();
                                while (l_pathEnumerator.MoveNext())
                                {
                                    EventQueue.enqueueEvent(p_eventQueue, NavigationNodeMoveEvent.alloc(Entity, l_pathEnumerator.Current));
                                }
                                break;

                            case AttackNode l_attackNode:

                                for (int j = 0; j < l_attackNode.NumberOfAttacks; j++)
                                {
                                    EventQueue.enqueueEvent(p_eventQueue, AttackEntityEvent.alloc(l_attackNode.SourceEntity, l_attackNode.TargetEntity, l_attackNode.Attack));
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
                EventQueue.enqueueEvent(p_eventQueue, StartEntityTurnEvent.alloc(Entity));
            }
            else
            {
                EventQueue.enqueueEvent(p_eventQueue, EndEntityTurnEvent.alloc(Entity));
            }
        }
    }

    /// <summary>
    /// This event is used only to trigger Event hooks.
    /// </summary>
    public class EndEntityTurnEvent : AEvent
    {
        public Entity Entity;

        public static EndEntityTurnEvent alloc(Entity p_entity)
        {
            EndEntityTurnEvent l_instance = new EndEntityTurnEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }
    }
}


#if comment

    // The non blocking version of StartEntityTurnEvent

    public class StartEntityTurnEvent : AAsyncEvent
    {
        public Entity Entity;
        private Task AsyncTask;
        private AIDecisionTreeChoice AIChoice;
        public static StartEntityTurnEvent alloc(Entity p_entity)
        {
            StartEntityTurnEvent l_instance = new StartEntityTurnEvent();
            l_instance.Entity = p_entity;
            l_instance.AsyncTask = null;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            AsyncTask = Task.Factory.StartNew(() =>
            {
                if (EntityComponent.get_component<ActionPoint>(Entity).ActionPointData.CurrentActionPoints > 0.00f)
                {

                    DecisionTree l_decisionTree = DecisionTree.alloc();
                    TreeBuilder.buildAggressiveTree(l_decisionTree, Entity);
                    AIChoice = Algorithm.traverseDecisionTree(l_decisionTree, Entity);
                }
            });
        }

        public override void OnCompleted(EventQueue p_eventQueue)
        {
            base.OnCompleted(p_eventQueue);

            int l_eventQueueSizeBeforeInsersion = p_eventQueue.Events.Count;

            if (AIChoice.DecisionNodesChoiceOrdered != null)
            {
                for (int i = 0; i < AIChoice.DecisionNodesChoiceOrdered.Length; i++)
                {
                    ADecisionNode l_decisionNode = AIChoice.DecisionNodesChoiceOrdered[i];
                    if (l_decisionNode.DecisionNodeConsumerAction == EDecisionNodeConsumerAction.EXECUTE)
                    {
                        switch (l_decisionNode)
                        {
                            case MoveToNavigationNodeNode l_moveToNavigationNode:

                                var l_pathEnumerator = l_moveToNavigationNode.CalculatedPath.GetEnumerator();
                                while (l_pathEnumerator.MoveNext())
                                {
                                    EventQueue.enqueueEvent(p_eventQueue, NavigationNodeMoveEvent.alloc(Entity, l_pathEnumerator.Current));
                                }
                                break;

                            case AttackNode l_attackNode:

                                for (int j = 0; j < l_attackNode.NumberOfAttacks; j++)
                                {
                                    EventQueue.enqueueEvent(p_eventQueue, AttackEntityEvent.alloc(l_attackNode.SourceEntity, l_attackNode.TargetEntity, l_attackNode.Attack));
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
                EventQueue.enqueueEvent(p_eventQueue, StartEntityTurnEvent.alloc(Entity));
            }
            else
            {
                EventQueue.enqueueEvent(p_eventQueue, EndEntityTurnEvent.alloc(Entity));
            }
        }

        public override bool IsCompleted()
        {
            return AsyncTask != null && AsyncTask.IsCompleted;
        }
    }

#endif
