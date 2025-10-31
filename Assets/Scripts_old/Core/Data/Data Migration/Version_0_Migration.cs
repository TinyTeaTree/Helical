public class Version_0_Migration : VersionMigration
{
    public override int FromVersion => 0;
    public override string DataId => "Some Id";

    public override int ToVersion => 1;

    public override StateBox MigratePlayer(StateBox state)
    {
        return state;
    }
}
