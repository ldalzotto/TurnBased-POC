using _Entity;
using _Functional;
using _Navigation._Modifier;
using _NavigationGraph;

namespace _NavigationEngine
{
    public static class ObstacleStep
    {
        public static void ResolveNavigationObstacleAlterations(Entity p_entity, NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            if (p_oldNavigationNode != p_newNavigationNode)
            {
                NavigationModifier l_entityNavigationModifier = EntityComponent.get_component<NavigationModifier>(p_entity);
                if (l_entityNavigationModifier != null && l_entityNavigationModifier.NavigationModifierData.IsObstacle)
                {
                    if (p_oldNavigationNode != null)
                    {
                        NavigationLinkAlteration.restoreNavigationLinksFromSnapshot(NavigationGraphContainer.UniqueNavigationGraph, NavigationLinkAlteration.ENavigationLinkAlterationMethod.TO,
                            p_oldNavigationNode);
                    }

                    NavigationLinkAlteration.removeNavigationLinks(NavigationGraphContainer.UniqueNavigationGraph, NavigationLinkAlteration.ENavigationLinkAlterationMethod.TO,
                        p_newNavigationNode);
                }
            }
        }

        public struct OnNavigationModifierComponentDetached : MyEvent<AEntityComponent>.IEventCallback
        {
            public int Handle { get; set; }

            public EventCallbackResponse Execute(ref AEntityComponent p_component)
            {
                // We check if the navigation graph is not null because the OnNavigationModifierComponentDetached event can be called when the level is unloaded.
                // As we don't know the order of execution from this event from freeing the navigation graph, we check to be sure.
                if (NavigationGraphContainer.UniqueNavigationGraph != null)
                {
                    NavigationModifier l_navigationModifier = p_component as NavigationModifier;
                    if (l_navigationModifier != null && l_navigationModifier.NavigationModifierData.IsObstacle)
                    {
                        if (l_navigationModifier.AssociatedEntity.CurrentNavigationNode != null)
                        {
                            NavigationLinkAlteration.restoreNavigationLinksFromSnapshot(NavigationGraphContainer.UniqueNavigationGraph, NavigationLinkAlteration.ENavigationLinkAlterationMethod.TO,
                                  l_navigationModifier.AssociatedEntity.CurrentNavigationNode);
                        }
                    }
                }

                return EventCallbackResponse.OK;
            }
        }
    }
}
