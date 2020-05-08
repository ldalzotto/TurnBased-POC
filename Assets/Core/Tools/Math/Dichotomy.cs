public static class Dichotomy
{
    private static float Max(float[] p_floatArray)
    {
        if (p_floatArray != null && p_floatArray.Length > 0)
        {
            float l_returnMax = p_floatArray[0];
            for (int i = 1; i < p_floatArray.Length; i++)
            {
                if (l_returnMax < p_floatArray[i])
                {
                    l_returnMax = p_floatArray[i];
                }
            }
            return l_returnMax;
        }
        else
        {
            return 0.0f;
        }

    }
    private static float Min(float[] p_floatArray)
    {
        if (p_floatArray != null && p_floatArray.Length > 0)
        {
            float l_returnMin = p_floatArray[0];
            for (int i = 1; i < p_floatArray.Length; i++)
            {
                if (l_returnMin > p_floatArray[i])
                {
                    l_returnMin = p_floatArray[i];
                }
            }
            return l_returnMin;
        }
        else
        {
            return 0.0f;
        }

    }

    public static float EvaluatePriority(float[] p_before, float[] p_after)
    {
        bool l_beforeCaluclation = (p_before != null && p_before.Length > 0);
        bool l_afterCaluclation = (p_after != null && p_after.Length > 0);

        if (l_beforeCaluclation && l_afterCaluclation)
        {
            return (Min(p_before) + Max(p_after)) * 0.5f;
        }
        else if (l_beforeCaluclation)
        {
            return Min(p_before) - 1.0f;
        }
        else if (l_afterCaluclation)
        {
            return Max(p_after) + 1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
}
