using UnityEngine;
using System.Collections;
using _AI._DecisionTree._Algorithm;
using _Entity;
using _NavigationGraph;
using _HealthRecovery;
using _Health;

namespace _AI._DecisionTree
{
    public class HealNode : ADecisionNode
    {
        public Entity SourceEntity;
        public HealthRecoveryTrigger TargetHealTrigger;

        public static HealNode alloc(Entity p_sourceEntity, HealthRecoveryTrigger p_targetHealTrigger)
        {
            HealNode l_instance = new HealNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetHealTrigger = p_targetHealTrigger;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode, ref Algorithm.EntityDecisionContext p_entityDecisionContextdata)
        {
            base.TreeTraversal(p_sourceNode, ref p_entityDecisionContextdata);
#if comment
            if (NavigationGraphAlgorithm.areNavigationNodesNeighbors(NavigationGraphContainer.UniqueNavigationGraph, SourceEntity.CurrentNavigationNode,
                    TargetHealTrigger.AssociatedEntity.CurrentNavigationNode, NavigationGraphFlag.SNAPSHOT))
            {
#endif
                p_entityDecisionContextdata.AIDecisionScore.HealScore += TargetHealTrigger.HealthRecoveryData.RecoveredHealth;
      //      }
        }
    }
}

