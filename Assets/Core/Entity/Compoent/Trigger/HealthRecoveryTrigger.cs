using _Entity;
using _Entity._Events;
using _EventQueue;
using _GameLoop;
using _Health;
using _NavigationEngine;
using System.Collections.Generic;

namespace _HealthRecovery
{
    public class HealthRecoveryTrigger : AEntityComponent, INavigationTriggerComponent
    {
        public HealthRecoveryData HealthRecoveryData;

        public static HealthRecoveryTrigger alloc(HealthRecoveryData p_healthRecoveryData)
        {
            HealthRecoveryTrigger l_instance = new HealthRecoveryTrigger();
            l_instance.HealthRecoveryData = p_healthRecoveryData;
            return l_instance;
        }

        public void OnTriggerEnter(Entity p_other, List<AEvent> p_producedEventsStack)
        {
            if (EntityComponent.get_component<Health>(p_other) != null)
            {
                ExternalHooks.LogDebug("HealthRecovery_OnTriggerEnter");

                p_producedEventsStack.Add(HealthRecoveryEvent.alloc(p_other, this));
                EntityEventsComposition.addEntityDestroyedEvents(p_producedEventsStack, AssociatedEntity);
            }
        }
    }

    public struct HealthRecoveryData
    {
        public float RecoveredHealth;
    }


    public class HealthRecoveryEvent : AEvent
    {
        public Entity TargetEntity;
        public HealthRecoveryTrigger HealthRecoveryComponent;

        public static HealthRecoveryEvent alloc(Entity p_targetEntity, HealthRecoveryTrigger p_healthRecoveryComponent)
        {
            HealthRecoveryEvent l_instance = new HealthRecoveryEvent();
            l_instance.TargetEntity = p_targetEntity;
            l_instance.HealthRecoveryComponent = p_healthRecoveryComponent;
            return l_instance;
        }

        public override void Execute(EventQueue p_eventQueue)
        {
            base.Execute(p_eventQueue);
            Health.addToCurrentHealth(EntityComponent.get_component<Health>(TargetEntity), HealthRecoveryComponent.HealthRecoveryData.RecoveredHealth);
        }
    }
}
