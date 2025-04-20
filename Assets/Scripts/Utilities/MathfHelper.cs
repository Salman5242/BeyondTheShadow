using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfHelper
{
    public static float CalculatePercent(float number, float percent)
    {
        return ((number * percent) / 100f);
    }
}
