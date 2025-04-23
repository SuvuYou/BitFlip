using UnityEngine;

namespace PathGeneration
{
    public class Map
    {
        public readonly TilesMatrix MapTiles;
        public readonly Path MapPath;
        public readonly PseudoRandom.SystemRandomManager _systemRandom;

        private IDungeonRoomFinder _dungeonRoomFinder = new DungeonRoomFinder();
        private IDungeonRoomPathConstructor _dungeonRoomPathConstructor;
        private IDungeonRoomTransformer _dungeonRoomTransformer = new DungeonRoomTransformer();

        public Vector2Int MaxRoomSize { get; private set; } 
        public Vector2Int MinRoomSize { get; private set; }
        public int MaxNumberOfDungeonRooms { get; private set; }
        public int NumberOfDungeonRooms { get; private set; }

        public Map(GameDataSO gameDataSO)
        {
            Vector2Int startPos = new (0 + gameDataSO.MapBorderSize.x, 0 + gameDataSO.MapBorderSize.y);
            Vector2Int endPos = new (gameDataSO.MapWidth - 1 - gameDataSO.MapBorderSize.x, gameDataSO.MapHeight - 1 - gameDataSO.MapBorderSize.y);

            MapTiles = new TilesMatrix(gameDataSO.MapWidth, gameDataSO.MapHeight, gameDataSO.MapStemLength, gameDataSO.MapBorderSize);

            MapPath = new Path(MapTiles, startPos, endPos, Direction.None, gameDataSO.MapStemLength);

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);

            MaxNumberOfDungeonRooms = gameDataSO.MaxNumberOfDungeonRooms;
            NumberOfDungeonRooms = 0;

            MaxRoomSize = gameDataSO.MaxDungeonRoomSize;
            MinRoomSize = gameDataSO.MinDungeonRoomSize;

            _dungeonRoomPathConstructor = new DungeonRoomPathConstructor(gameDataSO);
        }

        public void Generate()
        {
            MapPath.RandomWalk();

            // ExpandCorners();
            GenerateDungeonRooms();
        }

        private void ExpandCorners()
        {
            foreach (var pos in MapPath.Tiles.GetCornerTiles())
            {
                if (_systemRandom.GetRandomFloat() > 0.5f)
                {
                    var newPath = new Path(MapPath.Tiles, pos, pos, Direction.None, MapPath.StemLength);
                    newPath.RandomWalk();
                    MapPath.Tiles.MergeWithPath(newPath);
                }
            }
        }

        private void GenerateDungeonRooms()
        {
            foreach (var pos in MapPath.Tiles.GetCornerTiles())
            {
                if (_systemRandom.GetRandomFloat() > 0.5f && NumberOfDungeonRooms < MaxNumberOfDungeonRooms)
                {
                    if (!_dungeonRoomFinder.TryFindDungeonRoom(MapPath, pos, MinRoomSize, MaxRoomSize, out DungeonRoom dungeonRoom)) continue;

                    dungeonRoom = _dungeonRoomPathConstructor.ConstructPath(dungeonRoom);

                    // dungeonRoom = _dungeonRoomTransformer.TransformDungeonRoom(dungeonRoom);
                    
                    MapPath.Tiles.MergeWithDungeonRoom(dungeonRoom);

                    NumberOfDungeonRooms++;
                }
            }
        }
    }
}
