
using System;
using UnityEngine;

namespace ChessRaid
{
    public static class GridUtils
    {
        public static Direction GetDirection(Coord from, Coord to)
        {
            if(from == to)
            {
                Debug.LogWarning($"Failed to get Direction to same Coord {from}");
                return Direction.North; //Default
            }

            if(from.X % 2 == 0)
            {
                if(from.X == to.X)
                {
                    if (from.Y < to.Y)
                    {
                        return Direction.North;
                    }
                    else
                    {
                        return Direction.South;
                    }
                }
                else if(from.X < to.X)
                {
                    if (from.Y <= to.Y)
                    {
                        return Direction.NorthEast;
                    }
                    else
                    {
                        return Direction.SouthEast;
                    }
                }
                else
                {
                    if (from.Y <= to.Y)
                    {
                        return Direction.NorthWest;
                    }
                    else
                    {
                        return Direction.SouthWest;
                    }
                }
            }
            else //X is odd
            {
                if (from.X == to.X)
                {
                    if (from.Y < to.Y)
                    {
                        return Direction.North;
                    }
                    else
                    {
                        return Direction.South;
                    }
                }
                else if (from.X < to.X)
                {
                    if (from.Y < to.Y)
                    {
                        return Direction.NorthEast;
                    }
                    else
                    {
                        return Direction.SouthEast;
                    }
                }
                else
                {
                    if (from.Y < to.Y)
                    {
                        return Direction.NorthWest;
                    }
                    else
                    {
                        return Direction.SouthWest;
                    }
                }
            }
        }

        public static Vector3 GetEulerDirection(Direction direction)
        {
            return new Vector3(0, (int)direction * 360f / 6f, 0);
        }

        public static Coord GetLocation(Coord coord, Direction offset, RuleRangeEntry ruleRangeEntry)
        {
            foreach(var direction in ruleRangeEntry.Path)
            {
                var finalDirection = GetOffsetDirection(direction, offset);
                coord = NextCoord(coord, finalDirection);
            }

            return coord;
        }

        public static Direction GetOffsetDirection(Direction direction, Direction offset)
        {
            int from = (int)direction;
            int by = (int)offset;

            int result = (from + by) % 6;
            return (Direction)result;
        }

        public static Coord NextCoord(Coord coord, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    coord.Y += 1;
                    break;
                case Direction.NorthEast:
                    if(coord.X % 2 == 0)
                    {
                        coord.X += 1;
                    }
                    else
                    {
                        coord.X += 1;
                        coord.Y += 1;
                    }
                    break;
                case Direction.SouthEast:
                    if (coord.X % 2 == 0)
                    {
                        coord.X += 1;
                        coord.Y -= 1;
                    }
                    else
                    {
                        coord.X += 1;
                    }
                    break;
                case Direction.South:
                    coord.Y -= 1;
                    break;
                case Direction.SouthWest:
                    if (coord.X % 2 == 0)
                    {
                        coord.X -= 1;
                        coord.Y -= 1;
                    }
                    else
                    {
                        coord.X -= 1;
                    }
                    break;
                case Direction.NorthWest:
                    if (coord.X % 2 == 0)
                    {
                        coord.X -= 1;
                    }
                    else
                    {
                        coord.X -= 1;
                        coord.Y += 1;
                    }
                    break;
            }

            return coord;
        }
    }
}