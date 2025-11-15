using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Represents a single step in a travel path, containing the direction to move.
    /// </summary>
    public struct TravelStep
    {
        public HexDirection Direction;
        public Vector2Int Coordinate;

        public TravelStep(HexDirection direction, Vector2Int coordinate)
        {
            Direction = direction;
            Coordinate = coordinate;
        }
    }

    /// <summary>
    /// Represents a complete travel path from start to end coordinates.
    /// Contains all intermediate steps as directions.
    /// </summary>
    public class TravelPath
    {
        public Vector2Int StartCoordinate { get; private set; }
        public Vector2Int EndCoordinate { get; private set; }
        public List<TravelStep> Steps { get; private set; }
        public bool IsValid { get; private set; }

        public TravelPath(Vector2Int start, Vector2Int end)
        {
            StartCoordinate = start;
            EndCoordinate = end;
            Steps = new List<TravelStep>();
            IsValid = false;
        }

        public void AddStep(HexDirection direction, Vector2Int coordinate)
        {
            Steps.Add(new TravelStep(direction, coordinate));
        }

        public void MarkValid()
        {
            IsValid = true;
        }

        public int TotalSteps => Steps.Count;

        /// <summary>
        /// Gets the coordinate at a specific step index.
        /// Index 0 is the first step, not the starting position.
        /// </summary>
        public Vector2Int GetCoordinateAtStep(int stepIndex)
        {
            return Steps[stepIndex].Coordinate;
        }

        /// <summary>
        /// Gets the direction for a specific step.
        /// </summary>
        public HexDirection GetDirectionAtStep(int stepIndex)
        {
            return Steps[stepIndex].Direction;
        }
    }
}
