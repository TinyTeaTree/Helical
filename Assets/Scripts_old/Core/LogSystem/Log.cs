using System;

public class Log : WagSingleton<Log>
{
    ILog _defaultLogger = new DefaultLogger();

    public void SetDefaultLogger(ILog logger)
    {
        _defaultLogger = logger;
    }

    public static void Critical(string critical)
    {
        Single._defaultLogger.Critical(critical);
    }

    public static void Editor(string editorTrace)
    {
        Single._defaultLogger.Editor(editorTrace);
    }

    public static void Error(string error)
    {
        Single._defaultLogger.Error(error);
    }

    public static void Exception(Exception e)
    {
        Single._defaultLogger.Exception(e);
    }

    public static void Message(string message)
    {
        Single._defaultLogger.Message(message);
    }

    public static void Warning(string warning)
    {
        Single._defaultLogger.Warning(warning);
    }
}
