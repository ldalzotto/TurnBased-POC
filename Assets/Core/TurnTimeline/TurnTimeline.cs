
using _Entity;
using _Entity._Turn;
using _EntityCharacteristics;
using _EventQueue;
using _Functional;
using System;
using System.Collections.Generic;

namespace _TurnTimeline
{
    public static class TurnTimelineContainer
    {
        public static TurnTimeline UniqueTurnTimeline;
    }

    public class TurnTimeline
    {
        public RefDictionary<Entity, EntityTurnTimelineCalculationData> TimelineOrderingDatas;
        public int TurnTimelineElligibilityComponentAddedEventHandler;
        public int TurnTimelineElligibilityComponentRemovedEventHandler;

        private OnEntityTurnEndEventListener OnEntityTurnEndEventListener;

        public static TurnTimeline alloc()
        {
            TurnTimeline l_instance = new TurnTimeline();
            l_instance.TimelineOrderingDatas = new RefDictionary<Entity, EntityTurnTimelineCalculationData>();

            MyEvent<AEntityComponent>.IEventCallback l_turnTimelineComponentAddedEvent = OnTurnTimelineEllibilityComponentAdded.build(l_instance);
            l_instance.TurnTimelineElligibilityComponentAddedEventHandler =
                EntityComponentContainer.registerComponentAddedEvent<TurnTimelineElligibility>(ref l_turnTimelineComponentAddedEvent);

            MyEvent<AEntityComponent>.IEventCallback l_turnTimelineComponentRemovedEvent = OnTurnTimelineEllibilityComponentRemoved.build(l_instance);
            l_instance.TurnTimelineElligibilityComponentRemovedEventHandler =
                EntityComponentContainer.registerComponentRemovedEvent<TurnTimelineElligibility>(ref l_turnTimelineComponentRemovedEvent);

            if (EntityComponentContainer.Components.ContainsKey(typeof(TurnTimelineElligibility)))
            {
                List<AEntityComponent> l_turnTimelineElligiblityComponents = EntityComponentContainer.Components[typeof(TurnTimelineElligibility)];
                if (l_turnTimelineElligiblityComponents != null)
                {
                    for (int i = 0; i < l_turnTimelineElligiblityComponents.Count; i++)
                    {
                        OnTurnTimelineEllibilityComponentAdded.AddTurnTimelineEllibilityToTimeline(l_instance, (TurnTimelineElligibility)l_turnTimelineElligiblityComponents[i]);
                    }
                }
            }

            l_instance.OnEntityTurnEndEventListener = OnEntityTurnEndEventListener.alloc(l_instance);
            EventQueueListener.registerEvent(ref EventQueue.UniqueInstance.EventQueueListener, l_instance.OnEntityTurnEndEventListener);

            TurnTimelineContainer.UniqueTurnTimeline = l_instance;

            return l_instance;
        }

        public static void free(TurnTimeline p_turnTimeline)
        {
            EntityComponentContainer.unRegisterComponentAddedEvent<TurnTimelineElligibility>(p_turnTimeline.TurnTimelineElligibilityComponentAddedEventHandler);
            EntityComponentContainer.unRegisterComponentRemovedEvent<TurnTimelineElligibility>(p_turnTimeline.TurnTimelineElligibilityComponentRemovedEventHandler);
            EventQueueListener.unRegisterEvent(ref EventQueue.UniqueInstance.EventQueueListener, p_turnTimeline.OnEntityTurnEndEventListener);
        }


        public static void addEntityToTimeline(TurnTimeline p_turnTimeline, Entity p_entity)
        {
            if (!p_turnTimeline.TimelineOrderingDatas.ContainsKey(p_entity))
            {
                EntityTurnTimelineCalculationData l_initialEntityTurnTimelineCalculationdata = EntityTurnTimelineCalculationData.build();
                l_initialEntityTurnTimelineCalculationdata.CurrentTimelinePosition = EntityTurnTimelineCalculationData.calculateReferenceTurnTimelineScore(p_entity);
                p_turnTimeline.TimelineOrderingDatas.Add(p_entity, l_initialEntityTurnTimelineCalculationdata);
            }
        }


