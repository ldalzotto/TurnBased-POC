using Unity.Mathematics;

public static class float3Extensions
{
    public static float3 ProjectOnPlane(this float3 p_vector, float3 p_planeNormal)
    {
        float3 vector3;
        float single = math.dot(p_planeNormal, p_planeNormal);
        if (single >= float.Epsilon)
        {
            float single1 = math.dot(p_vector, p_planeNormal);
            vector3 = new float3(p_vector.x - p_planeNormal.x * single1 / single, p_vector.y - p_planeNormal.y * single1 / single, p_vector.z - p_planeNormal.z * single1 / single);
        }
        else
        {
            vector3 = p_vector;
        }
        return vector3;
    }

}


#if comment

  public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            Vector3 vector3;
            float single = Vector3.Dot(planeNormal, planeNormal);
            if (single >= Mathf.Epsilon)
            {
                float single1 = Vector3.Dot(vector, planeNormal);
                vector3 = new Vector3(vector.x - planeNormal.x * single1 / single, vector.y - planeNormal.y * single1 / single, vector.z - planeNormal.z * single1 / single);
            }
            else
            {
                vector3 = vector;
            }
            return vector3;
        }

#endif