using _ActionPoint;
using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _AI._DecisionTree._Builder;
using _Entity._Events;
using _EventQueue;
using System.Collections;

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
            // We insert the EndTurn event just to be sure that end will effectively occur.
            EventQueue.insertEventAt(p_eventQueue, 0, EndEntityTurnEvent.alloc(Entity));


            ActionPoint.resetActionPoints(EntityComponent.get_component<ActionPoint>(Entity));

            DecisionTree l_decisionTree = DecisionTree.alloc();
            TreeBuilder.buildAggressiveTree(l_decisionTree, Entity);
            var l_choice = Algorithm.traverseDecisionTree(l_decisionTree, Entity);

            int l_eventInsersionIndex = 0;

            for(int i = 0; i < l_choice.DecisionNodesChoiceOrdered.Length; i++)
            {
                ADecisionNode l_decisionNode = l_choice.DecisionNodesChoiceOrdered[i];
                if(l_decisionNode.DecisionNodeConsumerAction == EDecisionNodeConsumerAction.EXECUTE)
                {
                    switch (l_decisionNode)
                    {
                        case MoveToNavigationNodeNode l_moveToNavigationNode:

                            var l_pathEnumerator = l_moveToNavigationNode.CalculatedPath.GetEnumerator();
                            while (l_pathEnumerator.MoveNext())
                            {
                                EventQueue.insertEventAt(p_eventQueue, l_eventInsersionIndex, NavigationNodeMoveEntityEvent.alloc(Entity, l_pathEnumerator.Current));
                                l_eventInsersionIndex += 1;
                            }
                            break;

                        case AttackNode l_attackNode:

                            EventQueue.insertEventAt(p_eventQueue, l_eventInsersionIndex, AttackEntityEvent.alloc(l_attackNode.SourceEntity, l_attackNode.TargetEntity, l_attackNode.Attack));
                            l_eventInsersionIndex += 1;
                            break;
                    }
                }
            }
        }
    }

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

