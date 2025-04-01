using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public enum TileType { Wall, Path }
    public enum Direction { Up, Right, Down, Left }
    public enum RelativeMove { Forward, Right, Left, Backtrack }

    public static class DirectionExtentions
    {
        public static Direction Opposite(this Direction direction) =>
            direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.Up
            };

        public static Direction LocalRight(this Direction direction) =>
            direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => Direction.Up
            };

        public static Direction LocalLeft(this Direction direction) =>
            direction switch
            {
                Direction.Up => Direction.Left, 
                Direction.Right => Direction.Up,
                Direction.Down => Direction.Right,
                Direction.Left => Direction.Down,
                _ => Direction.Up
            };
    }

    public class Tile : ICloneable
    {
        public TileType Type { get; private set; }
        public bool IsCorner { get; private set; }
        public bool IsValid { get; private set; } = true;
        private HashSet<Direction> connections = new();

        public Tile(TileType type) => Type = type;

        public void Invalidate() => IsValid = false;

        public void Revalidate() => IsValid = true;

        public void AddConnection(Direction direction)
        {
            connections.Add(direction);
            CheckForCorner();
        }

        public void RemoveConnection(Direction direction)
        {
            connections.Remove(direction);
            CheckForCorner();
        }

        public void SwitchType(TileType newType) 
        {
            if (Type == TileType.Path && newType == TileType.Wall)
            {
                connections.Clear();
                CheckForCorner();
            }

            Type = newType;
        }

        public bool IsConnectedToDirection(Direction direction) => connections.Contains(direction);

        public object Clone()
        {
            Tile clone = new Tile(this.Type)
            {
                IsValid = this.IsValid,
                IsCorner = this.IsCorner
            };

            clone.connections = new HashSet<Direction>(this.connections);

            return clone;
        }

        private void CheckForCorner()
        {
            IsCorner = (connections.Contains(Direction.Up) && connections.Contains(Direction.Right)) ||
                       (connections.Contains(Direction.Right) && connections.Contains(Direction.Down)) ||
                       (connections.Contains(Direction.Down) && connections.Contains(Direction.Left)) ||
                       (connections.Contains(Direction.Left) && connections.Contains(Direction.Up));
        }


    }

    public class Map
    {
        public readonly Path MapPath;

        public Map(int width, int height, int stemLength = 2)
        {
            MapPath = new Path(width, height, new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1), stemLength);
        }

        public void Generate()
        {
            MapPath.Generate();
            // ExpandCorners();
        }

        private void ExpandCorners()
        {
            foreach (var (pos, tile) in MapPath.GetCornerTiles())
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    var newPath = new Path(MapPath.Width, MapPath.Height, pos, pos, MapPath.StemLength, MapPath.GetOccupiedPositions());
                    newPath.Generate();
                    MapPath.Merge(newPath);
                }
            }
        }
    }

    public class Path
    {
        private static readonly Dictionary<Direction, Vector2Int> DirectionVectors = new()
        {
            { Direction.Up, new Vector2Int(0, 1) },
            { Direction.Right, new Vector2Int(1, 0) },
            { Direction.Down, new Vector2Int(0, -1) },
            { Direction.Left, new Vector2Int(-1, 0) }
        };

        private readonly Dictionary<RelativeMove, float> moveWeights = new()
        {
            { RelativeMove.Forward, 0.6f },
            { RelativeMove.Right, 0.2f },
            { RelativeMove.Left, 0.2f }
        };

        public int Width { get; }
        public int Height { get; }
        public int StemLength { get; }
        public readonly Tile[,] tiles;
        private Vector2Int start, end;
        private HashSet<Vector2Int> bannedPositions;
        public Stack<(Vector2Int pos, Direction dir)> pathStack = new();
        private System.Random random = new();

        public readonly Stack<Tile[,]> TilesHistory;

        private Direction currentDirection;

        (Vector2Int pos, Direction dir) lastState;

        private bool hasBeenTracingBack = false;

        public Path(int width, int height, Vector2Int start, Vector2Int end, int stemLength = 1, HashSet<Vector2Int> banned = null)
        {
            Width = width;
            Height = height;
            StemLength = stemLength;
            this.start = start;
            this.end = end;
            bannedPositions = banned ?? new HashSet<Vector2Int>();

            tiles = new Tile[width, height];

            SetupTiles();

            SetTile(start.x, start.y, TileType.Path);

            currentDirection = Direction.Up;
            pathStack.Push((start, currentDirection));

            TilesHistory = new Stack<Tile[,]>();

            SaveTileHistory();
        }

        public void Generate()
        {
            (Vector2Int pos, Direction dir) state = pathStack.Peek();

            while (state.pos != end)
            {
                SaveTileHistory();

                var validMoves = GetValidRelativeMoves(state.pos, state.dir);

                if (validMoves.Count == 0)
                {
                    if (hasBeenTracingBack)
                    {
                        tiles[lastState.pos.x, lastState.pos.y].Revalidate();
                    }

                    hasBeenTracingBack = true;

                    if (pathStack.Count == 0)
                        break;
                        
                    for (int i = 0; i < StemLength; i++)
                    {
                        if (i == StemLength - 1)
                            tiles[state.pos.x, state.pos.y].Invalidate();

                        pathStack.Pop();

                        SetTile(state.pos.x, state.pos.y, TileType.Wall);

                        if (pathStack.Count == 0)
                            break;

                        lastState = state;

                        state = pathStack.Peek();
                    }
                    
                    continue;
                }

                hasBeenTracingBack = false;

                RelativeMove chosenMove = WeightedChoice(validMoves);

                Direction newDirection = GetNewDirection(state.dir, chosenMove);
                
                Vector2Int currentPos = state.pos;

                for (int i = 0; i < StemLength; i++)
                {
                    Vector2Int nextPos = currentPos + DirectionVectors[newDirection];
                    SetTile(nextPos.x, nextPos.y, TileType.Path);

                    currentPos = nextPos;
                    pathStack.Push((currentPos, newDirection));
                }

                state = (currentPos, newDirection);
                currentDirection = newDirection;
            }
        }

        private void SaveTileHistory() 
        {
            var clone = new Tile[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    clone[i, j] = tiles[i, j].Clone() as Tile;
                }
            }

            TilesHistory.Push(clone);
        }

        private bool CanMove(Vector2Int pos, Direction dir)
        {
            Vector2Int checkPos = pos;

            for (int i = 0; i < StemLength; i++)
            {
                checkPos += DirectionVectors[dir];

                if (!IsValidMove(toPosition: checkPos, dir))
                    return false;
            }

            return true;
        }

        private List<RelativeMove> GetValidRelativeMoves(Vector2Int pos, Direction dir)
        {
            var valid = new List<RelativeMove>();

            foreach (RelativeMove move in moveWeights.Keys)
            {
                Direction newDir = GetNewDirection(dir, move);

                if (CanMove(pos, newDir))
                    valid.Add(move);
            }

            return valid;
        }

        private Direction GetNewDirection(Direction currentFacing, RelativeMove relativeDirection)
        {
            return relativeDirection switch
            {
                RelativeMove.Forward => currentFacing,
                RelativeMove.Right => currentFacing.LocalRight(),
                RelativeMove.Left => currentFacing.Opposite(),
                RelativeMove.Backtrack => currentFacing.LocalLeft(),
                _ => currentFacing
            };
        }

        private RelativeMove WeightedChoice(List<RelativeMove> moves)
        {
            float total = 0f;

            foreach (var move in moves)
                total += moveWeights[move];

            float roll = (float)random.NextDouble() * total;

            foreach (var move in moves)
            {
                roll -= moveWeights[move];

                if (roll <= 0)
                    return move;
            }

            return moves[^1];
        }

        public void Merge(Path other)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (other.tiles[x, y].Type == TileType.Path && tiles[x, y] == null)
                    {
                        SetTile(x, y, TileType.Path);
                    }
                }
            }
        }

        public HashSet<Vector2Int> GetOccupiedPositions()
        {
            var positions = new HashSet<Vector2Int>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (tiles[x, y].Type == TileType.Path)
                        positions.Add(new Vector2Int(x, y));
                }
            }

            return positions;
        }

        public IEnumerable<(Vector2Int, Tile)> GetCornerTiles()
        {
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    if (tiles[x, y].IsCorner == true)
                        yield return (new Vector2Int(x, y), tiles[x, y]);
                }
            }
        }

        private bool IsValidMove(Vector2Int toPosition, Direction fromDirection)
        {
            // Out of bound
            if (toPosition.x < 0 || toPosition.y < 0 || toPosition.x >= Width || toPosition.y >= Height) return false;

            // Start position
            if (toPosition.x == 0 && toPosition.y == 0) return false;

            // Edge explored area
            if ((toPosition.x == 0 || toPosition.y == 0 || toPosition.x == Width - 1 || toPosition.y == Height - 1) && tiles[toPosition.x, toPosition.y].Type == TileType.Path) return false;

            // Banned position
            if (bannedPositions.Contains(toPosition)) return false;

            // Invalid tile
            if (!tiles[toPosition.x, toPosition.y].IsValid || tiles[toPosition.x, toPosition.y].IsCorner) return false;

            // Already explored path
            if (tiles[toPosition.x, toPosition.y].Type == TileType.Path && tiles[toPosition.x, toPosition.y].IsConnectedToDirection(fromDirection.Opposite())) return false;

            return true;
        }

        private void SetupTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tiles[x, y] = new Tile(TileType.Wall);
                }
            }
        }

        private void SetTile(int x, int y, TileType type)
        {
            tiles[x, y].SwitchType(type);

            SetupAdjacentConnections(x, y);
        }

        private void SetupAdjacentConnections(int x, int y)
        {
            foreach (var (dir, vec) in DirectionVectors)
            {
                int nx = x + vec.x, ny = y + vec.y;

                if (nx < 0 || ny < 0 || nx >= Width || ny >= Height) continue;

                if (tiles[nx, ny].Type == TileType.Path)
                {
                    tiles[x, y].AddConnection(dir);
                    tiles[nx, ny].AddConnection(dir.Opposite());
                }

                if (tiles[nx, ny].Type == TileType.Wall)
                {
                    tiles[x, y].RemoveConnection(dir);
                    tiles[nx, ny].RemoveConnection(dir.Opposite());
                }
            }
        }
    }
}
