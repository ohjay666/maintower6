using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class d
{
    private static readonly string prefix = "..:";
    public static void Log(object message)
    {
       l(message);
    }
    public static void l(object message)
    {
        Debug.Log(prefix + " " + message);
    }
}