using System;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// Set max straight length
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
            { RelativeMove.Forward, 0.8f },
            { RelativeMove.Right, 0.1f },
            { RelativeMove.Left, 0.1f }
        };

        public int Width { get; }
        public int Height { get; }
        public Vector2Int BorderSize { get; }
        public int StemLength { get; }

        public readonly Tile[,] Tiles;

        public readonly PseudoRandom.SystemRandomManager _systemRandom;
        public readonly Validator PathValidator = new();
        public readonly SnapshotManager<Tile[,]> TilesSnapshotManager;

        private Stack<IPathModification> _modificationsHistory = new();

        public Vector2Int StartPosition  { get; private set; }
        public Vector2Int EndPosition  { get; private set; }
        
        private HashSet<Vector2Int> _bannedTilePositions;

        private (Vector2Int position, Direction facingDirection) _currentState;

        public void SetCurrentState((Vector2Int position, Direction facingDirection) newState) => _currentState = newState;

        public Tile GetTileByPosition(Vector2Int position) => Tiles[position.x, position.y];
        public Tile GetTileByPosition(int x, int y) => Tiles[x, y];

        private bool _shouldLog = false;

        public Path(int width, int height, Vector2Int startPosition, Vector2Int endPosition, Vector2Int borderSize = default, int stemLength = 1, HashSet<Vector2Int> bannedTilePositions = null, bool shouldLog = false)
        {
            Width = width;
            Height = height;
            BorderSize = borderSize;
            StemLength = stemLength;

            StartPosition = startPosition;
            EndPosition = endPosition;

            _bannedTilePositions = bannedTilePositions ?? new HashSet<Vector2Int>();

            TilesSnapshotManager = new (this);
            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);

            Tiles = new Tile[width, height];

            SetupTiles();

            SetTile(StartPosition.x, StartPosition.y, TileType.Path, Direction.Up); 

            _currentState = (StartPosition, Direction.Up);

            TilesSnapshotManager.Snapshot();

            _shouldLog = shouldLog;
        }

        public void RandomWalk()
        {
            int i = 0;

            while (_currentState.position != EndPosition || i == 0)
            {
                i++;
                
                TilesSnapshotManager.Snapshot();

                var validMoves = GetValidRelativeMoves(_currentState.position, _currentState.facingDirection);

                if (_shouldLog) Debug.Log(validMoves.Count);

                if (validMoves.Count == 0)
                {
                    if (_modificationsHistory.Count == 0)
                    {
                        break;
                        // throw new Exception("Reached dead end with no way to backtrack");
                    }

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

                bool isValidMove = IsValidMove(toPosition: tempPosition, direction);

                if (_shouldLog)  Debug.Log("IsValidMove? " + tempPosition + " " + direction + " " + isValidMove);

                if (!isValidMove)
                    return false;
            }

            if (_shouldLog)  Debug.Log("IsValidMove?IsValidMove?IsValidMove?IsValidMove?IsValidMove?IsValidMove? ");

            return true;
        }

        private bool IsValidMove(Vector2Int toPosition, Direction fromDirection)
        {

            bool isOutOfBound = toPosition.x < 0 || toPosition.y < 0 || toPosition.x >= Width || toPosition.y >= Height;
            
            // Out of bound
            if (isOutOfBound) return false;

            bool isStartPosition = toPosition.x == StartPosition.x && toPosition.y == StartPosition.y;

            // Start position
            if (isStartPosition) return false;


            bool isEdge = (toPosition.x == BorderSize.x || toPosition.y == BorderSize.y || toPosition.x == Width - 1 - BorderSize.x || toPosition.y == Height - 1 - BorderSize.y) && Tiles[toPosition.x, toPosition.y].StateData.Type == TileType.Path;

            // Edge explored area
            if (isEdge) return false;


            bool isBanned = _bannedTilePositions.Contains(toPosition);

            // Banned position
            if (isBanned) return false;


            bool isInvalidTile = !Tiles[toPosition.x, toPosition.y].StateData.IsValid || Tiles[toPosition.x, toPosition.y].StateData.IsCorner || Tiles[toPosition.x, toPosition.y].StateData.IsBorder;

            // Invalid tile
            if (isInvalidTile) return false;

            bool isAlreadyExplored = Tiles[toPosition.x, toPosition.y].StateData.Type == TileType.Path && Tiles[toPosition.x, toPosition.y].IsConnectedToDirection(fromDirection.Opposite());

            // Already explored path
            if (isAlreadyExplored) return false;

            // Debug.Log("isOutOfBound: " + isOutOfBound);
            // Debug.Log("isStartPosition: " + isStartPosition);
            // Debug.Log("isEdge: " + isEdge); 
            // Debug.Log("isBanned: " + isBanned);
            // Debug.Log("isInvalidTile: " + isInvalidTile);
            // Debug.Log("isAlreadyExplored: " + isAlreadyExplored);

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

            float roll = _systemRandom.GetRandomFloat() * total;

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
                    if (other.GetTileByPosition(x, y).StateData.Type == TileType.Path)
                    {
                        SetTile(x, y, other.GetTileByPosition(x, y));
                    }
                }
            }
        }

        public void OverrideDungeonRoom(DungeonRoom dungeonRoom)
        {
            (Vector2Int lowerBound, Vector2Int upperBound) = dungeonRoom.Bounds;

            for (int x = lowerBound.x; x < upperBound.x; x++)
            {
                for (int y = lowerBound.y; y < upperBound.y; y++)
                {
                    SetTile(x, y, dungeonRoom.Tiles[x - lowerBound.x, y - lowerBound.y]);         
                    Tiles[x,y].SetAsDungeonRoomTile();
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
                    if (Tiles[x, y].StateData.Type == TileType.Path)
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
                    if (Tiles[x, y].StateData.IsCorner == true)
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

                    if (x < BorderSize.x || y < BorderSize.y || x >= (Width - BorderSize.x) || y >= (Height - BorderSize.y))
                    {
                        Tiles[x, y].SetAsBorder();
                    }
                }
            }
        }

        public void SetTile(int x, int y, TileType type, Direction facingDirection)
        {
            Tiles[x, y].SwitchType(type, facingDirection);

            SetupAdjacentConnections(x, y);
        }

        public void SetTile(int x, int y, Tile tile)
        {
            Tiles[x, y].CloneStateData(tile);

            SetupAdjacentConnections(x, y);
        }

        private void SetupAdjacentConnections(int x, int y)
        {
            foreach (var direction in DirectionExtentions.AllDirections)
            {
                int newX = x + direction.ToVector().x;
                int newY = y + direction.ToVector().y;

                if (newX < 0 || newY < 0 || newX >= Width || newY >= Height) continue;

                if (Tiles[newX, newY].StateData.Type == TileType.Path)
                {
                    Tiles[x, y].AddConnection(direction);
                    Tiles[newX, newY].AddConnection(direction.Opposite());
                }

                if (Tiles[newX, newY].StateData.Type == TileType.Wall)
                {
                    Tiles[x, y].RemoveConnection(direction);
                    Tiles[newX, newY].RemoveConnection(direction.Opposite());
                }
            }
        }
    }
}
