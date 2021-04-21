using _AnimatorPlayable._Interface;
using _Editor._Helper;
using _Entity;
using _EventQueue;
using _NavigationGraph;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EventQueueDebugWindow : OdinEditorWindow
{
    [MenuItem("Tools/EventQueueDebugWindow")]
    private static void OpenWindow()
    {
        GetWindow<EventQueueDebugWindow>().Show();
    }

    [FoldoutGroup("Log File")]
    [Sirenix.OdinInspector.FilePath(AbsolutePath = true)]
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

    /*
    [ValueDropdown("GetAllTypes")]
    public Type TypeFilter;
    */

    [TableList(ShowPaging = true, NumberOfItemsPerPage = 100)]
    [HideReferenceObjectPicker]
    public List<object> ExecutedEvents;

    /*
    private IEnumerable<Type> GetAllTypes()
    {
        List<Type> l_types = new List<Type>();
        if(ExecutedEvents != null)
        {
            for(int i = 0; i < ExecutedEvents.Count; i++)
            {
                EventEntry l_eventEntry = (EventEntry) ExecutedEvents[i];
                if (!l_types.Contains(l_eventEntry.ExecutedEvent.GetType()))
                {
                    l_types.Add(l_eventEntry.ExecutedEvent.GetType());
                }
            }
        }
        return l_types;
    }
    */

    protected override void OnEnable()
    {
        base.OnEnable();
        ExecutedEvents = new List<object>();


        EventQueue.OnEventExecuted = (AEvent p_event) =>
        {
            EventEntry l_eventEntry = new EventEntry()
            {
                ExecutionTime = DateTime.Now.ToString("hh:mm:ss.fff tt"),
                ExecutedEvent = (AEvent)SerializationUtility.CreateCopy(p_event)
            };
            ExecutedEvents.Add(l_eventEntry);
        };
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ExecutedEvents.Clear();
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class EventEntry
    {
        [HideLabel]
        [TableColumnWidth(10, Resizable = true)]
        public string ExecutionTime;
        public AEvent ExecutedEvent;
    }

}


public class EntityGameWorldInstanceIDDrawer : OdinValueDrawer<EntityGameWorldInstanceID>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        EditorGUILayout.IntField("Game ID", this.ValueEntry.SmartValue.ID);
        EditorGUILayout.ObjectField("Game Instance", EditorUtility.InstanceIDToObject(this.ValueEntry.SmartValue.ID), typeof(GameObject), true);
    }
}

public class NavigationNodeGameWorldInstanceIDDrawer : OdinValueDrawer<NavigationNodeGameWorldInstanceID>
{

    protected override void DrawPropertyLayout(GUIContent label)
    {
        EditorGUILayout.IntField("Game ID", this.ValueEntry.SmartValue.ID);
        EditorGUILayout.ObjectField("Game Instance", EditorUtility.InstanceIDToObject(this.ValueEntry.SmartValue.ID), typeof(GameObject), true);
    }
}


public class IAnimationPlayableDefinitionDrawer<T> : OdinValueDrawer<T> where T : IAnimationPlayableDefinition
{
    private GenericAssetSelector<T> InterfaceScriptableObjectAssetSelector;

    private GUIContent TreeSelectionButtonGUIContent = new GUIContent("O", "Open selection tree.");

    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (InterfaceScriptableObjectAssetSelector == null)
        {
            InterfaceScriptableObjectAssetSelector = new GenericAssetSelector<T>(
                    (T p_selectedAsset) => { ValueEntry.SmartValue = p_selectedAsset; });
        }

        Rect rect = EditorGUILayout.GetControlRect();

        ScriptableObject l_selectedScriptableObject = (ScriptableObject)EditorGUI.ObjectField(rect.SetWidth(rect.width * 0.9f), ValueEntry.SmartValue as ScriptableObject, typeof(ScriptableObject), false);
        if (l_selectedScriptableObject is IAnimationPlayableDefinition)
        {
            ValueEntry.SmartValue = (T)(IAnimationPlayableDefinition)l_selectedScriptableObject;
        }


        if (GUI.Button(rect.AlignRight(rect.width * 0.1f), TreeSelectionButtonGUIContent))
        {
            InterfaceScriptableObjectAssetSelector.ShowInPopup();
        }
    }
}
