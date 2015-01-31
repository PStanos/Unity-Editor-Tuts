using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
    public static string[] ToStrings<T>(this List<T> list)
    {
        List<string> strings = new List<string>();

        foreach (T obj in list)
        {
            strings.Add(obj.ToString());
        }

        return strings.ToArray();
    }
}
