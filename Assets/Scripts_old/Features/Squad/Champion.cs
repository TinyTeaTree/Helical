using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ChessRaid
{
    public class Champion : MonoBehaviour
    {
        [SerializeField] string _id;
        [SerializeField] ChampionDef _def;
        [SerializeField] MeshRenderer _teamRenderer;
        [SerializeField] Color _homeColor;
        [SerializeField] Color _awayColor;
        [SerializeField] protected Animator _animator; 


        public string Id => _id;

        public ChampionState State { get; set; }

        public Coord Location => State.Location;
        public Direction Direction => State.Direction;
        public Team Team => State.Team;
        public ChampionDef Def => _def;

        public int Health => State.Health;
        public int ActionPoints => State.ActionPoints;

        public void SetDirection(Direction direction)
        {
            State.Direction = direction;

            transform.rotation = Quaternion.Euler(GridUtils.GetEulerDirection(direction));
        }

        public void SetLocation(Coord location)
        {
            State.Location = location;
        }

        public void SetTeam(Team team)
        {
            State.Team = team;

            if(Team == Team.Home)
            {
                _teamRenderer.material.color = _homeColor;
            }
            else
            {
                _teamRenderer.material.color = _awayColor;
            }
        }

        public async Task RotateTo(Direction toDirection)
        {
            var tween = transform.DORotate(GridUtils.GetEulerDirection(toDirection), 0.5f);

            _animator.SetTrigger("Rotate");

            await TaskUtils.WaitYieldInstruction(tween.WaitForCompletion());

            SetDirection(toDirection);
        }

        public async Task MoveTo(Coord to)
        {
            Coord from = Location;
            var targetHex = GridManager.Single.GetHex(to);

            var tween = transform.DOMove(targetHex.transform.position, 1f);

            _animator.SetTrigger("Step");

            await TaskUtils.WaitYieldInstruction(tween.WaitForCompletion());

            GridManager.Single.GetHex(from).SetChampion(null);
            GridManager.Single.GetHex(to).SetChampion(this);

            SetLocation(to);
        }

        public virtual async Task Attack(Coord location)
        {
            _animator.SetTrigger("Attack");

            await Task.Delay(TimeSpan.FromSeconds(1f));
        }

        public async Task GetDamaged(int damage)
        {
            State.Health = State.Health - damage;
            _animator.SetTrigger("Damage");

            if(Health <= 0)
            {
                if(SelectionManager.Single.SelectedHex?.Champion == this)
                {
                    SelectionManager.Single.Deselect();
                }

                GridManager.Single.GetHex(Location).SetChampion(null);
                TurnModel.Single.RemoveTurnChain(this);
                Squad.Single.RemoveChampion(this);

                await Task.Delay(TimeSpan.FromSeconds(0.5f));

                Destroy(gameObject);
            }
        }
    }
}