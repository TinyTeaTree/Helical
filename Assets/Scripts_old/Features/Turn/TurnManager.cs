using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ChessRaid
{
    public class TurnManager : WagSingleton<TurnManager>
    {
        GridManager _grid;

        public bool IsExecutingTurn { get; private set; }

        public override void Start()
        {
            _grid = GridManager.Single;
        }

        public async Task ExecuteTurn()
        {
            IsExecutingTurn = true;
            {
                SelectionManager.Single.Deselect();

                var homeChampions = Squad.Single.GetChampions(Team.Home);

                List<Task> turnTasks = new();

                foreach (var champion in homeChampions)
                {
                    turnTasks.Add(ExecuteChampionTurn(champion));
                }

                await Task.WhenAll(turnTasks);

                TurnModel.Single.RemoveAllTurnChains();
            }
            IsExecutingTurn = false;
        }

        private async Task ExecuteChampionTurn(Champion champion)
        {
            var turnChain = TurnModel.Single.GetTurnChain(champion);

            foreach (var turn in turnChain.TurnEvents)
            {
                await ExecuteTurn(turn, champion);
            }
        }

        private async Task ExecuteTurn(TurnEvent turn, Champion champion)
        {
            switch (turn.Action)
            {
                case ActionType.Move:
                    await ExecuteMoveTurn(turn, champion);
                    break;
                case ActionType.Attack:
                    await ExecuteAttackTurn(turn, champion);
                    break;
                case ActionType.Rotate:
                    var direction = GridUtils.GetDirection(champion.Location, turn.Location);
                    await ExecuteRotateTurn(direction, champion);
                    break;
                default:
                    break;
            }
        }

        private async Task ExecuteAttackTurn(TurnEvent turn, Champion champion)
        {
            var locationFrom = champion.Location;
            var locationTo = turn.Location;

            if (locationFrom == locationTo)
            {
                Debug.LogWarning($"Can't attack to my own location {locationFrom}");
                return;
            }

            var direction = GridUtils.GetDirection(locationFrom, locationTo);

            await ExecuteRotateTurn(direction, champion);

            await champion.Attack(locationTo);


            var attackHex = _grid.GetHex(locationTo);
            var attackedChampion = attackHex.Champion;
            if(attackedChampion != null)
            {
                var def = attackedChampion.Def.Stats.Defense;
                var att = champion.Def.Stats.Attack;

                var damage = Mathf.Max(1, att - def);


                await attackedChampion.GetDamaged(damage);
            }
        }

        private async Task ExecuteMoveTurn(TurnEvent turn, Champion champion)
        {
            var locationFrom = champion.Location;
            var locationTo = turn.Location;

            if(locationFrom == locationTo)
            {
                Debug.LogWarning($"Can't travel to my own location {locationFrom}");
                return;
            }

            var direction = GridUtils.GetDirection(locationFrom, locationTo);

            await ExecuteRotateTurn(direction, champion);

            await champion.MoveTo(locationTo);
        }

        private async Task ExecuteRotateTurn(Direction toDirection, Champion champion)
        {
            var currentDirection = champion.Direction;

            if(currentDirection == toDirection)
            {
                return;
            }

            await champion.RotateTo(toDirection);
        }
    }
}