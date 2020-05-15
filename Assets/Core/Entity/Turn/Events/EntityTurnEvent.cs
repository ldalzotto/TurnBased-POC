using _EventQueue;
using _GameLoop;

namespace _Entity._Turn
{

    /// <summary>
    /// Performs an iteration loop on the <see cref="Entity"/> to check if there is <see cref="AEvent"/> to performs in relation of the <see cref="Entity"/> turn.
    /// When there is no more <see cref="AEvent"/>, the <see cref="EntityTurnIterationEndEvent"/> is queued.
    /// </summary>
    public class EntityTurnIterationEvent : AEvent
    {
        public Entity Entity;

        public static EntityTurnIterationEvent alloc(Entity p_entity)
        {
            EntityTurnIterationEvent l_instance = new EntityTurnIterationEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }

        /// <summary>
        /// The iteration loop is :
        ///     * Making a choice over a builded <see cref="_AI._DecisionTree.DecisionTree"/>.
        ///     * Queueing <see cref="AEvent"/> based on AI choice.
        /// </summary>
        /// <param name="p_eventQueue"></param>
        public override void Execute(EventQueue p_eventQueue)
        {
            ExternalHooks.Profiler_BeginSample("EntityTurnIterationEvent");

            if (TurnIteration.Iterate(Entity, p_eventQueue))
            {
                EventQueue.enqueueEvent(p_eventQueue, EntityTurnIterationEvent.alloc(Entity));
            }
            else
            {
                EventQueue.enqueueEvent(p_eventQueue, EntityTurnIterationEndEvent.alloc(Entity));
            }

            ExternalHooks.Profiler_EndSample();
        }
    }

    /// <summary>
    /// This event is used to have a visual representation of when the <see cref="Entity"/> has no more <see cref="AEvent"/> to perform.
    /// Queued when there is no more <see cref="AEvent"/> to perform.
    /// </summary>
    public class EntityTurnIterationEndEvent : AEvent
    {
        public Entity Entity;

        public static EntityTurnIterationEndEvent alloc(Entity p_entity)
        {
            EntityTurnIterationEndEvent l_instance = new EntityTurnIterationEndEvent();
            l_instance.Entity = p_entity;
            return l_instance;
        }
    }
}