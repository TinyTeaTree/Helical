public abstract class DataBox 
{
    public const string Default_Id = "Data";
    public virtual string Id => Default_Id;

    public WagEvent OnChanged = new("Data Box Changed");

    public virtual void Initialize()
    {

    }
}