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

        public PredeterminedUnitData[] PredeterminedUnits
        {
            get => _data.PredeterminedUnits ?? Array.Empty<PredeterminedUnitData>();
            set => _data.PredeterminedUnits = value ?? Array.Empty<PredeterminedUnitData>();
        }

        public GridData GetData()
        {
            return _data;
        }

        public void AddPredeterminedUnit(string unitId, Vector2Int coordinate, int level, string playerId)
        {
            var unitData = new PredeterminedUnitData
            {
                UnitId = unitId,
                Coordinate = coordinate,
                Level = level,
                PlayerId = playerId
            };

            var currentUnits = PredeterminedUnits;
            Array.Resize(ref currentUnits, currentUnits.Length + 1);
            currentUnits[currentUnits.Length - 1] = unitData;
            PredeterminedUnits = currentUnits;
        }

        public void RemovePredeterminedUnit(Vector2Int coordinate)
        {
            var currentUnits = PredeterminedUnits;
            var filteredUnits = Array.FindAll(currentUnits, unit => unit.Coordinate != coordinate);
            PredeterminedUnits = filteredUnits;
        }

        public PredeterminedUnitData? GetPredeterminedUnitAt(Vector2Int coordinate)
        {
            var unit = Array.Find(PredeterminedUnits, u => u.Coordinate == coordinate);
            return unit.UnitId != null ? unit : (PredeterminedUnitData?)null;
        }

        private bool IsWithinBounds(Vector2Int coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < Width &&
                   coordinate.y >= 0 && coordinate.y < Height;
        }
    }
}

