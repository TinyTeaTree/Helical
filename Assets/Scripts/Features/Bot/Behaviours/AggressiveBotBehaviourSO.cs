using UnityEngine;

namespace Game
{
    public class AggressiveBotBehaviourSO : BotBehaviourSO
    {
        [Header("Aggressive Behaviour Settings")]
        [SerializeField, Range(0, 10)] private int _maxAttacksPerTurn = 3;
        [SerializeField] private bool _prioritizeWeakEnemies = true;

        public int MaxAttacksPerTurn => _maxAttacksPerTurn;
        public bool PrioritizeWeakEnemies => _prioritizeWeakEnemies;
    }
}
