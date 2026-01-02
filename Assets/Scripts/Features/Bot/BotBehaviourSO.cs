using Core;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Base ScriptableObject for bot behavior configuration data
    /// Contains only configuration parameters - no logic
    /// </summary>
    public class BotBehaviourSO : BaseSO
    {
        [Header("Bot Behaviour Configuration")]
        [SerializeField] private BotType _botType;
        [SerializeField, TextArea] private string _description;

        // Common configuration parameters
        [Header("Common Settings")]
        [SerializeField, Range(0, 10)] private int _maxActionsPerTurn = 3;

        public BotType BotType => _botType;
        public string Description => _description;
        public int MaxActionsPerTurn => _maxActionsPerTurn;
    }
}
