public interface IController
{
    /// <summary>
    /// This is for getting references, Other Controllers might not have Awake() yet and may not be ready to provide
    /// functionality
    /// </summary>
    void Awake(ContextGroup<IController> group);

    /// <summary>
    /// This is right after all the Controllers have Awake() called, so all should be avialable for communication.
    /// </summary>
    void Start();
}