namespace ChessRaid
{
    public static class BattleEventBus
    {
        public static WagEvent OnSelectionChanged = new("On Selection Changed");
        public static WagEvent TurnActionChanged = new("On Turn Action Changed");
    }
}