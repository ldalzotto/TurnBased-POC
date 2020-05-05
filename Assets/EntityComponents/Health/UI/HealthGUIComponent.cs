using _Entity;
using _GameLoop;
using _RuntimeObject;
using _UI._Gauge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Health
{
    public static class HealthGUIComponentContainer
    {
        public static List<HealthGUIComponent> HealthGUIComponents = new List<HealthGUIComponent>();

        static HealthGUIComponentContainer()
        {
            GameLoop.AddGameLoopCallback(GameLoopHook.Tick, new GameLoopCallback()
            {
                GameLoopPriority = GameLoopCallback.EvaluatePriority(null, null),
                Callback = HealthGUIComponentContainer.Tick
            });
        }

        private static void Tick(float d)
        {
            for (int i = 0; i < HealthGUIComponents.Count; i++)
            {
                HealthGUIComponents[i].Tick(d);
            }
        }
    }

    [ExecuteAfter(typeof(EntityRegistrationComponent))]
    public class HealthGUIComponent : RuntimeComponent
    {
        private UIGaugeComponent m_uiGauge;

        private CachedRuntimeComponent<EntityRegistrationComponent> m_entityRegistrationComponent;
        private CachedEntityComponent<Health> healthEntityComponent;
        public override void Awake()
        {
            base.Awake();
            m_uiGauge = transform.GetComponentInChildren<UIGaugeComponent>();
            HealthGUIComponentContainer.HealthGUIComponents.Add(this);
            m_entityRegistrationComponent = CachedRuntimeComponent<EntityRegistrationComponent>.New(RuntimeObject);
        }

        private void Start()
        {
            healthEntityComponent = CachedEntityComponent<Health>.build(m_entityRegistrationComponent.Get().AssociatedEntity);
        }

        public void Tick(float d)
        {
            m_uiGauge.SetFillRate(Health.getHealthRatio(healthEntityComponent.Get()));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            HealthGUIComponentContainer.HealthGUIComponents.Remove(this);
        }
    }


}
