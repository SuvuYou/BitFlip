using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public class Map
    {
        public readonly TilesMatrix MapTiles;
        public readonly Path MapPath;
        public readonly PseudoRandom.SystemRandomManager _systemRandom;

        private IDungeonRoomFinder _dungeonRoomFinder = new VarietyDungeonRoomFinder();
        private IDungeonRoomPathConstructor _dungeonRoomPathConstructor;
        private IDungeonRoomTransformer _dungeonRoomTransformer = new DungeonRoomTransformer();

        private List<DungeonRoom> _dungeonRooms;

        public Vector2Int MaxRoomSize { get; private set; } 
        public Vector2Int MinRoomSize { get; private set; }
        public Vector2Int RoomBorderSize { get; private set; }
        public int MaxNumberOfDungeonRooms { get; private set; }

        public Map(GameDataSO gameDataSO)
        {
            Vector2Int startPos = new (0 + gameDataSO.MapBorderSize.x, 0 + gameDataSO.MapBorderSize.y);
            Vector2Int endPos = new (gameDataSO.MapWidth - 1 - gameDataSO.MapBorderSize.x, gameDataSO.MapHeight - 1 - gameDataSO.MapBorderSize.y);

            MapTiles = new TilesMatrix(gameDataSO.MapWidth, gameDataSO.MapHeight, gameDataSO.MapStemLength, gameDataSO.MapBorderSize);

            MapPath = new Path(MapTiles, startPos, endPos, Direction.None, gameDataSO.MapStemLength);

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);

            MaxNumberOfDungeonRooms = gameDataSO.MaxNumberOfDungeonRooms;

            MaxRoomSize = gameDataSO.MaxDungeonRoomSize;
            MinRoomSize = gameDataSO.MinDungeonRoomSize;
            RoomBorderSize = Vector2Int.one;

            _dungeonRoomPathConstructor = new VarietyDungeonRoomPathConstructor(gameDataSO);
            _dungeonRooms = new List<DungeonRoom>(MaxNumberOfDungeonRooms);
        }

        public void Generate()
        {
            MapPath.RandomWalk();

            // ExpandCorners();
            GenerateDungeonRooms();
        }

        private void GenerateDungeonRooms()
        {
            foreach (var pos in MapPath.Tiles.GetCornerTiles())
            {
                if (_dungeonRooms.Count < MaxNumberOfDungeonRooms)
                {
                    if (!_dungeonRoomFinder.TryFindDungeonRoom(MapPath, pos, MinRoomSize, MaxRoomSize, RoomBorderSize, out DungeonRoom dungeonRoom)) continue;

                    dungeonRoom = _dungeonRoomPathConstructor.ConstructPath(dungeonRoom);

                    // dungeonRoom = _dungeonRoomTransformer.TransformDungeonRoom(dungeonRoom);
                    
                    _dungeonRooms.Add(dungeonRoom);
                }

                Debug.Log($"Dungeon rooms: {_dungeonRooms.Count}");
            }
        }

        public DungeonRoom GetRandomDungeonRoom() => _dungeonRooms[_systemRandom.GetRandomInt(0, _dungeonRooms.Count)];
    }
}
