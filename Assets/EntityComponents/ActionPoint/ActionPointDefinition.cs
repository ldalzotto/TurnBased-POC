﻿using _Entity;
using _RuntimeObject;
using System;
using UnityEngine;

namespace _ActionPoint
{
    [Serializable]
    [CreateAssetMenu(fileName = "ActionPointDefinition", menuName = "EntityComponents/ActionPointDefinition")]
    public class ActionPointDefinition : EntityDefinitionSubObject
    {
        public ActionPointData ActionPointData;
        public ActionPointGUIComponent ActionPointGUIPrefab;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            ActionPoint l_actionPoint = ActionPoint.alloc(ref ActionPointData);
            l_actionPoint.ActionPointData.CurrentActionPoints = l_actionPoint.ActionPointData.InitialActionPoints;
            EntityComponent.add_component<ActionPoint>(p_entity, l_actionPoint);

            p_runtimeObjectRootComponent.gameObject.GetComponentInChildren<EntityGaugeContainerComponent>().InstanciateActionPointGUI(ActionPointGUIPrefab);
        }
    }

}
