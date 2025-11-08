using Core;

namespace Game
{
    public class MobsFeature : BaseVisualFeature<MobsVisual>, IMobs
    {
        [Inject] public MobsRecord Record { get; set; }
    }
}