using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathGeneration
{
    public enum DungeonRoomType { DedlyWall, Doorswitch, FlipPuzzle }

    public class DungeonRoom
    {
        private PseudoRandom.SystemRandomManager _random;

        public DungeonRoomType Type { get; private set; }

        public DungeonRoom(DungeonRoomType type, (Vector2Int, Vector2Int) bounds, List<Vector2Int> exitPositions)
        {
            Type = type;
            Bounds = bounds;
            ExitPositions = exitPositions;

            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);
        }

        public TilesMatrix Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }
        public Vector2Int LowerBounds => Bounds.Item1;
        public Vector2Int UpperBounds => Bounds.Item2;

        public int Width => UpperBounds.x - LowerBounds.x + 1;
        public int Height => UpperBounds.y - LowerBounds.y + 1;

        public List<Vector2Int> ExitPositions { get; private set; }
        public List<(Vector2Int, Vector2Int, Direction)> EnterExitPositionPairs { get; private set; } = new();

        public DungeonRoomVariant OriginalVariant { get; private set; }
        public Dictionary<(Vector2Int, Vector2Int), DungeonRoomVariant> VariantsPerEnter = new();

        int currentVariantIndex = 0;

        public void SetDungeonRoomVariant()
        {
            currentVariantIndex += 1;

            if (currentVariantIndex > VariantsPerEnter.Count)
                currentVariantIndex = 0;

            DungeonRoomVariant variantToSet = currentVariantIndex == 0 ? OriginalVariant : VariantsPerEnter.ElementAt(currentVariantIndex - 1).Value;

            Tiles.SetTilesDataFromMatrix(variantToSet.Tiles);
        }

        public void SetTiles(TilesMatrix tiles)
        {
            Tiles = tiles;

            Tiles.LoopThroughTiles(SetTileToDungeonRoomTile, TilesMatrix.LoopType.All);

            Tiles.InvalidateBorders();

            OriginalVariant = new DungeonRoomVariant((Vector2Int.zero, Vector2Int.zero));

            OriginalVariant.SetTiles(Tiles.CopyTilesRegion((Vector2Int.zero, new Vector2Int(Width -1, Height -1)), shouldCloneTiles: true));
        }

        private void SetTileToDungeonRoomTile (int x, int y, Tile tile) => tile.SetAsDungeonRoomTile();

        public Vector3Int GetRandomPathTilePosition()
        {
            var pathTiles = Tiles.GetOccupiedPositions();

            return (LowerBounds + pathTiles.ElementAt(_random.GetRandomInt(0, pathTiles.Count))).ToVector3WithZ(z: 0);
        }

        public void FindEnterExitPositionPairs() 
        {
            for (int i = 0; i < ExitPositions.Count; i++)
            {
                Vector2Int currentExitPosition = new (ExitPositions[i].x - LowerBounds.x, ExitPositions[i].y - LowerBounds.y);

                Tiles.FollowPath(currentExitPosition, Tiles.GetOccupiedPositions(), (int x, int y, Tile tile) => 
                { 
                    if (currentExitPosition.x == x && currentExitPosition.y == y) return;

                    if (ExitPositions.Any(pos => pos.x - LowerBounds.x == x && pos.y - LowerBounds.y == y))
                    {   
                        EnterExitPositionPairs.Add((currentExitPosition, new Vector2Int(x, y), Tiles.GetTileByPosition(currentExitPosition).StateData.PreviousFacingDirection));
                    } 
                });
            }
        }

        public void SetupDungeonRoomVariants() 
        {
            VariantsPerEnter.Clear();

            foreach ((Vector2Int enter, Vector2Int exit, Direction lockedDiirectiion) in EnterExitPositionPairs)
            {
                var variant = new DungeonRoomVariant((Vector2Int.zero, Vector2Int.zero), enter, exit);

                variant.SetTiles(Tiles.CopyTilesRegion((Vector2Int.zero, new Vector2Int(Width - 1, Height - 1)), shouldCloneTiles: true), shouldResetTiles: true);

                var newPath = new Path(variant.Tiles, enter, exit, lockedDiirectiion);

                Debug.Log($"Generating path from {enter} to {exit}");
                newPath.RandomWalk();

                VariantsPerEnter.Add((enter, exit), variant);
            }
        }
    }
}
