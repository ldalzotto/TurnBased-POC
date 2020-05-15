using _Entity;
using _HealthRecovery;

namespace _AI._DecisionTree
{
    public class HealNode : ADecisionNode
    {
        public Entity SourceEntity;
        public HealthRecoveryTrigger TargetHealTrigger;
        public float RecoveredHealth;

        public static HealNode alloc(Entity p_sourceEntity, HealthRecoveryTrigger p_targetHealTrigger)
        {
            HealNode l_instance = new HealNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetHealTrigger = p_targetHealTrigger;
            l_instance.RecoveredHealth = 0.0f;
            return l_instance;
        }

        public override void TreeTraversal(ADecisionNode p_sourceNode)
        {
            base.TreeTraversal(p_sourceNode);
            RecoveredHealth += TargetHealTrigger.HealthRecoveryData.RecoveredHealth;
        }
    }
}

