using System;
using System.Collections.Generic;
using System.Linq;
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
        public int StemLength { get; }

        public readonly Tile[,] Tiles;
        
        public int CurrentLargestRouteIndex { get; private set; }

        public readonly Validator PathValidator = new();
        public readonly SnapshotManager<Tile[,]> TilesSnapshotManager;

        public Tile GetTileByPosition(Vector2Int position) => Tiles[position.x, position.y];
        public Tile GetTileByPosition(int x, int y) => Tiles[x, y];

        public void SetTileReference(Vector2Int position, Tile tile) => Tiles[position.x, position.y] = tile;
        public void SetTileReference(int x, int y, Tile tile) => Tiles[x, y] = tile;

        Vector2Int MatrixLowerBounds, MatrixUpperBounds;

        public bool IsOutOfBounds(Vector2Int tilePosition) => !BoundsHelper.IsWithinBounds(tilePosition, MatrixLowerBounds, MatrixUpperBounds);
        public bool IsOutOfBounds(int x, int y) => !BoundsHelper.IsWithinBounds(x, y, MatrixLowerBounds, MatrixUpperBounds);

        Vector2Int PlacableAreaLowerBounds, PlacableAreaUpperBounds;

        public bool IsWithinPlacableArea(Vector2Int tilePosition) => BoundsHelper.IsWithinBounds(tilePosition, PlacableAreaLowerBounds, PlacableAreaUpperBounds);
        public bool IsWithinPlacableArea(int x, int y) => BoundsHelper.IsWithinBounds(x, y, PlacableAreaLowerBounds, PlacableAreaUpperBounds);

        Vector2Int StemLengthBorderLowerBounds, StemLengthBorderUpperBounds;

        public bool IsWithinStemLengthOfBorder(Vector2Int tilePosition) => BoundsHelper.IsWithinBounds(tilePosition, StemLengthBorderLowerBounds, StemLengthBorderUpperBounds);
        public bool IsWithinStemLengthOfBorder(int x, int y) => BoundsHelper.IsWithinBounds(x, y, StemLengthBorderLowerBounds, StemLengthBorderUpperBounds);

        public bool IsOnTheEdge(Vector2Int tilePosition) => tilePosition.x == MatrixLowerBounds.x || tilePosition.x == MatrixUpperBounds.x || tilePosition.y == MatrixLowerBounds.y || tilePosition.y == MatrixUpperBounds.y;
        public bool IsOnTheEdge(int x, int y) => x == MatrixLowerBounds.x || x == MatrixUpperBounds.x || y == MatrixLowerBounds.y || y == MatrixUpperBounds.y;

        public void SetTileRouteIndex(int x, int y, int routeIndex) 
        {
            if (Tiles[x, y].StateData.ConnectionType == TileConnectionType.Single || Tiles[x, y].StateData.ConnectionType == TileConnectionType.Corner) Tiles[x, y].SetRouteIndex(routeIndex);
            else Tiles[x, y].AddRouteIndex(routeIndex);
        }

        public void SetTile(int x, int y, TileType type, Direction facingDirection, int routeIndex = 1)
        {
            Tiles[x, y].SwitchType(type, facingDirection);

            SetupAdjacentConnections(x, y);

            SetTileRouteIndex(x, y, routeIndex);
        }

        public void SetTile(int x, int y, Tile tile)
        {
            Tiles[x, y].CloneStateData(tile);

            SetupAdjacentConnections(x, y);
        }

        public TilesMatrix(int width, int height, int stemLength, Vector2Int borderSize = default, int currentLargestRouteIndex = 1, bool shouldSetupDefaultTiles = true)
        {
            Width = width;
            Height = height;
            StemLength = stemLength;
            BorderSize = borderSize;

            CurrentLargestRouteIndex = currentLargestRouteIndex;

            TilesSnapshotManager = new (this);

            Tiles = new Tile[width, height];

            if (shouldSetupDefaultTiles)
            {
                SetupTiles();

                TilesSnapshotManager.Snapshot();
            }

            MatrixLowerBounds = Vector2Int.zero;
            MatrixUpperBounds = new (Width - 1, Height - 1);

            PlacableAreaLowerBounds = new (BorderSize.x, BorderSize.y);
            PlacableAreaUpperBounds = new (Width - 1 - BorderSize.x, Height - 1 - BorderSize.y);

            StemLengthBorderLowerBounds = new (BorderSize.x + (StemLength - 1), BorderSize.y + (StemLength - 1));
            StemLengthBorderUpperBounds = new (Width - 1 - BorderSize.x - (StemLength - 1), Height - 1 - BorderSize.y - (StemLength - 1));
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

        public HashSet<Vector2Int> GetSingleOccupiedPositions()
        {
            var positions = new HashSet<Vector2Int>();

            var GetSingleTileFunction = ConstructGetSingleOccupiedPositionFunction(positions);

            LoopThroughTiles(GetSingleTileFunction, LoopType.All);

            return positions;
        }

        public HashSet<Vector2Int> GetCornerTiles()
        {
            var corners = new HashSet<Vector2Int>();

            var GetCornerTileFunction = ConstructGetCornerTileFunction(corners);

            LoopThroughTiles(GetCornerTileFunction, LoopType.WithoutEdges);

            return corners;
        }

        public float GetPathPercentage() => GetOccupiedPositions().Count / (float)(Width * Height);

        public void InvalidateBorders() => LoopThroughTiles((int x, int y, Tile tile) => tile.SetAsBorder(), LoopType.OnlyBorders);

        public bool TryGetTwoConnectiveTiles(PseudoRandom.SystemRandomManager random, out Vector2Int cornerTilePosition, out Vector2Int singleTilePosition, out Direction lockedDirection)
        {
            cornerTilePosition = default;
            singleTilePosition = default;
            lockedDirection = default;

            var cornerTiles = GetCornerTiles();
            var pathTiles = GetSingleOccupiedPositions();
            var pathfinder = new Pathfinder(this);

            int limit = 10;
            int counter = 0;

            while (counter < limit)
            {
                counter++;

                var randomCornerTile = cornerTiles.ElementAt(random.GetRandomInt(0, cornerTiles.Count));

                if (!GetTileByPosition(randomCornerTile).TryGetAvailableTurnConnections(out Direction direction)) continue;
                
                lockedDirection = direction;

                pathTiles = pathTiles.Where(tilePosition => tilePosition.x % 2 == randomCornerTile.x % 2 && tilePosition.y % 2 == randomCornerTile.y % 2).ToHashSet();

                if (pathTiles.Count == 0) continue;

                var randPathTile = pathTiles.ElementAt(random.GetRandomInt(0, pathTiles.Count));

                if (!pathfinder.IsPathFindable(randomCornerTile, randPathTile)) continue;

                cornerTilePosition = randomCornerTile;
                singleTilePosition = randPathTile;

                return true;   
            }

            return false;
        }

        public TilesMatrix CopyTilesRegion((Vector2Int, Vector2Int) bounds, Vector2Int borderSize = default)
        {
            var (bottomLeft, topRight) = bounds;

            int width = topRight.x - bottomLeft.x;
            int height = topRight.y - bottomLeft.y;

            var clonedRegion = new TilesMatrix(width, height, StemLength, borderSize, CurrentLargestRouteIndex, shouldSetupDefaultTiles: false);

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
            Tiles[x, y] = new Tile(TileType.Wall, Direction.None);

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

        public void TraversePath(Vector2Int startPosition, HashSet<Vector2Int> blockedPositions, int routeIndex, Action<int, int, Tile> onTileVisited)
        {
            var visited = new HashSet<Vector2Int>();
            var stack = new Stack<Vector2Int>();

            stack.Push(startPosition);
            visited.Add(startPosition);
            visited.Concat(blockedPositions);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var currentTile = Tiles[current.x, current.y];

                foreach (var direction in DirectionExtentions.AllDirections)
                {
                    if (!currentTile.IsConnectedToDirection(direction)) continue;

                    var nextPos = current + direction.ToVector();

                    if (IsOutOfBounds(nextPos) || visited.Contains(nextPos)) continue;

                    if (GetTileByPosition(nextPos).StateData.Type != TileType.Path) continue;

                    if (!GetTileByPosition(nextPos).HasRouteIndex(routeIndex)) continue;

                    visited.Add(nextPos);
                    stack.Push(nextPos);

                    onTileVisited(nextPos.x, nextPos.y, Tiles[nextPos.x, nextPos.y]);
                }
            }
        }

        public void FollowPath(Vector2Int startPosition, HashSet<Vector2Int> blockedPositions, Action<int, int, Tile> onTileVisited)
        {
            var visited = new HashSet<Vector2Int>();
            var stack = new Stack<Vector2Int>();

            stack.Push(startPosition);
            visited.Add(startPosition);
            visited.Concat(blockedPositions);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var currentTile = Tiles[current.x, current.y];

                if (currentTile.TryGetFollowingTilePosition(current, out var nextPosition))
                {
                    if (IsOutOfBounds(nextPosition) || visited.Contains(nextPosition)) continue;

                    if (GetTileByPosition(nextPosition).StateData.Type != TileType.Path) continue;

                    visited.Add(nextPosition);
                    stack.Push(nextPosition);

                    onTileVisited(nextPosition.x, nextPosition.y, Tiles[nextPosition.x, nextPosition.y]);
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

        private Action<int, int, Tile> ConstructGetSingleOccupiedPositionFunction(HashSet<Vector2Int> occupiedPositions)
        { 
            return (int x, int y, Tile tile) => 
            {
                if (Tiles[x, y].StateData.Type == TileType.Path && Tiles[x, y].StateData.ConnectionType == TileConnectionType.Single && !Tiles[x, y].StateData.IsBorder)
                    occupiedPositions.Add(new Vector2Int(x, y));
            };
        }

        private Action<int, int, Tile> ConstructGetCornerTileFunction(HashSet<Vector2Int> corners)
        { 
            return (int x, int y, Tile tile) => 
            {
                if(Tiles[x, y].StateData.ConnectionType == TileConnectionType.Corner) 
                    corners.Add(new Vector2Int(x, y)); 
            };
        }
    }
}
