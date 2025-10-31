public abstract class VersionMigration
{
    public abstract int FromVersion { get; }
    public abstract int ToVersion { get; }

    public abstract string DataId { get; }

    public abstract StateBox MigratePlayer(StateBox state);
}