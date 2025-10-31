[System.Serializable]
public abstract class StateBox : DataBox
{
    public abstract int RequiredVersion { get; }
    public int RecordedVersion;
}