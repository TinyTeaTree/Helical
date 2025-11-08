using Agents;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class InputDetectionFeature : BaseVisualFeature<InputDetectionVisual>, IInputDetection, IBattleLaunchAgent
    {
        [Inject] public InputDetectionRecord Record { get; set; }
        [Inject] public IGridSelection GridSelection { get; set; }
        [Inject] public ICameraMove CameraMove { get; set; }

        public async UniTask BattleLaunch()
        {
            await SetupVisual();
        }

        private async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(InputDetectionVisual)}");
                return;
            }

            await CreateVisual();
            _visual.gameObject.SetActive(true);
        }

        public void Start()
        {
            Record.IsInputEnabled = true;
            if (_visual != null)
            {
                _visual.gameObject.SetActive(true);
            }
        }

        public void Halt()
        {
            Record.IsInputEnabled = false;
            if (_visual != null)
            {
                _visual.gameObject.SetActive(false);
            }
        }

        public void HandleClick()
        {
            GridSelection.HandleMouseClick();
        }

        public void HandleDrag(UnityEngine.Vector2 dragDelta)
        {
            CameraMove.HandleDrag(dragDelta);
        }
    }
}