using _Functional;
using System;
using Unity.Mathematics;

namespace _GameLoop
{
    public static class ExternalHooks
    {
        public static Action<string> LogDebug;
        public static MyEvent OnTickStartEvent;
        public static Action<string> Profiler_BeginSample;
        public static Action Profiler_EndSample;

        public static Func<object, float3> Transform_get_worldPosition;

        public delegate void SetWorldPositionDelegate(object transform, ref float3 worldPosition);
        public static SetWorldPositionDelegate Transform_set_worldPosition;

        static ExternalHooks()
        {
            OnTickStartEvent = MyEvent.build();
        }
    }

}
