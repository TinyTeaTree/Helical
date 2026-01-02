using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class BotConfig : BaseConfig
    {
        [Header("Bot Behaviour Collection")]
        [SerializeField] private List<BotBehaviourSO> _botBehaviours = new();

        /// <summary>
        /// All available bot behaviors (read-only access)
        /// </summary>
        public IReadOnlyList<BotBehaviourSO> BotBehaviours => _botBehaviours;
    }
}