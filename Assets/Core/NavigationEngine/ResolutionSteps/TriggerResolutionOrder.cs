using _Entity;
using System;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public static class TriggerResolutionOrder
    {

        public static Dictionary<Type, float> TriggerComponentResolutionOrder = new Dictionary<Type, float>();

        /// <summary>
        /// Extract all <see cref="INavigationTriggerComponent"/> components from <paramref name="p_requestedEntity"/> and sort them by querying the <see cref="TriggerComponentResolutionOrder"/>.
        /// </summary>
        public static void extractTriggerComponentSortedByExecutionOrder(List<INavigationTriggerComponent> p_extractedComponents, Entity p_requestedEntity)
        {
            p_extractedComponents.Clear();

            var l_components = p_requestedEntity.Components.GetRefEnumerator();
            while (l_components.MoveNext())
            {
                if (l_components.GetCurrentRef().value is INavigationTriggerComponent)
                {
                    p_extractedComponents.Add(l_components.GetCurrentRef().value as INavigationTriggerComponent);
                }
            }

            p_extractedComponents.Sort(ProducedEventOrdererFunction.build(TriggerComponentResolutionOrder));
        }

        public struct ProducedEventOrdererFunction : IComparer<INavigationTriggerComponent>
        {
            public Dictionary<Type, float> TriggerComponentPriorityLookup;

            public static ProducedEventOrdererFunction build(Dictionary<Type, float> p_triggerComponentPriorityLookup)
            {
                ProducedEventOrdererFunction l_instance = new ProducedEventOrdererFunction();
                l_instance.TriggerComponentPriorityLookup = p_triggerComponentPriorityLookup;
                return l_instance;
            }

            public int Compare(INavigationTriggerComponent x, INavigationTriggerComponent y)
            {
                return TriggerComponentPriorityLookup[x.GetType()].CompareTo(TriggerComponentPriorityLookup[y.GetType()]);
            }
        }
    }
}