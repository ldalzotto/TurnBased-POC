using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using _Navigation;
using System;
using _RuntimeObject;
using _Entity;
using _GameLoop;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Locomotion
{

    public static class LocomotionSystemComponentContainer
    {
        public static List<LocomotionSystemComponent> LocomotionSystemComponents = new List<LocomotionSystemComponent>();

        static LocomotionSystemComponentContainer()
        {
            GameLoop.AddGameLoopCallback(GameLoopHook.BeforePhysics, new GameLoopCallback() { GameLoopPriority = 0.0f, Callback = LocomotionSystemComponentContainer.FixedTick });
        }

        private static void FixedTick(float d)
        {
            for (int i = 0; i < LocomotionSystemComponents.Count; i++)
            {
                LocomotionSystemComponents[i].FixedTick(d);
            }
        }
    }


    /// <summary>
    /// Physically move the <see cref="RuntimeObject"/> towards a <see cref="NavigationNode"/>.
    /// His role is to update <see cref="Entity.CurrentNavigationNode"/> position. 
    /// This component is limited to move to a single <see cref="NavigationNode"/> and broadcast <see cref="OnNavigationNodeReachedEvent"/> when reached.
    /// </summary>
    public class LocomotionSystemComponent : RuntimeComponent
    {
        #region Component Dependencies
        public CachedComponent<Rigidbody> RigidBody;
        #endregion

        public float TravelSpeed;
        public Entity AssociatedEntity;

        #region Event callbacks
        /// <summary>
        /// Evetn raised when the <see cref="m_currentDestination"/> has been reached.
        /// </summary>
        private Action<NavigationNode, NavigationNode> OnNavigationNodeReachedEvent;
        #endregion

        public override void Awake()
        {
            base.Awake();
            RigidBody = CachedComponent<Rigidbody>.New(RuntimeObject.RuntimeObjectRootComponent.gameObject);
            LocomotionSystemComponentContainer.LocomotionSystemComponents.Add(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LocomotionSystemComponentContainer.LocomotionSystemComponents.Remove(this);
        }

        private bool m_headingTowardsTargetNode = false;
        private Vector3 m_currentDestination;
        private NavigationNode m_currentTargetNode;

        public void HeadTowardsNode(NavigationNode p_navigationNode, Action<NavigationNode, NavigationNode> p_onDestinationReached = null)
        {
            OnNavigationNodeReachedEvent = p_onDestinationReached;
            m_headingTowardsTargetNode = true;
            m_currentDestination = NavigationGraphComponent.get_WorldPositionFromNavigationNode(
                                NavigationGraphComponentContainer.UniqueNavigationGraphComponent,
                                p_navigationNode);
            m_currentTargetNode = p_navigationNode;
        }

        public static void HeadTowardNode(Entity p_entity, NavigationNode p_targetNavigationNode, Action<NavigationNode, NavigationNode> p_onDestinationReached = null)
        {
            
        }

        public static void warp(LocomotionSystemComponent p_locomotionSystemComponent, NavigationNode p_navigationNode)
        {
            Entity.set_currentNavigationNode(
                   p_locomotionSystemComponent.AssociatedEntity,
                   p_navigationNode
               );

            p_locomotionSystemComponent.RigidBody.Get().MovePosition(
                NavigationGraphComponent.get_WorldPositionFromNavigationNode(NavigationGraphComponentContainer.UniqueNavigationGraphComponent, p_navigationNode)
            );
        }

      

        public void FixedTick(float d)
        {
            if (m_headingTowardsTargetNode)
            {
                Vector3 l_initialDirection = Vector3.Normalize(m_currentDestination - RigidBody.Get().position);
                Vector3 l_targetPosition = RigidBody.Get().position + (l_initialDirection * TravelSpeed * d);
                Vector3 l_finalDirection = Vector3.Normalize(m_currentDestination - l_targetPosition);

                // If the initial direction is not the same as the final direction, this means that the destination point has been crossed
                // thus, the destination is reached
                bool l_isDestinationReached = Vector3.Dot(l_initialDirection, l_finalDirection) <= 0.0f;
                if (l_isDestinationReached)
                {
                    /*
                        When the NavigationNode is reached, we set the position to the exact final location.
                    */
                    l_targetPosition = m_currentDestination;
                }
                RigidBody.Get().MovePosition(l_targetPosition);

                if (l_isDestinationReached)
                {
                    m_headingTowardsTargetNode = false;
                    NavigationNode l_oldNavigationNode = AssociatedEntity.CurrentNavigationNode;

                    Entity.set_currentNavigationNode(AssociatedEntity, m_currentTargetNode);

                    if (OnNavigationNodeReachedEvent != null)
                    {
                        OnNavigationNodeReachedEvent.Invoke(l_oldNavigationNode, m_currentTargetNode);
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

            if (m_headingTowardsTargetNode)
            {
                Color l_oldColor = Handles.color;
                Handles.color = Color.red;
                Handles.DrawWireDisc(m_currentDestination, Vector3.up, 0.1f);
                Handles.color = l_oldColor;
            }

        }
#endif

    }

}

