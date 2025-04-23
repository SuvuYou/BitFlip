using System;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// Set max straight length
namespace PathGeneration
{
    public enum RelativeMove { Forward, Right, Left, Backtrack }

    public class Path
    {
        private readonly Dictionary<RelativeMove, float> MOVE_WEIGHTS = new()
        {
            { RelativeMove.Forward, 0.8f },
            { RelativeMove.Right, 0.1f },
            { RelativeMove.Left, 0.1f }
        };

        public int StemLength { get; }

        public readonly TilesMatrix Tiles;
        
        public int RouteIndex { get; private set; }

        public readonly PseudoRandom.SystemRandomManager _systemRandom;
        public readonly Validator PathValidator = new();

        private Stack<IPathModification> _modificationsHistory = new();

        public Vector2Int StartPosition  { get; private set; }
        public Vector2Int EndPosition  { get; private set; }

        private (Vector2Int position, Direction facingDirection) _currentState;

        public void SetCurrentState((Vector2Int position, Direction facingDirection) newState) => _currentState = newState;

        private bool _shouldLog = false;

        public Path(TilesMatrix tiles, Vector2Int startPosition, Vector2Int endPosition, Direction initialFacingDirection = Direction.None, int stemLength = 2, int routeIndex = 1, bool shouldLog = false)
        {
            StemLength = stemLength;

            StartPosition = startPosition;
            EndPosition = endPosition;

            RouteIndex = routeIndex;

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);

            Tiles = tiles;

            if (Tiles.GetTileByPosition(StartPosition).StateData.Type != TileType.Path)
            {
                Tiles.SetTile(StartPosition.x, StartPosition.y, TileType.Path, initialFacingDirection); 
            }

            Tiles.SetTileRouteIndex(StartPosition.x, StartPosition.y, Tiles.CurrentLargestRouteIndex);

            _currentState = (StartPosition, initialFacingDirection);

            _shouldLog = shouldLog;
        }

        public void RandomWalk()
        {
            int i = 0;

            while (_currentState.position != EndPosition || i == 0)
            {
                i++;
                
                Tiles.TilesSnapshotManager.Snapshot();

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

                if (i == 1 && _currentState.facingDirection != Direction.None && CanMoveInDirection(_currentState.position, _currentState.facingDirection))
                {
                    newFacingDirection = _currentState.facingDirection;
                }

                var exploreModification = new Explore(this, _currentState, newFacingDirection, RouteIndex);

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
            if (toPosition == EndPosition) return true;

            bool isOutOfBound = Tiles.IsOutOfBounds(toPosition) || Tiles.IsOnTheBorder(toPosition);
            
            // Out of bound
            if (isOutOfBound) return false;

            bool isStartPosition = toPosition.x == StartPosition.x && toPosition.y == StartPosition.y;

            // Start position
            if (isStartPosition) return false;

            var tile = Tiles.GetTileByPosition(toPosition);

            bool isInvalidTile = !tile.StateData.IsValid || tile.StateData.ConnectionType == TileConnectionType.Corner || tile.StateData.IsBorder;

            // Invalid tile
            if (isInvalidTile) return false;

            bool isAlreadyExplored = tile.StateData.Type == TileType.Path && tile.IsConnectedToDirection(fromDirection.Opposite());

            // Already explored path
            if (isAlreadyExplored) return false;

            return true;
        }

        private Direction ApplyRelativeTurnToDirection(Direction facingDirection, RelativeMove relativeDirection)
        {
            return relativeDirection switch
            {
                RelativeMove.Forward => facingDirection.LocalForward(),
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
    }
}
