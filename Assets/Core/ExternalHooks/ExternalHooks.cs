using _Functional;
using System;

namespace _GameLoop
{
    public static class ExternalHooks
    {
        public static Action<string> LogDebug;
        public static MyEvent OnTickStartEvent;
        public static Action<string> Profiler_BeginSample;
        public static Action Profiler_EndSample;

        static ExternalHooks()
        {
            OnTickStartEvent = MyEvent.build();
        }
    }

}
