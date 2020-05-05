using _Entity;
using _Entity._Turn;
using _EventQueue;
using _GameLoop;
using _RuntimeObject;
using _UI._Gauge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _ActionPoint
{
    public static class ActionPointGUIComponentContainer
    {
        public static List<ActionPointGUIComponent> ActionPointGUIComponents = new List<ActionPointGUIComponent>();

        static ActionPointGUIComponentContainer()
        {
            GameLoop.AddGameLoopCallback(GameLoopHook.Tick, new GameLoopCallback()
            {
                GameLoopPriority = GameLoopCallback.EvaluatePriority(null, null),
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
        #endregion

        private OnTurnStart onTurnStart;
        private OnTurnEnd onTurnEnd;

        private UIGaugeComponent m_uiGauge;

        public override void Awake()
        {
            base.Awake();
            m_uiGauge = transform.GetComponentInChildren<UIGaugeComponent>();
        }

        private void Start()
        {
            m_entityRegistrationComponent = RuntimeObject.FindComponent<EntityRegistrationComponent>();

            gameObject.SetActive(false);

            onTurnStart = OnTurnStart.alloc(this);
            onTurnEnd = OnTurnEnd.alloc(this);

            EventQueueListener.registerEvent(EventQueueContainer.EventQueueListener, onTurnStart);
            EventQueueListener.registerEvent(EventQueueContainer.EventQueueListener, onTurnEnd);
        }


        class OnTurnStart : AEventListener<StartEntityTurnEvent>
        {
            public ActionPointGUIComponent ActionPointGUIComponent;
            public static OnTurnStart alloc(ActionPointGUIComponent p_actionPointGUIComponent)
            {
                OnTurnStart l_instance = new OnTurnStart();
                l_instance.ActionPointGUIComponent = p_actionPointGUIComponent;
                return l_instance;
            }
            public override void OnEventExecuted(EventQueue p_eventQueue, StartEntityTurnEvent p_event)
            {
                StartEntityTurnEvent l_startEntityTurnEvent = p_event as StartEntityTurnEvent;
                if (l_startEntityTurnEvent.Entity == ActionPointGUIComponent.m_entityRegistrationComponent.AssociatedEntity)
                {
                    ActionPointGUIComponent.gameObject.SetActive(true);
                    ActionPointGUIComponentContainer.ActionPointGUIComponents.Add(ActionPointGUIComponent);
                }
                   
            }
        }

        class OnTurnEnd : AEventListener<EndEntityTurnEvent>
        {
            public ActionPointGUIComponent ActionPointGUIComponent;
            public static OnTurnEnd alloc(ActionPointGUIComponent p_actionPointGUIComponent)
            {
                OnTurnEnd l_instance = new OnTurnEnd();
                l_instance.ActionPointGUIComponent = p_actionPointGUIComponent;
                return l_instance;
            }

            public override void OnEventExecuted(EventQueue p_eventQueue, EndEntityTurnEvent p_event)
            {
                if(p_event.Entity == ActionPointGUIComponent.m_entityRegistrationComponent.AssociatedEntity)
                {
                    ActionPointGUIComponent.gameObject.SetActive(false);
                    ActionPointGUIComponentContainer.ActionPointGUIComponents.Remove(ActionPointGUIComponent);
                }
            }
        }

        public void Tick(float d)
        {
            m_uiGauge.SetFillRate(ActionPoint.getFillRate(EntityComponent.get_component<ActionPoint>(m_entityRegistrationComponent.AssociatedEntity)));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            EventQueueListener.unRegisterEvent(EventQueueContainer.EventQueueListener, onTurnStart);
            EventQueueListener.unRegisterEvent(EventQueueContainer.EventQueueListener, onTurnEnd);
            ActionPointGUIComponentContainer.ActionPointGUIComponents.Remove(this);
        }
    }
}