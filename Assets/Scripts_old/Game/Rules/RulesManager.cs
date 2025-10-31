using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessRaid
{
    public class RulesManager : WagSingleton<RulesManager>
    {
        private TurnModel _turnModel;

        public List<ChampionDef> Champions = null;

        public ChampionDef GetChampionDef(string id)
        {
            return Champions.FirstOrDefault(c => c.Id == id);
        }

        public override void Awake(ContextGroup<IController> group)
        {
            _turnModel = group.Get<TurnModel>();
            Champions = Resources.Load<ChampionsSO>("Champion Collection").ChampionDefinitions;
        }

        public List<TurnEvent> GetPossibleTurns(ChampionState championState, ActionType action)
        {
            var result = new List<TurnEvent>();

            var championDef = GetChampionDef(championState.ChampionId);
            var rules = championDef.Rules.RuleSet.Rules;

            if (!championState.ActionsBlocked)
            {
                foreach (var rule in rules)
                {
                    if (rule.Action != action)
                        continue;

                    if (rule.ActionPoints > championState.ActionPoints)
                        continue;

                    var possibilities = rule.Range.Group;

                    foreach(var possibility in possibilities)
                    {
                        var outcomeLocation = GridUtils.GetLocation(championState.Location, championState.Direction, possibility);

                        TurnEvent turn = new TurnEvent()
                        {
                            Action = rule.Action,
                            ActionPoints = rule.ActionPoints,
                            BlockPostActions = rule.BlockPostActions,
                            Location = outcomeLocation
                        };

                        if (result.All(t => t.Location != turn.Location))
                        {
                            result.Add(turn);
                        }
                    }
                }
            }

            return result;
        }

        public bool CanOrder(Champion champion, ActionType action, Hex hitHex, out ChampionRule outRule)
        {
            var rules = champion.Def.Rules.RuleSet.Rules;

            var state = _turnModel.GetChampionState(champion);

            var championCoord = state.Location;
            var orderingCoord = hitHex.Location;

            if( state.ActionsBlocked)
            {
                outRule = null;
                return false;
            }    

            foreach (var rule in rules)
            {
                if (rule.Action != action)
                    continue;

                if (rule.ActionPoints > state.ActionPoints)
                    continue;

                foreach (var range in rule.Range.Group)
                {
                    var ruleLocation = GridUtils.GetLocation(championCoord, state.Direction, range);

                    if (ruleLocation == orderingCoord)
                    {
                        outRule = rule;
                        return true;
                    }
                }
            }

            outRule = null;
            return false;
        }
    }
}