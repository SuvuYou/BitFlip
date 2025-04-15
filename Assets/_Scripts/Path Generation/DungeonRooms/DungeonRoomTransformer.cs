using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoomTransformer
    {
        public abstract DungeonRoom TransformDungeonRoom(DungeonRoom dungeonRoom);
    }

    public class DungeonRoomTransformer : IDungeonRoomTransformer
    {
        private DungeonRoom _dungeonRoom;
        private List<Tile> _wallTiles = new ();

        public DungeonRoom TransformDungeonRoom(DungeonRoom dungeonRoom)
        {
            _dungeonRoom = dungeonRoom;

            dungeonRoom.Tiles.LoopThroughTiles(SetDeadlyBorder, TilesMatrix.LoopType.OnlyEdges);
            dungeonRoom.Tiles.LoopThroughTiles(CollectWallTiles, TilesMatrix.LoopType.WithoutEdges);
            dungeonRoom.Tiles.LoopThroughTiles(OverrideToPathTiles, TilesMatrix.LoopType.WithoutEdges);

            foreach (var wallTile in _wallTiles)
                wallTile.SwitchType(TileType.Wall);

            return dungeonRoom;
        }

        private void SetDeadlyBorder(int x, int y, Tile tile)
        {
            if (tile.StateData.Type == TileType.Path) return;

            tile.SwitchType(TileType.DeadlyWall);
        }

        private void CollectWallTiles(int x, int y, Tile tile)
        {
            Debug.Log(tile.StateData.Type == TileType.Path);

            if (tile.StateData.Type == TileType.Path && tile.StateData.IsCorner)
            {
                var directionVector = tile.StateData.FacingDirection.ToVector();
                _wallTiles.Add(_dungeonRoom.Tiles.GetTileByPosition(x + directionVector.x, y + directionVector.y));
            }
        }

        private void OverrideToPathTiles(int x, int y, Tile tile)
        {
            tile.SwitchType(TileType.Path, tile.StateData.FacingDirection);
        }
    }
}
