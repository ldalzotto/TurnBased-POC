using System.Collections;
using System.Collections.Generic;
using System;

namespace _Entity._Action
{


    /// <summary>
    /// The <see cref="EntityActionStack"/> is a unique object that stores all <see cref="IEntityAction"/> ordered by execution order.
    /// When an execution is requested, the first <see cref="IEntityAction"/> is removed, then executed.
    ///     - While in execution, the stack can be altered.
    /// Execution occurs while there is no more <see cref="IEntityAction"/> to play.
    /// 
    /// This stack is necessary to centralize all execution of <see cref="IEntityAction"/> logic and thus, having a complete control about the game flow.
    /// This stack allows to cancel next <see cref="IEntityAction"/> is an unexepected event occurs.
    /// This stack allows to hook logic after a certan type of <see cref="IEntityAction"/>.
    /// </summary>

    [Serializable]
    public class EntityActionStack
    {

        public static EntityActionStack UniqueInstance = null;

        static EntityActionStack()
        {
            UniqueInstance = new EntityActionStack();
        }

        private List<IEntityAction> m_queuedEntityActions = new List<IEntityAction>();

        /// <summary>
        /// Called when the request (<see cref="ExecuteAllEntityActions"/>) has been completed, no more <see cref="IEntityAction"/> to execute.
        /// </summary>
        private Action<EntityActionStackConsumeResponse> m_onAllEntityActionsCompleted;

        public void EnqueueEntityAction(IEntityAction p_entityAction)
        {
            if (p_entityAction != null)
            {
                m_queuedEntityActions.Add(p_entityAction);
            }
        }

        public void EnqueueEntityActionAtFirstPosition(IEntityAction p_entityAction)
        {
            if (p_entityAction != null)
            {
                m_queuedEntityActions.Insert(0, p_entityAction);
            }
        }

        public int GetQueueSize()
        {
            return m_queuedEntityActions.Count;
        }

        public void ExecuteAllEntityActions(Action<EntityActionStackConsumeResponse> p_onAllEntityActionsCompleted)
        {
            m_onAllEntityActionsCompleted = p_onAllEntityActionsCompleted;
            ConsumeEntityAction();
        }

        private void ConsumeEntityAction()
        {
            if (m_queuedEntityActions.Count > 0)
            {
                IEntityAction p_firstEntityAction = m_queuedEntityActions[0];
                m_queuedEntityActions.RemoveAt(0);
                p_firstEntityAction.GetEntityAction().Invoke(OnEntityActionCompleted);
            }
            else
            {
                m_onAllEntityActionsCompleted.Invoke(EntityActionStackConsumeResponse.OK);
            }
        }

        private void OnEntityActionCompleted(EntityActionResultAction p_entityActionResultAction)
        {
            /* Logic executed when an EntityAction has been completed. */
            for (int i = 0; i < EntityDestructionContainer.EntitiesMarkedForDestruction.Count; i++)
            {
                EnqueueEntityActionAtFirstPosition(EntityDestroyAction.build(EntityDestructionContainer.EntitiesMarkedForDestruction[i]));
            }

            switch (p_entityActionResultAction)
            {
                case EntityActionResultAction.OK:
                    ConsumeEntityAction();
                    break;
            }

#if comment

            // When the completion conditions are met, we want to abort all subsequent IEntityAction and immediately return the 
            // the fact that the EntityActionStack requires that the level ends.
            if (LevelCompletionConditionsSystemComponentContainer.UniqueInstance.IsLevelEnded())
            {
                m_queuedEntityActions.Clear();
                m_onAllEntityActionsCompleted.Invoke(EntityActionStackConsumeResponse.LEVEL_ENDED);
            }
            else
            {
              
            }

#endif

        }
    }

    public enum EntityActionResultAction : ushort
    {
        OK = 0
    }

    /// <summary>
    /// Flag returned when <see cref="EntityActionStack.ConsumeEntityAction"/> has completed.
    /// </summary>
    public enum EntityActionStackConsumeResponse : ushort
    {
        OK = 0,

        /// <summary>
        /// Execution of <see cref="IEntityAction"/> have resulted to a request to end the current level.
        /// </summary>
        LEVEL_ENDED = 1
    }

}

