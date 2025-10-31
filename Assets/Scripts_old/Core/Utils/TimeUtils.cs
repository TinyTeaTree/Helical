using System;


public static class TimeUtils
{
    public static string GetTimerString(this TimeSpan span)
    {
        if (span.Days > 0)
        {
            return $"{span.Days:00}:{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }
        else if (span.Hours > 0)
        {
            return $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }
        else if (span.Minutes > 0)
        {
            return $"{span.Minutes:00}:{span.Seconds:00}";
        }
        else
        {
            return $"{span.TotalSeconds:00}";
        }
    }
}
