using System.Collections.Generic;
using ShessRaid;
using UnityEngine;

namespace ChessRaid
{
    public class MobsManager : WagSingleton<MobsManager>
    {
        private MobsState _state;
        private MobsView _view;
        private List<MobDef> _mobCollection; 

        public override void Awake(ContextGroup<IController> group)
        {
            GameObject go = new GameObject("Mobs");
            _view = go.AddComponent<MobsView>();
            _mobCollection = Resources.Load<MobsSO>("Mob Collection").Mobs;
        }

        public override void Start()
        {
            _state = PlayerManager.Single.State.Grid.Mobs;
        }

        public void SetUp()
        {

        }
    }
}