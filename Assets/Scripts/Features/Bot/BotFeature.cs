using Core;

namespace Game
{
    public class BotFeature : BaseFeature, IBot
    {
        [Inject] public BotRecord Record { get; set; }
    }
}