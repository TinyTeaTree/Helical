public class WagMonoton<T> : WagBehaviour
    where T : WagMonoton<T>
{
    public static T Single { get; private set; }


    
    protected override void Awake()
    {
        base.Awake();

        if (Single == null )
        {
            Single = (T) this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (this == Single)
        {
            Single = null;
        }
    }
}
