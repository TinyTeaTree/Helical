using Core;
using UnityEngine;

namespace Game
{
    public class GridSelectionRecord : BaseRecord
    {
        public Vector2Int? SelectedCoordinate { get; set; }
        public bool IsSelectionEnabled { get; set; }
        public AbilityMode CurrentAbilityMode { get; set; } = AbilityMode.None;
        
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
            CurrentAbilityMode = AbilityMode.None;
        }
        
        /// <summary>
        /// Sets the current ability mode.
        /// </summary>
        public void SetAbilityMode(AbilityMode mode)
        {
            CurrentAbilityMode = mode;
        }
        
        /// <summary>
        /// Clears the ability mode back to None.
        /// </summary>
        public void ClearAbilityMode()
        {
            CurrentAbilityMode = AbilityMode.None;
        }
    }
}