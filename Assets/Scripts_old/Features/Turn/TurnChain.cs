using System;
using System.Collections.Generic;

namespace ChessRaid
{
    public class TurnChain
    {
        public Champion Champion;
        public List<TurnEvent> TurnEvents = new();

        public void AddAction(Coord location, ChampionRule rule)
        {
            TurnEvents.Add(new TurnEvent()
            {
                Action = rule.Action,
                Location = location,
                ActionPoints = rule.ActionPoints,
                BlockPostActions = rule.BlockPostActions
            });
        }

        public void RemoveLastActionOrdered()
        {
            if(TurnEvents.Count == 0)
            {
                UnityEngine.Debug.LogWarning($"Calling remove ordered action on {Champion.Id} when no action was ordered");
                return;
            }

            TurnEvents.RemoveAt(TurnEvents.Count - 1);
        }

        public void RemoveTurnChain()
        {
            TurnEvents.Clear();
        }
    }
}