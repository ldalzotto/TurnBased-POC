using _Entity;
using _EventQueue;
using System.Collections.Generic;

namespace _NavigationEngine
{
    public interface INavigationTriggerComponent
    {
        void OnTriggerEnter(Entity p_other, List<AEvent> p_producedEventsStack);
    }
}

