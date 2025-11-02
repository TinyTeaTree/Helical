using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class GridSelection : BaseVisualFeature<GridSelectionVisual>, IGridSelection
    {
        [Inject] public GridSelectionRecord Record { get; set; }
        
        public async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(GridSelectionVisual)}");
                return;
            }

            await CreateVisual();
        }

        public void Start()
        {
            if (_visual == null)
            {
                Notebook.NoteError("GridSelectionVisual not initialized. Call SetupVisual first.");
                return;
            }

            _visual.StartSelection();
        }

        public void Halt()
        {
            if (_visual == null)
            {
                return;
            }

            _visual.HaltSelection();
        }
    }
}