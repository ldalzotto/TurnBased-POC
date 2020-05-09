using _Entity;
using _EventQueue;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public interface INavigationTriggerComponent
    {
        /// <summary>
        /// Called when another <see cref="Entity"/> have the same <see cref="Entity.CurrentNavigationNode"/>.
        /// During execution of this trigger event, <see cref="AEvent"/> can be pushed to the calling <see cref="EventQueue"/>. These <see cref="AEvent"/> are stored
        /// in <paramref name="p_producedEventsStack"/>.
        /// </summary>
        void OnTriggerEnter(Entity p_other, List<AEvent> p_producedEventsStack);
    }
}

