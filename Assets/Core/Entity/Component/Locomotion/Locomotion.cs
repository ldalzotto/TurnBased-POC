using _Entity;
using _NavigationGraph;
using System;

namespace _Locomotion
{
    public class Locomotion : AEntityComponent
    {
        public LocomotionData LocomotionData;

        public LocomotionSystemV2 LocomotionSystemV2;

        public static Locomotion alloc(LocomotionData p_locomotionData)
        {
            Locomotion l_instance = new Locomotion();
            l_instance.LocomotionData = p_locomotionData;
            l_instance.LocomotionSystemV2 = LocomotionSystemV2.alloc(l_instance);
            return l_instance;
        }

        public override void OnComponentRemoved()
        {
            base.OnComponentRemoved();
            LocomotionSystemV2.free(LocomotionSystemV2);
        }

    }

    public struct LocomotionData
    {
        public float Speed;
    }
}