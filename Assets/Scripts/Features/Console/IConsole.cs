using System;
using Core;

namespace Game
{
    public interface IConsole : IFeature
    {
        void RegisterCommand(string command, Action<string[]> callback);
        void ExecuteCommand(string input);
        void ShowConsole();
        void HideConsole();
        void ToggleConsole();
    }
}
