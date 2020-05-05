using _Entity;

namespace _AI._DecisionTree
{
    /// <summary>
    /// Usually linked to MoveToNavigationNodeNode for every reachable neighbors(m_neighborNavigationNodes) of the <see cref="TargetEntity"/>.
    /// It describes the intend to get in range of another <see cref="Entity"/>.
    /// </summary>
    public class MoveToEntityNode : ADecisionNode
    {
        public Entity SourceEntity;
        public Entity TargetEntity;

        public static MoveToEntityNode alloc(Entity p_sourceEntity, Entity p_targetEntity)
        {
            MoveToEntityNode l_instance = new MoveToEntityNode();
            l_instance.SourceEntity = p_sourceEntity;
            l_instance.TargetEntity = p_targetEntity;
            return l_instance;
        }
    };

}
