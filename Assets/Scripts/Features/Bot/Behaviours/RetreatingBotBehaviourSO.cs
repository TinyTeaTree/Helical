using UnityEngine;

namespace Game
{
    /// <summary>
    /// Retreating bot behavior configuration data - moves away from enemies and avoids combat
    /// </summary>
    [CreateAssetMenu(fileName = "RetreatingBotBehaviour", menuName = "Game/Bot Behaviours/Retreating", order = 4)]
    public class RetreatingBotBehaviourSO : BotBehaviourSO
    {
        [Header("Retreating Behaviour Settings")]
        [SerializeField, Range(1, 10)] private int _retreatDistance = 3;
        [SerializeField, Range(1, 10)] private int _enemyDetectionRange = 5;
        [SerializeField] private bool _useWaitWhenSafe = true;

        public int RetreatDistance => _retreatDistance;
        public int EnemyDetectionRange => _enemyDetectionRange;
        public bool UseWaitWhenSafe => _useWaitWhenSafe;
    }
}
