using System.Collections;
using _Functional;

namespace _GameLoop
{
    public static class GameLoopEvents
    {
        public static MyEvent OnTickStart;

        static GameLoopEvents()
        {
            OnTickStart = MyEvent.build();
        }
    }

}