        struct OnTurnTimelineEllibilityComponentAdded : MyEvent<AEntityComponent>.IEventCallback
        {
            public TurnTimeline TurnTimeline;

            public int Handle { get; set; }

            public static OnTurnTimelineEllibilityComponentAdded build(TurnTimeline p_turnTimeline)
            {
                OnTurnTimelineEllibilityComponentAdded l_instance = new OnTurnTimelineEllibilityComponentAdded();
                l_instance.TurnTimeline = p_turnTimeline;
                return l_instance;
            }

            public EventCallbackResponse Execute(ref AEntityComponent p_entityComponent)
            {
                AddTurnTimelineEllibilityToTimeline(TurnTimeline, ((TurnTimelineElligibility)p_entityComponent));
                return EventCallbackResponse.OK;
            }

            public static void AddTurnTimelineEllibilityToTimeline(TurnTimeline p_turnTimeline, TurnTimelineElligibility p_turnTimelineElligibility)
            {
                if (p_turnTimelineElligibility.TurnTimelineElligibilityData.IsElligibleForTimeline)
                {
                    TurnTimeline.addEntityToTimeline(p_turnTimeline, p_turnTimelineElligibility.AssociatedEntity);
                }
            }


        }

        struct OnTurnTimelineEllibilityComponentRemoved : MyEvent<AEntityComponent>.IEventCallback
        {
            public int Handle { get; set; }
            public TurnTimeline TurnTimeline;

            public static OnTurnTimelineEllibilityComponentRemoved build(TurnTimeline p_turnTimeline)
            {
                OnTurnTimelineEllibilityComponentRemoved l_instance = new OnTurnTimelineEllibilityComponentRemoved();
                l_instance.TurnTimeline = p_turnTimeline;
                return l_instance;
            }

            public EventCallbackResponse Execute(ref AEntityComponent p_entityComponent)
            {
                TurnTimelineElligibility l_turnTimelineElligibility = (TurnTimelineElligibility)p_entityComponent;
                if (l_turnTimelineElligibility.TurnTimelineElligibilityData.IsElligibleForTimeline)
                {
                    TurnTimeline.TimelineOrderingDatas.Remove(p_entityComponent.AssociatedEntity);
                }
                return EventCallbackResponse.OK;
            }
        }

    }

    /// <summary>
    /// The <see cref="EntityTurnTimelineCalculationData"/> holds the current position of an <see cref="Entity"/> in the <see cref="TurnTimeline"/>.
    /// Position in the <see cref="TurnTimeline"/> is indicated by <see cref="CurrentTimelinePosition"/>.
	///     - For details on <see cref="CurrentTimelinePosition"/> calculation, see <see cref="calculateReferenceTurnTimelineScore"/>.
    /// This structure is read and write by the TurnTimelineAlgorithm.
    /// </summary>
    public struct EntityTurnTimelineCalculationData
    {
        /// <summary>
        /// If we compare two <see cref="EntityTurnTimelineCalculationData"/>, the one with the lowest <see cref="CurrentTimelinePosition"/> is considered int front of the other in 
        /// the <see cref="TurnTimeline"/>.
        /// </summary>
        public float CurrentTimelinePosition;

        public static EntityTurnTimelineCalculationData build()
        {
            EntityTurnTimelineCalculationData l_instance = new EntityTurnTimelineCalculationData();
            l_instance.CurrentTimelinePosition = 0.0f;
            return l_instance;
        }


        /// <summary>
        /// Calculates the reference timeline score by taking into account :
        /// 	- The <see cref="EntityCharacteristics.EntityCharacteristicsData.Speed"/> Entity speed and the <see cref="EntityCharacteristics.MAX_SPEED"/>.
        /// 	
        /// /!\ This score doesn't take into account the <see cref="TurnTimeline"/>, it is the initial score attributed to <paramref name="p_entity"/>.
        /// </summary>
        public static float calculateReferenceTurnTimelineScore(Entity p_entity)
        {
            return EntityCharacteristics.MAX_SPEED - EntityComponent.get_component<EntityCharacteristics>(p_entity).EntityCharacteristicsData.Speed;
        }

    };

}

