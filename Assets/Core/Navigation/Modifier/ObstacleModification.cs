using System.Collections;
using _Entity;
using _Functional;

namespace _Navigation._Modifier
{
    public static class ObstacleModification
    {
        static ObstacleModification()
        {
            MyEvent<AEntityComponent>.IEventCallback l_callback = new OnNavigationModifierComponentDetached();
            EntityComponentContainer.registerComponentRemovedEvent<NavigationModifier>(ref l_callback);
        }

        public static void onNavigationNodeChanged(NavigationModifier p_navigationModifier, NavigationNode p_oldNavigationNode, NavigationNode p_newNavigationNode)
        {
            if (p_oldNavigationNode != p_newNavigationNode)
            {
                if (p_navigationModifier != null && p_navigationModifier.NavigationModifierData.IsObstacle)
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

        struct OnNavigationModifierComponentDetached : MyEvent<AEntityComponent>.IEventCallback
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
