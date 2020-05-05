using _EventQueue;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;

public class EventQueueDebugWindow : OdinEditorWindow
{
    [MenuItem("Tools/EventQueueDebugWindow")]
    private static void OpenWindow()
    {
        GetWindow<EventQueueDebugWindow>().Show();
    }

    public List<AEvent> ExecutedEvents;

    protected override void OnEnable()
    {
        base.OnEnable();
        ExecutedEvents = new List<AEvent>();
        EventQueue.OnEventExecuted = (AEvent p_event) =>
        {
            ExecutedEvents.Add(p_event);
        };
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        ExecutedEvents.Clear();
    }
}
