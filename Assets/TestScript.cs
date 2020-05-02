using _ActionPoint;
using _AI._DecisionTree;
using _AI._DecisionTree._Algorithm;
using _Attack;
using _Entity;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Update()
    {
        Entity l_entity = Entity.alloc();
        ActionPointData l_actionPointData = new ActionPointData() {InitialActionPoints = 3.0f, CurrentActionPoints = 3.0f};
        ActionPoint l_actionPoint = ActionPoint.alloc(ref l_actionPointData);
        EntityComponent.add_component(l_entity, ref l_actionPoint);
        AttackData l_attackData = new AttackData() {APCost = 1.5f, Damage = 1.0f};
        Attack l_attack = Attack.alloc(ref l_attackData);

        DecisionTree l_decisionTree = DecisionTree.alloc();
        AttackNode l_testNode1 = AttackNode.alloc(l_entity, null, l_attack);
        AttackNode l_testNode2 = AttackNode.alloc(l_entity, null, l_attack);
        AttackNode l_testNode3 = AttackNode.alloc(l_entity, null, l_attack);
        DecisionTree.linkDecisionNodes(l_decisionTree, l_decisionTree.RootNode, l_testNode1);
        DecisionTree.linkDecisionNodes(l_decisionTree, l_testNode1, l_testNode2);
        DecisionTree.linkDecisionNodes(l_decisionTree, l_testNode2, l_testNode3);
        ref Algorithm.AIDecisionTreeChoice l_choice = ref Algorithm.traverseDecisionTree(l_decisionTree, l_entity);

        Debug.Log(l_choice);
    }
}