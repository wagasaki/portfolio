using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class YieldCache
{
    private static readonly Dictionary<float, WaitForSeconds> WaitforSecondsDic = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds tick;
        if(!WaitforSecondsDic.TryGetValue(seconds, out tick))
        {
            WaitforSecondsDic.Add(seconds, tick = new WaitForSeconds(seconds));
        }
        return tick;
    }
}
