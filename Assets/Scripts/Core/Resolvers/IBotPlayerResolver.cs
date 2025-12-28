using Core;

namespace Game
{
    /// <summary>
    /// Resolver for determining if a player is a bot
    /// Exposes partial API from IPlayerAccount
    /// </summary>
    public interface IBotPlayerResolver : IResolver
    {
        bool IsBotPlayer(string playerId);
    }
}
