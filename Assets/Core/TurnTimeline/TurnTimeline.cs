
using _Entity;
using _EntityCharacteristics;
using _Functional;
using System.Collections.Generic;

namespace _TurnTimeline
{
    public class TurnTimeline
    {
        public RefDictionary<Entity, EntityTurnTimelineCalculationData> TimelineOrderingDatas;

        public static TurnTimeline alloc()
        {
            TurnTimeline l_instance = new TurnTimeline();
            l_instance.TimelineOrderingDatas = new RefDictionary<Entity, EntityTurnTimelineCalculationData>();
            return l_instance;
        }

        public static void addEntityToTimeline(TurnTimeline p_turnTimeline, Entity p_entity)
        {
            if (!p_turnTimeline.TimelineOrderingDatas.ContainsKey(p_entity))
            {
                EntityTurnTimelineCalculationData l_initialEntityTurnTimelineCalculationdata = EntityTurnTimelineCalculationData.build();
                l_initialEntityTurnTimelineCalculationdata.CurrentTimelinePosition = EntityTurnTimelineCalculationData.calculateReferenceTurnTimelineScore(p_entity);
                p_turnTimeline.TimelineOrderingDatas.Add(p_entity, l_initialEntityTurnTimelineCalculationdata);

                MyEvent<Entity>.IEventCallback l_onEntityDestroyed = OnEntityDestroyed.build(p_turnTimeline);
                MyEvent<Entity>.register(ref p_entity.OnEntityDestroyed, ref l_onEntityDestroyed);
            }
        }



        struct OnEntityDestroyed : MyEvent<Entity>.IEventCallback
        {
            public TurnTimeline TurnTimeline;

            public static OnEntityDestroyed build(TurnTimeline p_turnTimeline)
            {
                OnEntityDestroyed l_instance = new OnEntityDestroyed();
                l_instance.TurnTimeline = p_turnTimeline;
                return l_instance;
            }

            public void Execute(ref Entity p_entity)
            {
                TurnTimeline.TimelineOrderingDatas.Remove(p_entity);
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
        /// 	- The <see cref="EntityCharacteristics.Speed"/> Entity speed and the <see cref="EntityCharacteristics.MAX_SPEED"/>.
        /// 	
        /// /!\ This score doesn't take into account the <see cref="TurnTimeline"/>, it is the initial score attributed to <paramref name="p_entity"/>.
        /// </summary>
        public static float calculateReferenceTurnTimelineScore(Entity p_entity)
        {
            return EntityCharacteristics.MAX_SPEED - Entity.get_component<EntityCharacteristics>(p_entity).Speed;
        }

    };

}

