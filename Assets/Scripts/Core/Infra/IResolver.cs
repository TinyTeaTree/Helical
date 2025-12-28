namespace Core
{
    /// <summary>
    /// Marker interface for Resolver interfaces that expose partial API of Features/Services
    /// Resolvers allow injecting specific functionality without depending on entire interfaces
    /// </summary>
    public interface IResolver : IInjectableInterface
    {
    }
}
