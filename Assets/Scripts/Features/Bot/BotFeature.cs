using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using System.Collections.Generic;

namespace Game
{
    public class BotFeature : BaseFeature, IBot
    {
        [Inject] public BotRecord Record { get; set; }
        [Inject] public BattleUnitsRecord BattleUnitsRecord { get; set; }
        [Inject] public IBotPlayerResolver BotPlayerResolver { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }
        [Inject] public ITurn Turn { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }
        [Inject] public IGrid Grid { get; set; }

        private BotConfig _config;

        public UniTask AppLaunch()
        {
            _config = ConfigService.GetConfig<BotConfig>();
            return UniTask.CompletedTask;
        }

        public UniTask OnBeforeBattleTurnStart()
        {
            // Find all bot-controlled units and order their turns
            foreach (var unit in BattleUnitsRecord.BattleUnits)
            {
                if (IsBotUnit(unit))
                {
                    OrderBotTurn(unit).Forget();
                }
            }

            return UniTask.CompletedTask;
        }

        public UniTask OnBattleTurnStarted()
        {
            // Bot turn has started - currently no action needed
            return UniTask.CompletedTask;
        }

        public UniTask OnBattleTurnEnded()
        {
            // Bot turn has ended - currently no action needed
            return UniTask.CompletedTask;
        }

        public UniTask OrderBotTurn(BattleUnitData botUnit)
        {
            // TODO: Determine bot type from unit data or config
            // For now, use Aggressive behavior as default
            BotType botType = BotType.Aggressive;

            var behaviour = _config.BotBehaviours.FirstOrDefault(b => b.BotType == botType);
            if (behaviour != null)
            {
                bool success = OrderBotBehaviour(botUnit, behaviour);
                if (success)
                {
                    Notebook.NoteData($"Bot {botUnit.BattleUnitId} successfully ordered turn using {botType} behavior");
                }
                else
                {
                    Notebook.NoteWarning($"Bot {botUnit.BattleUnitId} failed to order turn using {botType} behavior");
                }
            }
            else
            {
                Notebook.NoteError($"No behavior found for bot type {botType}. Available behaviors: {string.Join(", ", _config.BotBehaviours.Select(b => b.BotType.ToString()))}");
            }

            return UniTask.CompletedTask;
        }

        private bool OrderBotBehaviour(BattleUnitData botUnit, BotBehaviourSO behaviour)
        {
            // Logic is implemented here, using the SO as pure data configuration
            switch (behaviour.BotType)
            {
                case BotType.Aggressive:
                    return OrderAggressiveBehaviour(botUnit, behaviour as AggressiveBotBehaviourSO);
                case BotType.Roaming:
                    return OrderRoamingBehaviour(botUnit, behaviour as RoamingBotBehaviourSO);
                case BotType.Defending:
                    return OrderDefendingBehaviour(botUnit, behaviour as DefendingBotBehaviourSO);
                case BotType.Retreating:
                    return OrderRetreatingBehaviour(botUnit, behaviour as RetreatingBotBehaviourSO);
                default:
                    Notebook.NoteError($"Unknown bot type: {behaviour.BotType}");
                    return false;
            }
        }

