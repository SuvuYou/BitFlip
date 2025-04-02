using System;
using System.Collections.Generic;

namespace PathGeneration
{
    public enum TileType { Wall, Path }

    public class Tile : ICloneable
    {
        public TileType Type { get; private set; }
        public bool IsCorner { get; private set; }
        public bool IsValid { get; private set; } = true;
        private HashSet<Direction> _connections = new();

        public Tile(TileType type) => Type = type;

        public void Invalidate() => IsValid = false;

        public void Revalidate() => IsValid = true;

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

        public void SwitchType(TileType newType) 
        {
            if (Type == TileType.Path && newType == TileType.Wall)
            {
                _connections.Clear();
                CheckIsCorner();
            }

            Type = newType;
        }

        public bool IsConnectedToDirection(Direction direction) => _connections.Contains(direction);

        private void CheckIsCorner()
        {
            IsCorner = (_connections.Contains(Direction.Up) && _connections.Contains(Direction.Right)) ||
                       (_connections.Contains(Direction.Right) && _connections.Contains(Direction.Down)) ||
                       (_connections.Contains(Direction.Down) && _connections.Contains(Direction.Left)) ||
                       (_connections.Contains(Direction.Left) && _connections.Contains(Direction.Up));
        }

        public object Clone()
        {
            Tile clone = new Tile(this.Type)
            {
                IsValid = this.IsValid,
                IsCorner = this.IsCorner
            };

            clone._connections = new HashSet<Direction>(this._connections);

            return clone;
        }
    }
}
