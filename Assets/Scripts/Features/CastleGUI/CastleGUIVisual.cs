using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CastleGUIVisual : BaseVisual<CastleGUIFeature>
    {
        [SerializeField] private TMP_Text _locationLabel;
        [SerializeField] private TMP_Text _nameLabel;
        
        [SerializeField] private CastleGUIUnitIcon _unitIconPrefab;
        
        private List<CastleGUIUnitIcon> _unitIcons;
        
        
    }
}