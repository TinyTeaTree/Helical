using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class StringUtils
{

    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static bool HasContent(this string s)
    {
        return !string.IsNullOrEmpty(s);
    }

}
