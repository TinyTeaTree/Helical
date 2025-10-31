using Newtonsoft.Json;

public static class GeneralUtils
{
    public static T CloneMembers<T>(this T context)
    {
        var type = typeof(T);
        var cloneFunc = type.GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var clone = cloneFunc.Invoke(context, null);
        return (T)clone;
    }

    public static T CloneViaSerialization<T>(this T context)
    {
        var json = JsonConvert.SerializeObject(context);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static bool IsDestroyed(this WagBehaviour wagBehaviour)
    {
        return wagBehaviour == null || wagBehaviour.IsDead;
    }
}
