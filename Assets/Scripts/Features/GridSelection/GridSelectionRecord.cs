using Core;
using UnityEngine;

namespace Game
{
    public class GridSelectionRecord : BaseRecord
    {
        public Vector2Int? SelectedCoordinate { get; set; }
        public bool IsSelectionEnabled { get; set; }
        
        /// <summary>
        /// Checks if any hex is currently selected.
        /// </summary>
        public bool HasSelection => SelectedCoordinate.HasValue;
        
        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection()
        {
            SelectedCoordinate = null;
        }
    }
}