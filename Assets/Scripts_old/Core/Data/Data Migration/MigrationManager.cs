using System.Collections.Generic;
using System.Linq;

public class MigrationManager : WagSingleton<MigrationManager>
{
    List<VersionMigration> _migrations = new()
    {
        new Version_0_Migration()
    };

    public StateBox TryMigrateState(StateBox state)
    {
        while(state.RequiredVersion != state.RecordedVersion && _migrations.Any(m => m.FromVersion == state.RecordedVersion))
        {
            var migration = _migrations.FirstOrDefault(m => m.FromVersion == state.RecordedVersion);

            state = migration.MigratePlayer(state);
            state.RecordedVersion = migration.ToVersion;
        }

        return state;
    }
}