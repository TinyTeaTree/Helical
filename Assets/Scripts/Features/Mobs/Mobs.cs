using Core;

namespace Game
{
    public class Mobs : BaseVisualFeature<MobsVisual>, IMobs
    {
        [Inject] public MobsRecord Record { get; set; }
    }
}