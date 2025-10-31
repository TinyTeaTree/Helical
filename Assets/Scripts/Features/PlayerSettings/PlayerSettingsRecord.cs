using Core;

namespace Game
{
    public class PlayerSettingsRecord : BaseRecord
    {
        public bool Music { get; set; } = true;
        public bool Effects { get; set; } = true;
    }
}