using _Entity;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EntityInspectorWindow : OdinEditorWindow
{

    [MenuItem("Tools/EntityInspectorWindow")]
    private static void OpenWindow()
    {
        GetWindow<EntityInspectorWindow>().Show();
    }

    [SerializeField]
    private List<EntityRegistrationToEntity> EntityRegistrationComponents = new List<EntityRegistrationToEntity>();

    [Button("REFRESH")]
    private void Refresh()
    {
        EntityRegistrationComponents.Clear();

        new List<EntityRegistrationComponent>(GameObject.FindObjectsOfType<EntityRegistrationComponent>())
            .ForEach(
                    (EntityRegistrationComponent p_entityRegistrationComponent)
                        =>
                    {
                        EntityRegistrationComponents.Add(new EntityRegistrationToEntity() { Entity = p_entityRegistrationComponent.AssociatedEntity, EntityRegistrationComponent = p_entityRegistrationComponent });
                    });
    }

    struct EntityRegistrationToEntity
    {
        public EntityRegistrationComponent EntityRegistrationComponent;
        public Entity Entity;
    }
}
