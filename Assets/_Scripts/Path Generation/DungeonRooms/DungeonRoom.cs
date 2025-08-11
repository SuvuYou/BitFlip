using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathGeneration
{
    public enum DungeonRoomType { DedlyWall, Doorswitch, FlipPuzzle }

    public class DungeonRoom : IDungeonRoom
    {
        private PseudoRandom.SystemRandomManager _random;

        public DungeonRoomType Type { get; private set; }

        public DungeonRoom(DungeonRoomType type, (Vector2Int, Vector2Int) bounds, List<Vector2Int> exitPositions)
        {
            Type = type;
            Bounds = bounds;
            GlobalExitPositions = exitPositions;

            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);
        }

        public TilesMatrix Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }
        public Vector2Int LowerBounds => Bounds.Item1;
        public Vector2Int UpperBounds => Bounds.Item2;

        public int Width => UpperBounds.x - LowerBounds.x + 1;
        public int Height => UpperBounds.y - LowerBounds.y + 1;

        public List<Vector2Int> GlobalExitPositions { get; private set; }
        public Dictionary<DungeonTilePosition, (DungeonTilePosition, Direction)> EnterExitPositionPairs { get; private set; } = new();

        public List<DungeonDoorData> Doors { get; private set; } = new();

        public DungeonRoomVariant OriginalVariant { get; private set; }
        public Dictionary<DungeonTilePosition, DungeonRoomVariant> VariantsPerEntrance = new();

        public void SetDungeonRoomVariant(DungeonRoomVariant variant)
        {
            Tiles.SetTilesDataFromMatrix(variant.Tiles);
        }

        public void SetTiles(TilesMatrix tiles)
        {
            Tiles = tiles;

            Tiles.LoopThroughTiles(SetTileToDungeonRoomTile, TilesMatrix.LoopType.All);

            Tiles.InvalidateBorders();

            OriginalVariant = new DungeonRoomVariant((Vector2Int.zero, Vector2Int.zero));

            OriginalVariant.SetTiles(Tiles.CopyTilesRegion((Vector2Int.zero, new Vector2Int(Width -1, Height -1)), Vector2Int.one, shouldCloneTiles: true));
        }

        private void SetTileToDungeonRoomTile (int x, int y, Tile tile) => tile.SetAsDungeonRoomTile();

        public Vector3Int GetRandomPathTilePosition()
        {
            var pathTiles = Tiles.GetOccupiedPositions();

            return (LowerBounds + pathTiles.ElementAt(_random.GetRandomInt(0, pathTiles.Count))).ToVector3WithZ(z: 0);
        }

        public void FindEnterExitPositionPairs() 
        {
            List<int> occupiedIndecies = new();

            for (int enterIndex = 0; enterIndex < GlobalExitPositions.Count; enterIndex++)
            {
                Vector2Int currentLocalExitPosition = new (GlobalExitPositions[enterIndex].x - LowerBounds.x, GlobalExitPositions[enterIndex].y - LowerBounds.y);

                Tiles.FollowPath(currentLocalExitPosition, Tiles.GetOccupiedPositions(), (int localX, int localY, Tile tile) => 
                { 
                    if (currentLocalExitPosition.x == localX && currentLocalExitPosition.y == localY) return;

                    var exitIndex = GlobalExitPositions.FindIndex(pos => pos.x - LowerBounds.x == localX && pos.y - LowerBounds.y == localY);

                    if (!occupiedIndecies.Contains(exitIndex) && exitIndex != -1)
                    {   
                        DungeonTilePosition enterPosition = new (currentLocalExitPosition, currentLocalExitPosition + LowerBounds);
                        DungeonTilePosition exitPosition = new (new Vector2Int(localX, localY), new Vector2Int(localX, localY) + LowerBounds);

                        EnterExitPositionPairs.Add(enterPosition, (exitPosition, Tiles.GetTileByPosition(currentLocalExitPosition).StateData.PreviousFacingDirection));

                        occupiedIndecies.Add(exitIndex);
                        occupiedIndecies.Add(enterIndex);
                    } 
                });
            }
        }

        public void SetupDungeonRoomVariants() 
        {
            VariantsPerEntrance.Clear();

            foreach ((DungeonTilePosition enterPosition, (DungeonTilePosition exitPosition, Direction lockedDirection)) in EnterExitPositionPairs)
            {
                var variant = new DungeonRoomVariant((Vector2Int.zero, Vector2Int.zero), enterPosition, exitPosition);

                TilesMatrix variantTiles = Tiles.CopyTilesRegion((Vector2Int.zero, new Vector2Int(Width - 1, Height - 1)), Vector2Int.one, shouldCloneTiles: true);

                variantTiles.ResetTiles();
                variant.SetTiles(variantTiles);

                var pathGenerator = new PathGenerator(variant.Tiles, enterPosition.Local, exitPosition.Local, lockedDirection);

                Debug.Log($"Generating path from {enterPosition} to {exitPosition}");
                pathGenerator.RandomWalk();

                VariantsPerEntrance.Add(enterPosition, variant);

                variant.Tiles.SetTileData(enterPosition.Local.x, enterPosition.Local.y, TileType.Door, Tiles.GetTileByPosition(enterPosition.Local).StateData.PreviousFacingDirection);
                variant.Tiles.SetTileData(exitPosition.Local.x, exitPosition.Local.y, TileType.Door, Tiles.GetTileByPosition(exitPosition.Local).StateData.PreviousFacingDirection);

                OriginalVariant.Tiles.SetTileData(enterPosition.Local.x, enterPosition.Local.y, TileType.Door, Tiles.GetTileByPosition(enterPosition.Local).StateData.PreviousFacingDirection);
                OriginalVariant.Tiles.SetTileData(exitPosition.Local.x, exitPosition.Local.y, TileType.Door, Tiles.GetTileByPosition(exitPosition.Local).StateData.PreviousFacingDirection);

                Doors.Add(new DungeonDoorData(DungeonDoorType.Entrance, enterPosition, variant, Tiles.GetTileByPosition(enterPosition.Local).StateData.PreviousFacingDirection));
                Doors.Add(new DungeonDoorData(DungeonDoorType.Exit, exitPosition, variant, Tiles.GetTileByPosition(exitPosition.Local).StateData.PreviousFacingDirection));
            }

            SetDungeonRoomVariant(OriginalVariant);
        }    
    }
}
