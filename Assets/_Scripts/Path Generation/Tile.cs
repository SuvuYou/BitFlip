using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public enum TileType { Wall, Path, DeadlyWall }
    public enum TileConnectionType { Single, Corner, T_Junction, Intersection }

    public struct TileData 
    {
        public TileType Type;

        public Direction PreviousFacingDirection;

        public int RouteIndices;
        public TileConnectionType ConnectionType;

        public bool IsBorder;
        public bool IsIncludedInDungeonRoom;
        public bool IsValid;

        public TileData(TileType type, Direction previousFacingDirection, TileConnectionType connectionType, bool isBorder, bool isIncludedInDungeonRoom)
        {
            Type = type;
            PreviousFacingDirection = previousFacingDirection;

            ConnectionType = connectionType;

            IsBorder = isBorder;
            IsIncludedInDungeonRoom = isIncludedInDungeonRoom;
            IsValid = true;
        }
    }

    public class Tile : ICloneable
    {
        public TileData StateData;

        private HashSet<Direction> _connections = new();

        public Tile(TileType type, Direction previousFacingDirection)
        {
            StateData = new TileData(type, previousFacingDirection, TileConnectionType.Single, false, false);
        }

        public void Invalidate() => StateData.IsValid = false;
        public void Revalidate() => StateData.IsValid = true;
        public void SetAsBorder() => StateData.IsBorder = true;
        public void SetAsDungeonRoomTile() => StateData.IsIncludedInDungeonRoom = true;

        public void SwitchType(TileType newType, Direction previousFacingDirection)  
        {
            if (StateData.Type == TileType.Path && newType == TileType.Wall)
            {
                _connections.Clear();

                SetupConnectionType();
            }

            StateData.Type = newType;
            StateData.PreviousFacingDirection = previousFacingDirection;
        }

        #region Connections

        public bool TryGetFollowingTilePosition(Vector2Int currentPosition, out Vector2Int nextPosition) 
        {
            nextPosition = new Vector2Int();

            if (_connections.Contains(StateData.PreviousFacingDirection))
            {
                nextPosition = currentPosition + StateData.PreviousFacingDirection.ToVector();

                return true;
            }
 
            foreach (var direction in _connections)
            {
                if (direction == StateData.PreviousFacingDirection.Opposite()) continue;

                nextPosition = currentPosition + direction.ToVector();

                return true;
            }

            return false;
        }

        public bool TryGetNextConnectedTilePosition(Vector2Int currentPosition, out Vector2Int nextPosition) 
        {
            nextPosition = new Vector2Int();

            foreach (var direction in _connections)
            {
                if (direction == StateData.PreviousFacingDirection.Opposite()) continue;

                nextPosition = currentPosition + direction.ToVector();

                return true;
            }

            return false;
        }

        public bool TryGetAvailableTurnConnections(out Direction direction) 
        {
            direction = Direction.None;

            foreach (var dir in DirectionExtentions.AllDirections)
            {
                direction = dir;

                if (!_connections.Contains(dir) && direction != StateData.PreviousFacingDirection) return true;
            }

            return false;
        }

        public bool IsConnectedToDirection(Direction direction) => _connections.Contains(direction);

        public void AddConnection(Direction direction)
        {
            _connections.Add(direction);

            SetupConnectionType();
        }

        public void RemoveConnection(Direction direction)
        {
            _connections.Remove(direction);

            SetupConnectionType();
        }

        private void SetupConnectionType()
        {
            int cornersCount = 0;

            foreach (var direction in DirectionExtentions.AllDirections)
            {
                if (_connections.Contains(direction) && _connections.Contains(direction.LocalRight()))
                {
                    cornersCount++;
                }
            }
            
            StateData.ConnectionType = cornersCount switch
            {
                0 => TileConnectionType.Single,
                1 => TileConnectionType.Corner,
                2 => TileConnectionType.T_Junction,
                _ => TileConnectionType.Intersection
            };
        }

        #endregion

        #region Cloning

        public void CloneStateData(Tile tile)
        {
            StateData = tile.StateData;
        }

        public object Clone()
        {
            Tile clone = new (this.StateData.Type, this.StateData.PreviousFacingDirection, this.StateData.RouteIndices)
            {
                StateData = this.StateData,
            };

            clone._connections = new HashSet<Direction>(this._connections);

            return clone;
        }

        #endregion
    }
}
