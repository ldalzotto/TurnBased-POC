using _Entity;
using _Navigation._Modifier;
using _NavigationGraph;

namespace _NavigationEngine
{
    public static class ObstacleStep
    {

        /// <summary>
        /// If the <paramref name="p_entity"/> has a <see cref="NavigationModifier"/> component, then the fact that he has moved involves that the 
        /// <see cref="NavigationGraph"/> layout is no more valid. <see cref="NavigationLink"/> must be recomputed to take into account the change.
        /// </summary>
        /// 
        /// <param name="p_newNavigationNode"> 
        /// The next <see cref="NavigationNode"/> where the <paramref name="p_entity"/> will be.
        /// This value can be null, meaning that the <paramref name="p_entity"/>"s <see cref="NavigationModifier"/> component has been detached.
        /// </param>
        public static void resolveNavigationObstacleAlterations(NavigationEngine p_navigationEngine, Entity p_entity, NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            if (p_oldNavigationNode != p_newNavigationNode)
            {
                NavigationModifier l_entityNavigationModifier = EntityComponent.get_component<NavigationModifier>(p_entity);
                if (l_entityNavigationModifier != null && l_entityNavigationModifier.NavigationModifierData.IsObstacle)
                {
                    if (p_newNavigationNode != null)
                    {

                        if (p_oldNavigationNode != null)
                        {
                            // We restore p_oldNavigationNode NavigationLinks only if there is no more NavigationModifier.Obstacle
                            if (!EntityQuery.isThereAtLeastOfComponentOfType<NavigationModifier>(ref p_navigationEngine.EntitiesIndexedByNavigationNodes, p_oldNavigationNode, NavigationModifier.IsObstacle))
                            {
                                NavigationLinkAlteration.restoreNavigationLinksFromSnapshot(NavigationGraphContainer.UniqueNavigationGraph, NavigationLinkAlteration.ENavigationLinkAlterationMethod.TO,
                                       p_oldNavigationNode);
                            }
                        }

                        NavigationLinkAlteration.removeNavigationLinks(NavigationGraphContainer.UniqueNavigationGraph, NavigationLinkAlteration.ENavigationLinkAlterationMethod.TO,
                            p_newNavigationNode);

                    }
                    else
                    {
                        // We restore p_oldNavigationNode NavigationLinks only if there is no more NavigationModifier.Obstacle
                        if (!EntityQuery.isThereAtLeastOfComponentOfType<NavigationModifier>(ref p_navigationEngine.EntitiesIndexedByNavigationNodes, p_oldNavigationNode, NavigationModifier.IsObstacle))
                        {
                            NavigationLinkAlteration.restoreNavigationLinksFromSnapshot(NavigationGraphContainer.UniqueNavigationGraph, NavigationLinkAlteration.ENavigationLinkAlterationMethod.TO,
                                   p_oldNavigationNode);
                        }
                    }
                }
            }
        }
    }
}
