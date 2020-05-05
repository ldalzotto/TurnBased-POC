using _Functional;
using System;

namespace _GameLoop
{
    public static class ExternalHooks
    {
        public static Action<string> LogDebug;
        public static MyEvent OnTickStartEvent;

        static ExternalHooks()
        {
            OnTickStartEvent = MyEvent.build();
        }
    }

}
