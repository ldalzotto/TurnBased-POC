using _Entity;
using _Functional;
using _GameLoop;
using _RuntimeObject;
using _UI._Gauge;
using System.Collections.Generic;

namespace _ActionPoint
{
    public static class ActionPointGUIComponentContainer
    {
        public static List<ActionPointGUIComponent> ActionPointGUIComponents = new List<ActionPointGUIComponent>();

        static ActionPointGUIComponentContainer()
        {
            GameLoop.AddGameLoopCallback(GameLoopHook.Tick, new GameLoopCallback()
            {
                GameLoopPriority = Dichotomy.EvaluatePriority(null, null),
                Callback = ActionPointGUIComponentContainer.Tick
            });
        }

        public static void Tick(float d)
        {
            for (int i = 0; i < ActionPointGUIComponents.Count; i++)
            {
                ActionPointGUIComponents[i].Tick(d);
            }
        }
    }



    /// <summary>
    /// The GUI representation of <see cref="ActionPoint"/>.
    /// </summary>
    public class ActionPointGUIComponent : RuntimeComponent
    {
        #region Component dependencies
        private EntityRegistrationComponent m_entityRegistrationComponent;
        private CachedEntityComponent<ActionPoint> cachedActionPointEntityComponent;
        #endregion

        private UIGaugeComponent m_uiGauge;


        public override void Awake()
        {
            base.Awake();
            m_uiGauge = transform.GetComponentInChildren<UIGaugeComponent>();
        }

        private void Start()
        {
            m_entityRegistrationComponent = RuntimeObject.FindComponent<EntityRegistrationComponent>();
            cachedActionPointEntityComponent = CachedEntityComponent<ActionPoint>.build(m_entityRegistrationComponent.AssociatedEntity);

            gameObject.SetActive(false);

            MyEvent<Entity>.IEventCallback l_onTurnStart = OnTurnStart.alloc(this);
            MyEvent<Entity>.IEventCallback l_onTurnEnd = OnTurnEnd.alloc(this);

            MyEvent<Entity>.register(ref m_entityRegistrationComponent.AssociatedEntity.OnEntityTurnStart, ref l_onTurnStart);
            MyEvent<Entity>.register(ref m_entityRegistrationComponent.AssociatedEntity.OnEntityTurnEnd, ref l_onTurnEnd);
        }

        struct OnTurnStart : MyEvent<Entity>.IEventCallback
        {
            public int Handle { get; set; }
            public ActionPointGUIComponent ActionPointGUIComponent;

            public static OnTurnStart alloc(ActionPointGUIComponent p_actionPointGUIComponent)
            {
                OnTurnStart l_instance = new OnTurnStart();
                l_instance.ActionPointGUIComponent = p_actionPointGUIComponent;
                return l_instance;
            }

            public EventCallbackResponse Execute(ref Entity p_param1)
            {
                ActionPointGUIComponent.gameObject.SetActive(true);
                ActionPointGUIComponentContainer.ActionPointGUIComponents.Add(ActionPointGUIComponent);
                return EventCallbackResponse.OK;
            }
        }

        struct OnTurnEnd : MyEvent<Entity>.IEventCallback
        {
            public int Handle { get; set; }
            public ActionPointGUIComponent ActionPointGUIComponent;

            public static OnTurnEnd alloc(ActionPointGUIComponent p_actionPointGUIComponent)
            {
                OnTurnEnd l_instance = new OnTurnEnd();
                l_instance.ActionPointGUIComponent = p_actionPointGUIComponent;
                return l_instance;
            }

            public EventCallbackResponse Execute(ref Entity p_param1)
            {
                ActionPointGUIComponent.gameObject.SetActive(false);
                ActionPointGUIComponentContainer.ActionPointGUIComponents.Remove(ActionPointGUIComponent);
                return EventCallbackResponse.OK;
            }

        }

        public void Tick(float d)
        {
            m_uiGauge.SetFillRate(ActionPoint.getFillRate(cachedActionPointEntityComponent.Get()));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ActionPointGUIComponentContainer.ActionPointGUIComponents.Remove(this);
        }
    }
}