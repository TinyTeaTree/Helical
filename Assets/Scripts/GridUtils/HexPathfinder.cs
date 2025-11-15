using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A* pathfinding implementation for hex grids.
    /// </summary>
    public static class HexPathfinder
    {
        private class PathNode
        {
            public Vector2Int Coordinate;
            public int GCost; // Cost from start
            public int HCost; // Heuristic cost to end
            public int FCost => GCost + HCost;
            public PathNode Parent;
            public HexDirection DirectionFromParent;

            public PathNode(Vector2Int coordinate, int gCost, int hCost, PathNode parent, HexDirection direction)
            {
                Coordinate = coordinate;
                GCost = gCost;
                HCost = hCost;
                Parent = parent;
                DirectionFromParent = direction;
            }
        }

        /// <summary>
        /// Calculates a path from start to end coordinates using A* algorithm.
        /// </summary>
        /// <param name="hexProvider">Provider for hex grid data</param>
        /// <param name="start">Starting coordinate</param>
        /// <param name="end">Ending coordinate</param>
        /// <returns>TravelPath if found, null if no path exists</returns>
        public static TravelPath CalculatePath(IHexProvider hexProvider, Vector2Int start, Vector2Int end)
        {
            var path = new TravelPath(start, end);

            // If start and end are the same, return empty path
            if (start == end)
            {
                path.MarkValid();
                return path;
            }

            var gridData = hexProvider.GetGridData();
            var openSet = new List<PathNode>();
            var closedSet = new HashSet<Vector2Int>();
            var nodeMap = new Dictionary<Vector2Int, PathNode>();

            // Create start node
            var startNode = new PathNode(start, 0, CalculateHexDistance(start, end), null, HexDirection.North);
            openSet.Add(startNode);
            nodeMap[start] = startNode;

            while (openSet.Count > 0)
            {
                // Get node with lowest F cost
                var currentNode = GetLowestFCostNode(openSet);

                // If we reached the end, reconstruct path
                if (currentNode.Coordinate == end)
                {
                    ReconstructPath(path, currentNode);
                    path.MarkValid();
                    return path;
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Coordinate);

                // Check all 6 neighbors
                foreach (HexDirection direction in System.Enum.GetValues(typeof(HexDirection)))
                {
                    var neighborCoord = GridUtils.NextHex(currentNode.Coordinate, direction);

                    // Skip if already evaluated
                    if (closedSet.Contains(neighborCoord))
                        continue;

                    // Skip if invalid for movement or occupied
                    if (!hexProvider.IsValidForMovement(neighborCoord) || hexProvider.IsOccupied(neighborCoord))
                        continue;

                    var tentativeGCost = currentNode.GCost + 1; // Each hex costs 1 to traverse

                    PathNode neighborNode;
                    if (!nodeMap.TryGetValue(neighborCoord, out neighborNode))
                    {
                        // Create new node
                        neighborNode = new PathNode(
                            neighborCoord,
                            tentativeGCost,
                            CalculateHexDistance(neighborCoord, end),
                            currentNode,
                            direction
                        );
                        nodeMap[neighborCoord] = neighborNode;
                        openSet.Add(neighborNode);
                    }
                    else if (tentativeGCost < neighborNode.GCost)
                    {
                        // Update existing node with better path
                        neighborNode.GCost = tentativeGCost;
                        neighborNode.Parent = currentNode;
                        neighborNode.DirectionFromParent = direction;
                    }
                }
            }

            // No path found
            return path; // Returns invalid path
        }

        /// <summary>
        /// Calculates hex distance (Manhattan distance for hex grids).
        /// </summary>
        private static int CalculateHexDistance(Vector2Int a, Vector2Int b)
        {
            // For hex grids, we need to use cube coordinates or proper hex distance
            // This is a simplified version - in a real implementation you'd convert to cube coordinates
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);

            // For hex grids with pointy-top orientation, distance calculation is more complex
            // This is a simplified version that works for basic cases
            return Mathf.Max(dx, dy, Mathf.Abs(dx + dy));
        }

        private static PathNode GetLowestFCostNode(List<PathNode> nodes)
        {
            PathNode lowest = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].FCost < lowest.FCost ||
                    (nodes[i].FCost == lowest.FCost && nodes[i].HCost < lowest.HCost))
                {
                    lowest = nodes[i];
                }
            }
            return lowest;
        }

        private static void ReconstructPath(TravelPath path, PathNode endNode)
        {
            var current = endNode;
            var steps = new List<TravelStep>();

            // Traverse from end to start
            while (current.Parent != null)
            {
                steps.Insert(0, new TravelStep(current.DirectionFromParent, current.Coordinate));
                current = current.Parent;
            }

            // Add steps to path
            foreach (var step in steps)
            {
                path.AddStep(step.Direction, step.Coordinate);
            }
        }
    }
}
