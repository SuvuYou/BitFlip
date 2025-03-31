using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge.Xml;
using UnityEngine;

namespace PathGeneration
{
    public struct Tile
    {
        public enum TileType 
        {
            Wall = 1,
            Path = 2
        }

        public enum ConnectionDirection
        {
            Top = 0,
            Right = 1,
            Bottom = 2,
            Left = 3
        }

        public TileType Type { get; private set; }
        public bool IsCorner { get; private set; }
        public bool IsValid { get; private set; }
        public bool[] Connections { get; private set; }

        public Tile(TileType type)
        {
            Type = type;
            IsCorner = false;
            IsValid = true;
            Connections = new bool[4];
        }

        public void Invalidate() => IsValid = false;

        public void AddConnection(ConnectionDirection direction) 
        {
            Connections[(int)direction] = true;
            CheckForCorner();
        }

        private void CheckForCorner()
        {
            for (int i = 0; i < Connections.Length - 1; i++)
            {
                if (Connections[i] && Connections[i + 1]) IsCorner = true;
            }

            if (Connections[0] && Connections[^1]) IsCorner = true;
        }
    }

    public class Map
    {
        private Path _path;

        public Map(int mapWidth, int mapHeight)
        {
            _path = new Path(mapWidth, mapHeight, new Vector2Int(0, 0), new Vector2Int(mapWidth - 1, mapHeight - 1));

            _path.RandomWalk();

            ExpandCorners();
        }

        private void MergePaths(Path mainPath, Path additivePath)
        {            
            mainPath.ConnectPath(additivePath);
        }

        private void ExpandCorners()
        {
            for (int x = 1; x < _path.Tiles.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < _path.Tiles.GetLength(1) - 1; y++)
                {
                    if (_path.Tiles[x, y].IsCorner && Random.value > 0.5f)
                    {
                        Vector2Int cornerPosition = new(x, y);

                        var cornerPath = new Path(_path.Tiles.GetLength(0), _path.Tiles.GetLength(1), cornerPosition, cornerPosition);
                        
                        for (int m = 1; m < _path.Tiles.GetLength(0) - 1; m++)
                        {
                            for (int n = 1; n < _path.Tiles.GetLength(1) - 1; n++)
                            {
                                if (_path.Tiles[m, n].Type == Tile.TileType.Path) cornerPath.AddBannedPosition(new Vector2Int(m, n));
                            }
                        }

                        cornerPath.RandomWalk();

                        MergePaths(_path, cornerPath);
                    }
                }
            }
        }
    }

    public class Path
    {
        public enum Direction
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        private Dictionary<Direction, Vector2Int> _directionsLookup = new()
        {
            { Direction.Up, new Vector2Int(0, 1) },
            { Direction.Right, new Vector2Int(1, 0) },
            { Direction.Down, new Vector2Int(0, -1) },
            { Direction.Left, new Vector2Int(-1, 0) }
        };

        public Tile[,] Tiles { get; private set; }
        public Vector2Int StartPosition, EndPosition, CurrentPosition;

        public List<Vector2Int> _bannedPositions = new();

        private Stack<Vector2Int> pathStack = new();
        private System.Random random = new();

        public Path(int width, int height, Vector2Int startPosition, Vector2Int endPosition)
        {
            Tiles = new Tile[width, height];

            StartPosition = startPosition;
            EndPosition = endPosition;
            CurrentPosition = startPosition;

            Tiles[StartPosition.x, StartPosition.y] = new Tile(Tile.TileType.Path);

            pathStack.Push(CurrentPosition);
        }

        public void AddBannedPosition(Vector2Int position)
        {
            _bannedPositions.Add(position);
        }

        public void ConnectPath(Path additivePath)
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (additivePath.Tiles[x, y].Type == Tile.TileType.Path && Tiles[x, y].Type != Tile.TileType.Path)
                    {
                        Tiles[x, y] = new Tile(Tile.TileType.Path);

                        foreach (var dir in _directionsLookup)
                        {
                            int neighbourPosX = x + dir.Value.x;
                            int neighbourPosY = x + dir.Value.y;

                            if (Tiles[neighbourPosX, neighbourPosY].Type == Tile.TileType.Path)
                            {
                                Tiles[x, y].AddConnection((Tile.ConnectionDirection)dir.Key);
                                Tiles[neighbourPosX, neighbourPosY].AddConnection((Tile.ConnectionDirection)(((int)dir.Key + 2) % 4)); // Connect back
                            }
                        }
                    }
                }
            }
        }

        public void RandomWalk()
        {
            while (CurrentPosition != EndPosition)
            {
                List<Direction> validMoves = GetValidMoves(CurrentPosition);
                
                if (validMoves.Count == 0) // Dead end, backtrack
                {
                    if (pathStack.Count > 0)
                    {
                        Tiles[CurrentPosition.x, CurrentPosition.y].Invalidate();
                        CurrentPosition = pathStack.Pop();
                    }
                    else break;
                }
                else
                {
                    Direction chosenMove = validMoves[random.Next(validMoves.Count)];
                    Vector2Int nextPosition = CurrentPosition + _directionsLookup[chosenMove];
                    
                    Tiles[nextPosition.x, nextPosition.y] = new Tile(Tile.TileType.Path);
                    Tiles[CurrentPosition.x, CurrentPosition.y].AddConnection((Tile.ConnectionDirection)chosenMove);
                    Tiles[nextPosition.x, nextPosition.y].AddConnection((Tile.ConnectionDirection)(((int)chosenMove + 2) % 4)); // Connect back
                    
                    pathStack.Push(CurrentPosition);
                    CurrentPosition = nextPosition;
                }
            }
        }

        private List<Direction> GetValidMoves(Vector2Int position)
        {
            List<Direction> validMoves = new();

            foreach (var dir in _directionsLookup)
            {
                Vector2Int nextPos = position + dir.Value;

                if (IsValidMove(nextPos))
                    validMoves.Add(dir.Key);
            }

            return validMoves;
        }

        private bool IsValidMove(Vector2Int pos)
        {   
            if (_bannedPositions.Contains(pos)) return false;

            if (pos.x < 0 || pos.y < 0 || pos.x >= Tiles.GetLength(0) || pos.y >= Tiles.GetLength(1)) return false;

            if (Tiles[pos.x, pos.y].IsCorner || !Tiles[pos.x, pos.y].IsValid) return false;

            return  true;
        }
    }
}
