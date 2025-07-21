using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public class Map
    {
        public readonly TilesMatrix MapTiles;
        public readonly PathGenerator MapPathGenerator;
        public readonly PseudoRandom.SystemRandomManager _systemRandom;

        private IDungeonRoomFinder _dungeonRoomFinder = new VarietyDungeonRoomFinder();
        private IDungeonRoomPathConstructor _dungeonRoomPathConstructor;
        private IDungeonRoomTransformer _dungeonRoomTransformer = new DungeonRoomTransformer();

        public List<DungeonRoom> DungeonRooms { get; private set; }

        public int MaxNumberOfDungeonRooms { get; private set; }

        public Map()
        {
            var mapSettings = MapSettingsProvider.Instance.MapSettings;

            Vector2Int startPos = new (0 + mapSettings.MapBorderSize.x, 0 + mapSettings.MapBorderSize.y);
            Vector2Int endPos = new (mapSettings.MapWidth - 1 - mapSettings.MapBorderSize.x, mapSettings.MapHeight - 1 - mapSettings.MapBorderSize.y);

            MapTiles = new TilesMatrix(mapSettings.MapWidth, mapSettings.MapHeight, mapSettings.MapStemLength, mapSettings.MapBorderSize);

            MapPathGenerator = new PathGenerator(MapTiles, startPos, endPos);

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);

            MaxNumberOfDungeonRooms = mapSettings.MaxNumberOfDungeonRooms;

            _dungeonRoomPathConstructor = new VarietyDungeonRoomPathConstructor();
            DungeonRooms = new List<DungeonRoom>(MaxNumberOfDungeonRooms);
        }

        public void Generate()
        {
            MapPathGenerator.RandomWalk();

            // ExpandCorners();
            GenerateDungeonRooms();
        }

        private void GenerateDungeonRooms()
        {
            foreach (var pos in MapPathGenerator.Tiles.GetCornerTiles())
            {
                if (DungeonRooms.Count < MaxNumberOfDungeonRooms)
                {
                    if (!_dungeonRoomFinder.TryFindDungeonRoom(MapPathGenerator, pos, out DungeonRoom dungeonRoom)) continue;

                    dungeonRoom = _dungeonRoomPathConstructor.ConstructPath(dungeonRoom) as DungeonRoom;

                    // dungeonRoom = _dungeonRoomTransformer.TransformDungeonRoom(dungeonRoom);
                    
                    DungeonRooms.Add(dungeonRoom);
                }
            }
        }
    }
}
