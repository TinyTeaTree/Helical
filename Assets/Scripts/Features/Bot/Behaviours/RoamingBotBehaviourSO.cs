using UnityEngine;

namespace Game
{
    /// <summary>
    /// Roaming bot behavior configuration data - moves around randomly, attacks when enemies are nearby
    /// </summary>
    [CreateAssetMenu(fileName = "RoamingBotBehaviour", menuName = "Game/Bot Behaviours/Roaming", order = 2)]
    public class RoamingBotBehaviourSO : BotBehaviourSO
    {
        [Header("Roaming Behaviour Settings")]
        [SerializeField, Range(1, 5)] private int _maxMovesPerTurn = 2;
        [SerializeField, Range(1, 10)] private int _attackRange = 3;
        [SerializeField] private bool _avoidDanger = true;

        public int MaxMovesPerTurn => _maxMovesPerTurn;
        public int AttackRange => _attackRange;
        public bool AvoidDanger => _avoidDanger;
    }
}
