using _Entity;
using _EventQueue;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
public class TestWindow : OdinEditorWindow
{
    [MenuItem("Tools/Test")]
    private static void OpenWindow()
    {
        GetWindow<TestWindow>().Show();
    }

    public List<AEvent> ExecutedEvents;

    protected override void OnEnable()
    {
        base.OnEnable();
        ExecutedEvents = new List<AEvent>();
    }

    [Button]
    private void Hook()
    {
        EventQueue.OnEventExecuted = (AEvent p_event) =>
        {
            ExecutedEvents.Add(p_event);
        };
    }
}

    */
