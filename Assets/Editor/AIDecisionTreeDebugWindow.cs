using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.Collections.Generic;
using static _AI._DecisionTree._Algorithm.Algorithm;
using Sirenix.Serialization;
using System;
using Sirenix.OdinInspector;
using System.IO;

public class AIDecisionTreeDebugWindow : OdinEditorWindow
{

    [MenuItem("Tools/AIDecisionTreeDebugWindow")]
    private static void OpenWindow()
    {
        GetWindow<AIDecisionTreeDebugWindow>().Show();
    }

    [FoldoutGroup("Log File")]
    [FilePath(AbsolutePath = true)]
    public string UnityProjectPath;

    [FoldoutGroup("Log File")]
    [HorizontalGroup("Log File/Buttons")]
    [Button]
    public void Read()
    {
        ExecutedEvents = SerializationUtility.DeserializeValue<List<object>>(File.ReadAllBytes(UnityProjectPath), DataFormat.JSON);
    }

    [FoldoutGroup("Log File")]
    [HorizontalGroup("Log File/Buttons")]
    [Button]
    public void Save()
    {
        File.WriteAllBytes(UnityProjectPath, SerializationUtility.SerializeValue(ExecutedEvents, DataFormat.JSON));
    }

    [TableList(ShowPaging = true)]
    [HideReferenceObjectPicker]
    public List<object> ExecutedEvents;

    protected override void OnEnable()
    {
        base.OnEnable();
        ExecutedEvents = new List<object>();
        _AI._DecisionTree._Algorithm.Algorithm.OnAIDecisionTreeTraversed = (RefList<AIDecisionTreeChoice> p_choices) =>
        {
            ExecutedEvents.Add(
                new AIDecisionTreeEntry()
                {
                    ExecutionTime = DateTime.Now.ToString("hh:mm:ss.fff tt"),
                    ExecutedEvent = SerializationUtility.CreateCopy(p_choices)
                }
            );
        };
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        ExecutedEvents.Clear();
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class AIDecisionTreeEntry
    {
        [HideLabel]
        [TableColumnWidth(10, Resizable = true)]
        public string ExecutionTime;

        public object ExecutedEvent;
    }

}
