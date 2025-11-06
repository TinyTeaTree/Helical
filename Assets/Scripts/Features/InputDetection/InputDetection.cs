using Core;

namespace Game
{
    public class InputDetection : BaseVisualFeature<InputDetectionVisual>, IInputDetection
    {
        [Inject] public InputDetectionRecord Record { get; set; }
    }
}