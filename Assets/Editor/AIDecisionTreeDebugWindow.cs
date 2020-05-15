using _AI._DecisionTree._Algorithm;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using static _AI._DecisionTree._Algorithm.TreeIteration;

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

        TreeIteration.OnDecisionTreeIterated = (TreeIterationResult p_choices) =>
        {
            ExecutedEvents.Add(
                new AIDecisionTreeEntry()
                {
                    ExecutionTime = DateTime.Now.ToString("hh:mm:ss.fff tt"),
                    ExecutedEvent = SerializationUtility.CreateCopy(p_choices)
                }
            );
        };

        /*
        _AI._DecisionTree._Algorithm.Algorithm.OnAIDecisionTreeTraversed = (AIdecisionTreeTraversalResponse p_choices) =>
        {
            ExecutedEvents.Add(
                new AIDecisionTreeEntry()
                {
                    ExecutionTime = DateTime.Now.ToString("hh:mm:ss.fff tt"),
                    ExecutedEvent = SerializationUtility.CreateCopy(p_choices)
                }
            );
        };.*/
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
