public class WagSingleton<T> : IController
    where T : class, new()
{
    static T _instance;
    

    public static T Single
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                if (_instance is IResetable resetable)
                {
                    WagSingletonSystem.Resetables.Add(resetable);
                }
            }

            return _instance;
        }
    }

    public virtual void Awake(ContextGroup<IController> group)
    {

    }

    public virtual void Start()
    {

    }
}
