using System;
using System.Collections.Generic;

namespace PathGeneration
{
    public enum TileType { Wall, Path, DeadlyWall }

    public struct TileData 
    {
        public TileType Type;

        public Direction FacingDirection;

        public bool IsBorder;
        public bool IsCorner;
        public bool IsIncludedInDungeonRoom;
        public bool IsValid;

        public TileData(TileType type, Direction facingDirection, bool isBorder, bool isCorner, bool isIncludedInDungeonRoom)
        {
            Type = type;
            FacingDirection = facingDirection;
            IsBorder = isBorder;
            IsCorner = isCorner;
            IsIncludedInDungeonRoom = isIncludedInDungeonRoom;
            IsValid = true;
        }
    }

    public class Tile : ICloneable
    {
        public TileData StateData;

        private HashSet<Direction> _connections = new();

        public Tile(TileType type, Direction facingDirection = Direction.Up)
        {
            StateData = new TileData(type, facingDirection, false, false, false);
        }

        public void Invalidate() => StateData.IsValid = false;
        public void Revalidate() => StateData.IsValid = true;
        public void SetAsBorder() => StateData.IsBorder = true;
        public void SetAsDungeonRoomTile() => StateData.IsIncludedInDungeonRoom = true;

        public void SwitchType(TileType newType, Direction facingDirection = Direction.Up)  
        {
            if (StateData.Type == TileType.Path && newType == TileType.Wall)
            {
                _connections.Clear();
                CheckIsCorner();
            }

            StateData.Type = newType;
            StateData.FacingDirection = facingDirection;
        }

        public bool IsConnectedToDirection(Direction direction) => _connections.Contains(direction);

        public void AddConnection(Direction direction)
        {
            _connections.Add(direction);
            CheckIsCorner();
        }

        public void RemoveConnection(Direction direction)
        {
            _connections.Remove(direction);
            CheckIsCorner();
        }

        private void CheckIsCorner()
        {
            StateData.IsCorner = (_connections.Contains(Direction.Up) && _connections.Contains(Direction.Right)) ||
                       (_connections.Contains(Direction.Right) && _connections.Contains(Direction.Down)) ||
                       (_connections.Contains(Direction.Down) && _connections.Contains(Direction.Left)) ||
                       (_connections.Contains(Direction.Left) && _connections.Contains(Direction.Up));
        }

        public void CloneStateData(Tile tile)
        {
            StateData = tile.StateData;
        }

        public object Clone()
        {
            Tile clone = new (this.StateData.Type)
            {
                StateData = this.StateData,
            };

            clone._connections = new HashSet<Direction>(this._connections);

            return clone;
        }
    }
}
