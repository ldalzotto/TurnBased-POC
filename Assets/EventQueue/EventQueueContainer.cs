namespace _EventQueue
{
    public static class EventQueueContainer
    {
        /// <summary>
        /// The <see cref="TurnTimelineQueue"/> is responsible of sequencing the order of <see cref="_Entity.Entity"/> turn.
        /// </summary>
        public static EventQueue TurnTimelineQueue;

        /// <summary>
        /// The <see cref="EntityActionQueue"/> executes all logic when <see cref="_Entity.Entity"/> interacts each others.
        /// </summary>
        public static EventQueue EntityActionQueue;

        static EventQueueContainer()
        {
            TurnTimelineQueue = EventQueue.alloc();
            EntityActionQueue = EventQueue.alloc();
        }

        public static void iterate()
        {
            EventQueue.iterate(TurnTimelineQueue);
        }
    }
}
