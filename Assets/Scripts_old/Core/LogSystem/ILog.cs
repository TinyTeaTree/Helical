using System;

public interface ILog
{
    void Editor(string editorTrace);
    void Message(string message);
    void Warning(string warning);
    void Error(string error);
    void Exception(Exception e);
    void Critical(string critical);
}