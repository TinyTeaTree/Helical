using System;
using System.Collections.Generic;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class ConsoleFeature : BaseVisualFeature<ConsoleVisual>, IConsole, IAppLaunchAgent
    {
        private Dictionary<string, Action<string[]>> _commands = new Dictionary<string, Action<string[]>>();
        private bool _isActive = false;

        public override void Bootstrap(IBootstrap bootstrap)
        {
            base.Bootstrap(bootstrap);
        }

        public async UniTask AppLaunch()
        {
            await Start();
        }

        public async UniTask Start()
        {
            await CreateVisual();
            _visual.SetFeature(this);
            // Visual starts active but UI hidden
        }

        public void RegisterCommand(string command, Action<string[]> callback)
        {
            _commands[command.ToLower()] = callback;
        }

        public void ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            var command = parts[0].ToLower();
            var args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);

            if (_commands.TryGetValue(command, out var callback))
            {
                try
                {
                    callback(args);
                }
                catch (Exception e)
                {
                    Notebook.NoteError($"Error executing command '{command}': {e.Message}");
                }
            }
            else
            {
                Notebook.NoteWarning($"Unknown command: {command}");
            }
        }

        public void ShowConsole()
        {
            if (!_isActive)
            {
                _isActive = true;
                _visual.ShowConsole();
            }
        }

        public void HideConsole()
        {
            if (_isActive)
            {
                _isActive = false;
                _visual.HideConsole();
            }
        }

        public void ToggleConsole()
        {
            if (_isActive)
            {
                HideConsole();
            }
            else
            {
                ShowConsole();
            }
        }
    }
}
