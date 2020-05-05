﻿using UnityEngine;
using System.Collections;
using _Entity;
using System;
using _RuntimeObject;

namespace _Navigation._Modifier
{
    [Serializable]
    [CreateAssetMenu(fileName = "NavigationModifierDefinition", menuName = "EntityComponents/NavigationModifierDefinition")]
    public class NavigationModifierDefinition : EntityDefinitionSubObject
    {
        public NavigationModifierData NavigationModifierData;
        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            NavigationModifier l_navigationModifier = NavigationModifier.alloc(ref NavigationModifierData);
            EntityComponent.add_component<NavigationModifier>(p_entity, l_navigationModifier);
        }
    }
}