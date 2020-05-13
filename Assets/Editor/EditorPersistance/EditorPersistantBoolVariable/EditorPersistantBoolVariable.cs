using UnityEditor;
using UnityEngine;

public class EditorPersistantBoolVariable
{
    private bool Value;

    private string key;

    public static string BuildKeyFromObject(Object obj, string foldoutName)
    {
        return obj.GetInstanceID() + "_" + foldoutName;
    }

    public static void Initialize(ref EditorPersistantBoolVariable EditorPersistantBoolVariable, string key)
    {
        if (EditorPersistantBoolVariable == null)
        {
            EditorPersistantBoolVariable = new EditorPersistantBoolVariable(key);
        }
    }

    public EditorPersistantBoolVariable(string key)
    {
        this.key = key;

        this.Value = GetValue();
    }

    public void SetValue(bool value)
    {
        if ((!this.Value && value) || (this.Value && !value))
        {
            EditorPrefs.SetBool(key, value);
        }

        this.Value = value;
    }

    public bool GetValue()
    {
        return EditorPrefs.GetBool(key);
    }
}