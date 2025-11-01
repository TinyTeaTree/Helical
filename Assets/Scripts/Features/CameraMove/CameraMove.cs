using Core;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class CameraMove : BaseVisualFeature<CameraMoveVisual>, ICameraMove
    {
        [Inject] public IGrid Grid { get; set; }

        public UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(CameraMoveVisual)}");
                return UniTask.CompletedTask;
            }

            // Attach visual directly to Camera.main instead of loading from Resources
            var camera = Camera.main;
            if (camera == null)
            {
                Notebook.NoteError("Camera.main not found. Cannot attach CameraMoveVisual.");
                return UniTask.CompletedTask;
            }

            _visual = camera.gameObject.AddComponent<CameraMoveVisual>();
            _visual.SetFeature(this);
            
            return UniTask.CompletedTask;
        }

        public void InitializeBounds()
        {
            if (_visual == null)
            {
                Notebook.NoteError("CameraMoveVisual not initialized. Call SetupVisual first.");
                return;
            }

            _visual.CalculateAndSetBounds(Grid);
        }

        public void Start()
        {
            if (_visual == null)
            {
                Notebook.NoteError("CameraMoveVisual not initialized. Call SetupVisual first.");
                return;
            }

            _visual.StartMovement();
        }

        public void Halt()
        {
            if (_visual == null)
            {
                return;
            }

            _visual.HaltMovement();
        }
    }
}