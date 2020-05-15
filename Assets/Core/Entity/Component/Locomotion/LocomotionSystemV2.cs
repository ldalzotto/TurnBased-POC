using _Entity;
using _NavigationGraph;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace _Locomotion
{
    public static class LocomotionSystemV2Container
    {
        public static List<LocomotionSystemV2> LocomotionSystemComponents = new List<LocomotionSystemV2>();

        public static void Tick(float d)
        {
            for (int i = 0; i < LocomotionSystemComponents.Count; i++)
            {
                LocomotionSystemComponents[i].Tick(d);
            }
        }
    }

    /// <summary>
    /// Physically move the <see cref="Entity"/> towards a <see cref="_NavigationGraph.NavigationNode"/>.
    /// This component is limited to move to a single <see cref="_NavigationGraph.NavigationNode"/> and broadcast <see cref="OnNavigationNodeReachedEvent"/> when reached.
    /// </summary>
    public class LocomotionSystemV2
    {
        public Locomotion LocomotionComponent;

        #region Event callbacks
        /// <summary>
        /// Evetn raised when the <see cref="CurrentDestination"/> has been reached.
        /// </summary>
        private Action<NavigationNode, NavigationNode> OnNavigationNodeReachedEvent;
        #endregion

        public bool HeadingTowardsTargetNode = false;
        public float3 CurrentDestination;
        private NavigationNode CurrentTargetNode;

        public static LocomotionSystemV2 alloc(Locomotion p_locomotion)
        {
            LocomotionSystemV2 l_instance = new LocomotionSystemV2();
            l_instance.LocomotionComponent = p_locomotion;
            LocomotionSystemV2Container.LocomotionSystemComponents.Add(l_instance);
            return l_instance;
        }

        public static void free(LocomotionSystemV2 p_locomotionSystemV2)
        {
            LocomotionSystemV2Container.LocomotionSystemComponents.Remove(p_locomotionSystemV2);
        }

        public static void warp(LocomotionSystemV2 p_locomotionSystem, NavigationNode p_navigationNode)
        {
            p_locomotionSystem.LocomotionComponent.AssociatedEntity.EntityGameWorld.RootGround.WorldPosition
                    = p_navigationNode.LocalPosition;
        }

        public static void HeadTowardsNode(LocomotionSystemV2 p_locomotionSystem,
                        NavigationNode p_navigationNode, Action<NavigationNode, NavigationNode> p_onDestinationReached = null)
        {
            p_locomotionSystem.OnNavigationNodeReachedEvent = p_onDestinationReached;
            p_locomotionSystem.HeadingTowardsTargetNode = true;
            p_locomotionSystem.CurrentDestination = p_navigationNode.LocalPosition;
            p_locomotionSystem.CurrentTargetNode = p_navigationNode;
        }

        public void Tick(float d)
        {
            if (HeadingTowardsTargetNode)
            {
                float3 l_initialDirection = math.normalize(CurrentDestination - LocomotionComponent.AssociatedEntity.EntityGameWorld.RootGround.WorldPosition);
                float3 l_targetPosition = LocomotionComponent.AssociatedEntity.EntityGameWorld.RootGround.WorldPosition + (l_initialDirection * LocomotionComponent.LocomotionData.Speed * d);
                float3 l_finalDirection = math.normalize(CurrentDestination - l_targetPosition);

                // If the initial direction is not the same as the final direction, this means that the destination point has been crossed
                // thus, the destination is reached
                bool l_isDestinationReached = math.dot(l_initialDirection, l_finalDirection) <= 0.0f;
                if (l_isDestinationReached)
                {
                    /*
                        When the NavigationNode is reached, we set the position to the exact final location.
                    */
                    l_targetPosition = CurrentDestination;
                }
                LocomotionComponent.AssociatedEntity.EntityGameWorld.RootGround.WorldPosition = l_targetPosition;

                if (l_isDestinationReached)
                {
                    HeadingTowardsTargetNode = false;
                    NavigationNode l_oldNavigationNode = LocomotionComponent.AssociatedEntity.CurrentNavigationNode;

                    if (OnNavigationNodeReachedEvent != null)
                    {
                        OnNavigationNodeReachedEvent.Invoke(l_oldNavigationNode, CurrentTargetNode);
                    }
                }
                else
                {
                    float3 l_orientationDirection = l_initialDirection.ProjectOnPlane(math.up());
                    EntityGameWorld.orientTowards(ref LocomotionComponent.AssociatedEntity.EntityGameWorld, ref l_orientationDirection);
                }
            }
        }
    }
}
