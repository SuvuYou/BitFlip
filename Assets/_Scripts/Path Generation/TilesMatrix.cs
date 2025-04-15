using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public class TilesMatrix : ISnapshotable<Tile[,]>
    {
        public enum LoopType { All, WithoutEdges, OnlyEdges, WithoutBorders, OnlyBorders };

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

        public int Width { get; }
        public int Height { get; }
        public Vector2Int BorderSize { get; }

        public readonly Tile[,] Tiles;

        public readonly Validator PathValidator = new();
        public readonly SnapshotManager<Tile[,]> TilesSnapshotManager;

        public Tile GetTileByPosition(Vector2Int position) => Tiles[position.x, position.y];
        public Tile GetTileByPosition(int x, int y) => Tiles[x, y];

        public void SetTileReference(Vector2Int position, Tile tile) => Tiles[position.x, position.y] = tile;
        public void SetTileReference(int x, int y, Tile tile) => Tiles[x, y] = tile;

        public bool IsOutOfBounds(Vector2Int tilePosition) => tilePosition.x < 0 || tilePosition.y < 0 || tilePosition.x >= Width || tilePosition.y >= Height;
        public bool IsOnTheEdge(Vector2Int tilePosition) => tilePosition.x == BorderSize.x || tilePosition.y == BorderSize.y || tilePosition.x == Width - 1 - BorderSize.x || tilePosition.y == Height - 1 - BorderSize.y;

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

        public TilesMatrix(int width, int height, Vector2Int borderSize = default, bool shouldSetupDefaultTiles = true)
        {
            Width = width;
            Height = height;
            BorderSize = borderSize;

            TilesSnapshotManager = new (this);

            Tiles = new Tile[width, height];

            if (shouldSetupDefaultTiles)
                SetupTiles();

            TilesSnapshotManager.Snapshot();
        }

        public void MergeWithPath(Path other)
        {
            var SetPathTileFunction = ConstructSetPathTileFunction(other.Tiles);

            LoopThroughTiles(SetPathTileFunction, LoopType.All);
        }

        public void MergeWithDungeonRoom(DungeonRoom dungeonRoom)
        {
            (Vector2Int lowerBound, Vector2Int upperBound) = dungeonRoom.Bounds;

            for (int x = lowerBound.x; x < upperBound.x; x++)
            {
                for (int y = lowerBound.y; y < upperBound.y; y++)
                {
                    SetTile(x, y, dungeonRoom.Tiles.GetTileByPosition(x - lowerBound.x, y - lowerBound.y));

                    Tiles[x,y].SetAsDungeonRoomTile();
                }
            }
        }

        public HashSet<Vector2Int> GetOccupiedPositions()
        {
            var positions = new HashSet<Vector2Int>();

            var GetCornerTileFunction = ConstructGetOccupiedPositionsFunction(positions);

            LoopThroughTiles(GetCornerTileFunction, LoopType.All);

            return positions;
        }

        public HashSet<(Vector2Int, Tile)> GetCornerTiles()
        {
            var corners = new HashSet<(Vector2Int, Tile)>();

            var GetCornerTileFunction = ConstructGetCornerTileFunction(corners);

            LoopThroughTiles(GetCornerTileFunction, LoopType.WithoutEdges);

            return corners;
        }

        public TilesMatrix CopyTilesRegion((Vector2Int, Vector2Int) bounds)
        {
            var (bottomLeft, topRight) = bounds;

            int width = topRight.x - bottomLeft.x;
            int height = topRight.y - bottomLeft.y;

            var clonedRegion = new TilesMatrix(width, height, borderSize: Vector2Int.zero, shouldSetupDefaultTiles: false);

            var SetRegionTileFunction = ConstructSetRegionTileFunction(bounds, clonedRegion);

            clonedRegion.LoopThroughTiles(SetRegionTileFunction, LoopType.All);

            return clonedRegion;
        }

        private void SetupTiles()
        {
            LoopThroughTiles(SetupDefaultTile, LoopType.All);
        }

        private void SetupDefaultTile(int x, int y, Tile tile)
        {
            Tiles[x, y] = new Tile(TileType.Wall);

            if (x < BorderSize.x || y < BorderSize.y || x >= (Width - BorderSize.x) || y >= (Height - BorderSize.y))
            {
                Tiles[x, y].SetAsBorder();
            }
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

        public void LoopThroughTiles(Action<int, int, Tile> action, LoopType loopType = LoopType.All) 
        {
            switch (loopType)
            {                
                case LoopType.All:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.WithoutEdges:
                    for (int x = 1; x < Width - 1; x++)
                    {
                        for (int y = 1; y < Height - 1; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.OnlyEdges:
                    for (int x = 0; x < Width; x++)
                    {
                        action(x, 0, Tiles[x, 0]); 
                        action(x, Height - 1, Tiles[x, Height - 1]); 
                    }

                    for (int y = 1; y < Height - 1; y++)
                    {
                        action(0, y, Tiles[0, y]);
                        action(Width - 1, y, Tiles[Width - 1, y]); 
                    } 
                    break;
                case LoopType.WithoutBorders:
                    for (int x = BorderSize.x; x < Width - BorderSize.x; x++)
                    {
                        for (int y = BorderSize.y; y < Height - BorderSize.y; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.OnlyBorders:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < BorderSize.y; y++)
                        {
                            action(x, y, Tiles[x, y]); 
                            action(x, Height - y - 1, Tiles[x, Height - y - 1]); 
                        }
                    }

                    for (int y = BorderSize.y; y < Height - BorderSize.y - 1; y++)
                    {
                        for (int x = 0; x < BorderSize.x; x++)
                        {
                            action(x, y, Tiles[x, y]); 
                            action(x, Height - y - 1, Tiles[x, Height - y - 1]); 
                        }
                    }
                    break;
            }
        }

        private Action<int, int, Tile> ConstructSetRegionTileFunction((Vector2Int, Vector2Int) bounds, TilesMatrix clonedRegion)
        { 
            var (bottomLeft, topRight) = bounds;

            return (x, y, tile) =>
            {
                int posX = x + bottomLeft.x;
                int posY = y + bottomLeft.y;

                clonedRegion.SetTileReference(x, y, GetTileByPosition(posX, posY));
            };
        }

        private Action<int, int, Tile> ConstructSetPathTileFunction(TilesMatrix otherMatrix)
        { 
            return (int x, int y, Tile tile) => 
            {
                if (otherMatrix.GetTileByPosition(x, y).StateData.Type == TileType.Path)
                {
                    SetTile(x, y, otherMatrix.GetTileByPosition(x, y));
                }
            };
        }

        private Action<int, int, Tile> ConstructGetOccupiedPositionsFunction(HashSet<Vector2Int> occupiedPositions)
        { 
            return (int x, int y, Tile tile) => 
            {
                if (Tiles[x, y].StateData.Type == TileType.Path)
                    occupiedPositions.Add(new Vector2Int(x, y));
            };
        }

        private Action<int, int, Tile> ConstructGetCornerTileFunction(HashSet<(Vector2Int, Tile)> corners)
        { 
            return (int x, int y, Tile tile) => 
            {
                if(Tiles[x, y].StateData.IsCorner) 
                    corners.Add((new Vector2Int(x, y), Tiles[x, y])); 
            };
        }
    }
}
