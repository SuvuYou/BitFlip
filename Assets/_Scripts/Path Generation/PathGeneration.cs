using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: Clean up
// Set max straight length
// Use seeded random
namespace PathGeneration
{
    public enum TileType { Wall, Path }
    public enum Direction { Up, Right, Down, Left }
    public enum RelativeMove { Forward, Right, Left, Backtrack }

    public interface IPathModification
    {
        void Modify();
        void Undo();
    }

    public class Explore : IPathModification
    {
        // Ref
        private readonly Path path;
        
        // Cache
        private readonly TileType[] previousTypes;
        private readonly (Vector2Int pos, Direction currentFacingDirection) cachedState;

        // Modification params
        private readonly Direction ExploreDirection;

        public Explore(Path path, (Vector2Int pos, Direction currentFacingDirection) cachedState, Direction ExploreDirection)
        {
            this.path = path;
            this.cachedState = cachedState;
            this.ExploreDirection = ExploreDirection;

            previousTypes = new TileType[path.StemLength];
        }

        public void Modify()
        {
            Vector2Int currentPos = cachedState.pos;

            for (int i = 0; i < path.StemLength; i++)
            {
                Vector2Int nextPos = currentPos + ExploreDirection.ToVector();

                previousTypes[i] = path.tiles[nextPos.x, nextPos.y].Type;

                path.SetTile(nextPos.x, nextPos.y, TileType.Path);

                currentPos = nextPos;

                if (i == 0)
                    path.AddPathRoot(nextPos);
            }

            path.CurrentState = (currentPos, ExploreDirection);
        }

        public void Undo()
        {
            Vector2Int currentPos = cachedState.pos;

            for (int i = 0; i < path.StemLength; i++)
            {
                var tileType = previousTypes[i];

                Debug.Log(tileType);

                Vector2Int nextPos = currentPos + ExploreDirection.ToVector();
                path.SetTile(nextPos.x, nextPos.y, tileType);

                currentPos = nextPos;

                if (i == 0)
                    path.PopPathRoot();
            }

            path.CurrentState = cachedState;
        }
    }


    public static class DirectionExtentions
    {
        public static List<Direction> AllDirections = new () { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

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

        public static Vector2Int ToVector(this Direction direction) =>
            direction switch
            {
                Direction.Up => new Vector2Int(0, 1), 
                Direction.Right => new Vector2Int(1, 0),
                Direction.Down => new Vector2Int(0, -1),
                Direction.Left => new Vector2Int(-1, 0),
                _ => new Vector2Int(0, 1)
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
        private readonly Dictionary<RelativeMove, float> moveWeights = new()
        {
            { RelativeMove.Forward, 0.4f },
            { RelativeMove.Right, 0.3f },
            { RelativeMove.Left, 0.3f }
        };

        public int Width { get; }
        public int Height { get; }
        public int StemLength { get; }
        public readonly Tile[,] tiles;
        private Vector2Int start, end;
        private HashSet<Vector2Int> bannedPositions;
        private System.Random random = new();

        public readonly Stack<Tile[,]> TilesHistory;

        private Stack<IPathModification> _modificationsHistory = new();
        private Stack<Vector2Int> _pathRoots = new();

        private Stack<Vector2Int> _validationStack = new();

        private bool _hasBeenInvalidated;

        public (Vector2Int pos, Direction currentFacingDirection) CurrentState;

        public void AddPathRoot(Vector2Int rootPos) => _pathRoots.Push(rootPos);

        public void PopPathRoot() => _pathRoots.Pop();

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

            TilesHistory = new Stack<Tile[,]>();

            SaveTileHistory();
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

        public void Generate()
        {
            while (CurrentState.pos != end)
            {
                SaveTileHistory();

                var validMoves = GetValidRelativeMoves(CurrentState.pos, CurrentState.currentFacingDirection);

                if (validMoves.Count == 0)
                {
                    if (_modificationsHistory.Count == 0)
                        throw new Exception("Reached dead end with no way to backtrack");

                    if (_hasBeenInvalidated)
                    {
                        Vector2Int pos = _validationStack.Pop();
                        tiles[pos.x, pos.y].Revalidate();
                    }

                    Vector2Int invalidPos = _pathRoots.Peek();

                    tiles[invalidPos.x, invalidPos.y].Invalidate();

                    _validationStack.Push(invalidPos);

                    var lastModifiation = _modificationsHistory.Pop();
                    lastModifiation.Undo();

                    _hasBeenInvalidated = true;
                    
                    continue;
                }

                _hasBeenInvalidated = false;

                RelativeMove chosenMove = WeightedChoice(validMoves);

                Direction newCurrentFacingDirection = GetNewDirection(CurrentState.currentFacingDirection, chosenMove);

                var exploreModification = new Explore(this, CurrentState, newCurrentFacingDirection);

                exploreModification.Modify();

                _modificationsHistory.Push(exploreModification);
            }
        }

        private bool CanMove(Vector2Int pos, Direction dir)
        {
            Vector2Int checkPos = pos;

            for (int i = 0; i < StemLength; i++)
            {
                checkPos += dir.ToVector();

                if (!IsValidMove(toPosition: checkPos, dir))
                    return false;
            }

            return true;
        }

        private List<RelativeMove> GetValidRelativeMoves(Vector2Int pos, Direction currentFacing)
        {
            var valid = new List<RelativeMove>();

            foreach (RelativeMove move in moveWeights.Keys)
            {
                Direction newDir = GetNewDirection(currentFacing, move);

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
                RelativeMove.Left => currentFacing.LocalLeft(),
                RelativeMove.Backtrack => currentFacing.Opposite(),
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

        public void SetTile(int x, int y, TileType type)
        {
            tiles[x, y].SwitchType(type);

            SetupAdjacentConnections(x, y);
        }

        private void SetupAdjacentConnections(int x, int y)
        {
            foreach (var dir in DirectionExtentions.AllDirections)
            {
                int nx = x + dir.ToVector().x, ny = y + dir.ToVector().y;

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
