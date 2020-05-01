using UnityEngine;
using System.Collections;
using System;
using _Entity;
using _RuntimeObject;

namespace _ActionPoint
{
    [Serializable]
    [CreateAssetMenu(fileName = "ActionPointDefinition", menuName = "EntityComponents/ActionPointDefinition")]
    public class ActionPointDefinition : EntityDefinitionSubObject
    {
        public ActionPointData ActionPointData;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            ActionPoint l_actionPoint = ActionPoint.alloc(ref ActionPointData);
            l_actionPoint.ActionPointData.CurrentActionPoints = l_actionPoint.ActionPointData.InitialActionPoints;
            EntityComponent.add_component<ActionPoint>(p_entity, ref l_actionPoint);
        }
    }

}
