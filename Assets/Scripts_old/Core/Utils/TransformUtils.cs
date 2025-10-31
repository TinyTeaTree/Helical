using System.Collections;
using UnityEngine;

public static class TransformUtils
{
    public static void ResetPosition(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
    }

    public static void SetX(this Transform transform, float x)
    {
        var pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }
    public static void SetY(this Transform transform, float y)
    {
        var pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
    public static void SetZ(this Transform transform, float z)
    {
        var pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }

    public static void SetLocalX(this Transform transform, float x)
    {
        var pos = transform.localPosition;
        pos.x = x;
        transform.localPosition = pos;
    }
    public static void SetLocalY(this Transform transform, float y)
    {
        var pos = transform.localPosition;
        pos.y = y;
        transform.localPosition = pos;
    }
    public static void SetLocalZ(this Transform transform, float z)
    {
        var pos = transform.localPosition;
        pos.z = z;
        transform.localPosition = pos;
    }
}