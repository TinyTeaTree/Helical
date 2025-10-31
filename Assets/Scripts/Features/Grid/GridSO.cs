using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Grid", menuName = "Game/Grid/Grid")]
    public class GridSO : BaseSO
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private List<HexData> _cells = new List<HexData>();

        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        public string Id 
        { 
            get => _id; 
            set => _id = value; 
        }
        
        public int Width 
        { 
            get => _width; 
            set => _width = value; 
        }
        
        public int Height 
        { 
            get => _height; 
            set => _height = value; 
        }
        
        public List<HexData> Cells 
        { 
            get => _cells; 
            set => _cells = value ?? new List<HexData>(); 
        }
    }
}

