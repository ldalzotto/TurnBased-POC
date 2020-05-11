using _Entity;
using _GameLoop;
using _RuntimeObject;
using System;
using UnityEngine;

namespace _Locomotion
{
    [Serializable]
    [CreateAssetMenu(fileName = "EntityLocomotionDefinition", menuName = "EntityComponents/EntityLocomotionDefinition")]
    public class EntityLocomotionDefinition : EntityDefinitionSubObject
    {
        public float TravelSpeed;

        static EntityLocomotionDefinition()
        {
            GameLoop.AddGameLoopCallback(GameLoopHook.Tick, new GameLoopCallback() { GameLoopPriority = 0.0f, Callback = LocomotionSystemV2Container.Tick });
        }

        public override void Initialize(Entity p_entity, RuntimeObjectRootComponent p_runtimeObjectRootComponent)
        {
            Locomotion l_locomotion = Locomotion.alloc(new LocomotionData() { Speed = TravelSpeed });
            EntityComponent.add_component<Locomotion>(p_entity, l_locomotion);
        }
    }
}

