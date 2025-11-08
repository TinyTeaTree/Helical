using System;
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
        private GridData _data;

        [SerializeField]
        private GameObject _gluePrefab;

        [SerializeField]
        private GameObject _cameraAnchorPrefab;

        public string Id 
        { 
            get => _id; 
            set => _id = value; 
        }
        
        public int Width 
        { 
            get => _data.Width; 
            set => _data.Width = value; 
        }
        
        public int Height 
        { 
            get => _data.Height; 
            set => _data.Height = value; 
        }
        
        public HexData[] Cells
        {
            get => _data.Cells;
            set
            {
                _data.Cells = value ?? Array.Empty<HexData>();
            }
        }

        public GameObject GluePrefab
        {
            get => _gluePrefab;
            set => _gluePrefab = value;
        }

        public GameObject CameraAnchorPrefab
        {
            get => _cameraAnchorPrefab;
            set => _cameraAnchorPrefab = value;
        }

        public GridData GetData()
        {
            return _data;
        }

        private bool IsWithinBounds(Vector2Int coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < Width &&
                   coordinate.y >= 0 && coordinate.y < Height;
        }
    }
}