        private bool OrderAggressiveBehaviour(BattleUnitData botUnit, AggressiveBotBehaviourSO config)
        {
            // Find enemy units (not bot units)
            var enemyUnits = BattleUnitsRecord.BattleUnits
                .Where(unit => !BotPlayerResolver.IsBotPlayer(unit.PlayerId) && !unit.IsDead)
                .ToList();

            if (enemyUnits.Count == 0)
            {
                // No enemies found - fall back to roaming behavior
                Notebook.NoteData($"Bot {botUnit.BattleUnitId} using Aggressive behavior - no enemies found, falling back to roaming");
                return TryMoveToRandomAdjacentHex(botUnit);
            }

            // Find closest enemy
            BattleUnitData closestEnemy = null;
            int closestDistance = int.MaxValue;

            foreach (var enemy in enemyUnits)
            {
                int distance = GridUtils.HexDistance(botUnit.Coordinate, enemy.Coordinate);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            // If prioritizing weak enemies, choose the weakest among enemies at the same distance
            if (config.PrioritizeWeakEnemies && closestDistance < int.MaxValue)
            {
                var enemiesAtSameDistance = enemyUnits
                    .Where(enemy => GridUtils.HexDistance(botUnit.Coordinate, enemy.Coordinate) == closestDistance)
                    .OrderBy(enemy => enemy.Health)
                    .ToList();

                if (enemiesAtSameDistance.Count > 0)
                {
                    closestEnemy = enemiesAtSameDistance.First();
                }
            }

            // Get bot's unit config to determine available attack abilities
            var botConfig = BattleUnits.GetUnitConfig(botUnit.BattleUnitId);
            if (botConfig == null)
            {
                Notebook.NoteError($"Bot {botUnit.BattleUnitId} - could not find unit config, falling back to roaming");
                // Fall back to roaming if we can't attack
                return TryMoveToRandomAdjacentHex(botUnit);
            }

            // Choose appropriate attack based on distance and available abilities
            AbilityMode chosenAttack = ChooseAttackType(botConfig, closestDistance);

            if (chosenAttack == AbilityMode.None)
            {
                // No suitable attack found - fall back to roaming behavior
                Notebook.NoteData($"Bot {botUnit.BattleUnitId} - no suitable attack for enemy at distance {closestDistance}, falling back to roaming");
                return TryMoveToRandomAdjacentHex(botUnit);
            }

            // Order the attack
            Turn.OrderTurn(botUnit.Coordinate, closestEnemy.Coordinate, chosenAttack);

            Notebook.NoteData($"Bot {botUnit.BattleUnitId} ordered {chosenAttack} on enemy {closestEnemy.BattleUnitId} at distance {closestDistance}");
            return true;
        }

        private AbilityMode ChooseAttackType(BattleUnitConfig botConfig, int distanceToTarget)
        {
            // Check for cleave attack (melee range only)
            if (distanceToTarget == 1 && botConfig.Actions.Any(a => a.Ability == AbilityMode.CleaveAttack))
            {
                return AbilityMode.CleaveAttack;
            }

            // Check for range attack if within range
            var rangeAttack = botConfig.Actions.FirstOrDefault(a => a.Ability == AbilityMode.RangeAttack);
            if (rangeAttack != null && distanceToTarget <= rangeAttack.Range && distanceToTarget > 1)
            {
                return AbilityMode.RangeAttack;
            }

            // Check for regular attack (melee range)
            if (distanceToTarget == 1 && botConfig.Actions.Any(a => a.Ability == AbilityMode.Attack))
            {
                return AbilityMode.Attack;
            }

            // No suitable attack found
            return AbilityMode.None;
        }

        private bool TryMoveToRandomAdjacentHex(BattleUnitData botUnit)
        {
            // Get all adjacent coordinates (range 1, excluding center)
            var adjacentCoords = GridUtils.GetCoordinatesInRange(botUnit.Coordinate, 1)
                .Where(coord => coord != botUnit.Coordinate) // Exclude current position
                .ToList();

            // Filter for valid movement locations
            var validMoveCoords = adjacentCoords
                .Where(coord => Grid.IsValidForAbility(AbilityMode.Move, coord))
                .ToList();

            if (validMoveCoords.Count == 0)
            {
                Notebook.NoteData($"Bot {botUnit.BattleUnitId} at {botUnit.Coordinate} has no valid adjacent hexes to move to");
                return false;
            }

            // Pick a random valid coordinate
            var randomIndex = UnityEngine.Random.Range(0, validMoveCoords.Count);
            var targetCoordinate = validMoveCoords[randomIndex];

            // Order the move
            Turn.OrderTurn(botUnit.Coordinate, targetCoordinate, AbilityMode.Move);

            Notebook.NoteData($"Bot {botUnit.BattleUnitId} ordered move from {botUnit.Coordinate} to {targetCoordinate}");
            return true;
        }

        private bool OrderRoamingBehaviour(BattleUnitData botUnit, RoamingBotBehaviourSO config)
        {
            // For now, just move to a random adjacent hex
            return TryMoveToRandomAdjacentHex(botUnit);
        }

        private bool OrderDefendingBehaviour(BattleUnitData botUnit, DefendingBotBehaviourSO config)
        {
            // TODO: Implement defending bot logic using config data
            // - Stay in current position
            // - Attack enemies that come within config.DefendRange
            // - Counter attack up to config.MaxCounterAttacks if config.CounterAttack is enabled

            Notebook.NoteData($"Bot {botUnit.BattleUnitId} using Defending behavior (DefendRange: {config.DefendRange}, CounterAttack: {config.CounterAttack}) - not yet implemented");
            return false;
        }

        private bool OrderRetreatingBehaviour(BattleUnitData botUnit, RetreatingBotBehaviourSO config)
        {
            // TODO: Implement retreating bot logic using config data
            // - Detect enemies within config.EnemyDetectionRange
            // - Move away by config.RetreatDistance if enemies detected
            // - Use Wait action if no enemies nearby and config.UseWaitWhenSafe is true

            Notebook.NoteData($"Bot {botUnit.BattleUnitId} using Retreating behavior (RetreatDistance: {config.RetreatDistance}, DetectionRange: {config.EnemyDetectionRange}) - not yet implemented");
            return false;
        }

        private bool IsBotUnit(BattleUnitData unit)
        {
            var isbot = BotPlayerResolver.IsBotPlayer(unit.PlayerId);
            return isbot;
        }
    }
}