using _TrasformHierarchy;
using Unity.Mathematics;

namespace _Entity
{
    /// <summary>
    /// Used for debug purpose, to be able to find the associated GameObject is possible.
    /// </summary>
    public struct EntityGameWorldInstanceID
    {
        public int ID;
    }

    public struct EntityGameWorld
    {
        public TransformComponent RootGround;

        public static EntityGameWorld build(TransformComponent p_rootGround)
        {
            EntityGameWorld l_instance = new EntityGameWorld();
            l_instance.RootGround = p_rootGround;
            return l_instance;
        }

        public static void orientTowards(ref EntityGameWorld p_entityGameWorld, ref float3 p_worldDirection)
        {
            p_entityGameWorld.RootGround.WorldRotation = quaternion.LookRotationSafe(p_worldDirection, math.up());
        }

        public static void orientTowards(ref EntityGameWorld p_entityGameWorld, Entity p_targetEntiry)
        {
            float3 p_direction = math.normalize(p_targetEntiry.EntityGameWorld.RootGround.WorldPosition - p_entityGameWorld.RootGround.WorldPosition);
            orientTowards(ref p_entityGameWorld, ref p_direction);
        }

        public static void orientTowards(ref EntityGameWorld p_entityGameWorld, Entity p_targetEntiry, float3 p_projectionPlaneNormal)
        {
            float3 p_direction = math.normalize(p_targetEntiry.EntityGameWorld.RootGround.WorldPosition - p_entityGameWorld.RootGround.WorldPosition).ProjectOnPlane(p_projectionPlaneNormal);
            orientTowards(ref p_entityGameWorld, ref p_direction);
        }
    }
}
