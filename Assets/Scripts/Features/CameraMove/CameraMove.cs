using Agents;
using Core;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class CameraMove : BaseVisualFeature<CameraMoveVisual>, ICameraMove, IBattleLaunchAgent
    {
        [Inject] public IGrid Grid { get; set; }

        public async UniTask BattleLaunch()
        {
            await SetupVisual();
        }

        private async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(CameraMoveVisual)}");
                return;
            }

            // Create visual from prefab (it won't be parented to the camera)
            await CreateVisual();
            _visual.Camera = Camera.main;
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

        public void LerpToCoordinate(Vector2Int coordinate)
        {
            if (_visual == null)
            {
                Notebook.NoteError("CameraMoveVisual not initialized. Call SetupVisual first.");
                return;
            }

            Vector3 worldPosition = Grid.GetWorldPosition(coordinate);
            _visual.LerpToWorldPosition(worldPosition);
        }
        
        public void HandleDrag(Vector2 dragDelta)
        {
            if (_visual == null)
            {
                Notebook.NoteError("CameraMoveVisual not initialized. Call SetupVisual first.");
                return;
            }

            _visual.MoveCameraByDrag(dragDelta);
        }
    }
}