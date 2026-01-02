using UnityEngine;

namespace Game
{
    /// <summary>
    /// Defending bot behavior configuration data - stays in position and attacks approaching enemies
    /// </summary>
    [CreateAssetMenu(fileName = "DefendingBotBehaviour", menuName = "Game/Bot Behaviours/Defending", order = 3)]
    public class DefendingBotBehaviourSO : BotBehaviourSO
    {
        [Header("Defending Behaviour Settings")]
        [SerializeField, Range(1, 10)] private int _defendRange = 4;
        [SerializeField] private bool _counterAttack = true;
        [SerializeField, Range(0, 5)] private int _maxCounterAttacks = 2;

        public int DefendRange => _defendRange;
        public bool CounterAttack => _counterAttack;
        public int MaxCounterAttacks => _maxCounterAttacks;
    }
}
