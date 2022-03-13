using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static string GetTimeScore(float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds((double)time);
        string timeText = ts.ToString(@"mm\:ss\.f");
        return timeText;
    }
}
