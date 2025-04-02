using System;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// Set max straight length
// Use seeded random
namespace PathGeneration
{
    public enum RelativeMove { Forward, Right, Left, Backtrack }

    public class Path : ISnapshotable<Tile[,]>
    {
        public Tile[,] TakeSnapshot()
        {
            var clone = new Tile[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    clone[i, j] = Tiles[i, j].Clone() as Tile;
                }
            }

            return clone;
        }

        private readonly Dictionary<RelativeMove, float> MOVE_WEIGHTS = new()
        {
            { RelativeMove.Forward, 0.4f },
            { RelativeMove.Right, 0.3f },
            { RelativeMove.Left, 0.3f }
        };

        public int Width { get; }
        public int Height { get; }
        public int StemLength { get; }

        public readonly Tile[,] Tiles;

        public readonly Validator PathValidator = new();
        public readonly SnapshotManager<Tile[,]> TilesSnapshotManager;

        private Stack<IPathModification> _modificationsHistory = new();

        private Vector2Int _startPosition, _endPosition;
        private HashSet<Vector2Int> _bannedTilePositions;

        private System.Random random = new();

        private (Vector2Int position, Direction facingDirection) _currentState;

        public void SetCurrentState((Vector2Int position, Direction facingDirection) newState) => _currentState = newState;

        public Tile GetTileByPosition(Vector2Int position) => Tiles[position.x, position.y];
        public Tile GetTileByPosition(int x, int y) => Tiles[x, y];

        public Path(int width, int height, Vector2Int startPosition, Vector2Int endPosition, int stemLength = 1, HashSet<Vector2Int> bannedTilePositions = null)
        {
            Width = width;
            Height = height;
            StemLength = stemLength;

            _startPosition = startPosition;
            _endPosition = endPosition;

            _bannedTilePositions = bannedTilePositions ?? new HashSet<Vector2Int>();

            TilesSnapshotManager = new (this);

            Tiles = new Tile[width, height];

            SetupTiles();

            SetTile(_startPosition.x, _startPosition.y, TileType.Path); 

            TilesSnapshotManager.Snapshot();
        }

        public void RandomWalk()
        {
            while (_currentState.position != _endPosition)
            {
                TilesSnapshotManager.Snapshot();

                var validMoves = GetValidRelativeMoves(_currentState.position, _currentState.facingDirection);

                if (validMoves.Count == 0)
                {
                    if (_modificationsHistory.Count == 0)
                        throw new Exception("Reached dead end with no way to backtrack");

                    PathValidator.InvalidateLastPathRoot();

                    var lastModifiation = _modificationsHistory.Pop();

                    lastModifiation.Undo();
                    
                    continue;
                }

                PathValidator.ReleaseInvalidationStreak();

                RelativeMove chosenMove = WeightedChoice(validMoves);

                Direction newFacingDirection = ApplyRelativeTurnToDirection(_currentState.facingDirection, chosenMove);

                var exploreModification = new Explore(this, _currentState, newFacingDirection);

                exploreModification.Modify();

                _modificationsHistory.Push(exploreModification);
            }
        }

        private List<RelativeMove> GetValidRelativeMoves(Vector2Int position, Direction facingDirection)
        {
            var validMoves = new List<RelativeMove>();

            foreach (RelativeMove move in MOVE_WEIGHTS.Keys)
            {
                Direction newDirection = ApplyRelativeTurnToDirection(facingDirection, move);

                if (CanMoveInDirection(position, newDirection))
                    validMoves.Add(move);
            }

            return validMoves;
        }

        private bool CanMoveInDirection(Vector2Int position, Direction direction)
        {
            Vector2Int tempPosition = position;

            for (int i = 0; i < StemLength; i++)
            {
                tempPosition += direction.ToVector();

                if (!IsValidMove(toPosition: tempPosition, direction))
                    return false;
            }

            return true;
        }

        private bool IsValidMove(Vector2Int toPosition, Direction fromDirection)
        {
            // Out of bound
            if (toPosition.x < 0 || toPosition.y < 0 || toPosition.x >= Width || toPosition.y >= Height) return false;

            // Start position
            if (toPosition.x == 0 && toPosition.y == 0) return false;

            // Edge explored area
            if ((toPosition.x == 0 || toPosition.y == 0 || toPosition.x == Width - 1 || toPosition.y == Height - 1) && Tiles[toPosition.x, toPosition.y].Type == TileType.Path) return false;

            // Banned position
            if (_bannedTilePositions.Contains(toPosition)) return false;

            // Invalid tile
            if (!Tiles[toPosition.x, toPosition.y].IsValid || Tiles[toPosition.x, toPosition.y].IsCorner) return false;

            // Already explored path
            if (Tiles[toPosition.x, toPosition.y].Type == TileType.Path && Tiles[toPosition.x, toPosition.y].IsConnectedToDirection(fromDirection.Opposite())) return false;

            return true;
        }

        private Direction ApplyRelativeTurnToDirection(Direction facingDirection, RelativeMove relativeDirection)
        {
            return relativeDirection switch
            {
                RelativeMove.Forward => facingDirection,
                RelativeMove.Right => facingDirection.LocalRight(),
                RelativeMove.Left => facingDirection.LocalLeft(),
                RelativeMove.Backtrack => facingDirection.Opposite(),
                _ => facingDirection
            };
        }

        private RelativeMove WeightedChoice(List<RelativeMove> moves)
        {
            float total = 0f;

            foreach (var move in moves)
                total += MOVE_WEIGHTS[move];

            float roll = (float)random.NextDouble() * total;

            foreach (var move in moves)
            {
                roll -= MOVE_WEIGHTS[move];

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
                    if (other.GetTileByPosition(x, y).Type == TileType.Path && Tiles[x, y] == null)
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
                    if (Tiles[x, y].Type == TileType.Path)
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
                    if (Tiles[x, y].IsCorner == true)
                        yield return (new Vector2Int(x, y), Tiles[x, y]);
                }
            }
        }

        private void SetupTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new Tile(TileType.Wall);
                }
            }
        }

        public void SetTile(int x, int y, TileType type)
        {
            Tiles[x, y].SwitchType(type);

            SetupAdjacentConnections(x, y);
        }

        private void SetupAdjacentConnections(int x, int y)
        {
            foreach (var direction in DirectionExtentions.AllDirections)
            {
                int newX = x + direction.ToVector().x;
                int newY = y + direction.ToVector().y;

                if (newX < 0 || newY < 0 || newX >= Width || newY >= Height) continue;

                if (Tiles[newX, newY].Type == TileType.Path)
                {
                    Tiles[x, y].AddConnection(direction);
                    Tiles[newX, newY].AddConnection(direction.Opposite());
                }

                if (Tiles[newX, newY].Type == TileType.Wall)
                {
                    Tiles[x, y].RemoveConnection(direction);
                    Tiles[newX, newY].RemoveConnection(direction.Opposite());
                }
            }
        }
    }
}
