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

        public int MaxNumberOfDungeonRooms { get; private set; }

        public Map()
        {
            var mapSettings = MapSettingsProvider.Instance.MapSettings;

            Vector2Int startPos = new (0 + mapSettings.MapBorderSize.x, 0 + mapSettings.MapBorderSize.y);
            Vector2Int endPos = new (mapSettings.MapWidth - 1 - mapSettings.MapBorderSize.x, mapSettings.MapHeight - 1 - mapSettings.MapBorderSize.y);

            MapTiles = new TilesMatrix(mapSettings.MapWidth, mapSettings.MapHeight, mapSettings.MapStemLength);

            MapPath = new Path(MapTiles, startPos, endPos);

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);

            MaxNumberOfDungeonRooms = mapSettings.MaxNumberOfDungeonRooms;

            _dungeonRoomPathConstructor = new VarietyDungeonRoomPathConstructor();
            _dungeonRooms = new List<DungeonRoom>(MaxNumberOfDungeonRooms);
        }

        public void Generate()
        {
            MapPath.RandomWalk();

            // ExpandCorners();
            GenerateDungeonRooms();
        }

        public void SetupDungeonRoomVariants()
        {
            foreach (var dungeonRoom in _dungeonRooms)
            {
                dungeonRoom.SetDungeonRoomVariant();
            }
        }

        private void GenerateDungeonRooms()
        {
            foreach (var pos in MapPath.Tiles.GetCornerTiles())
            {
                if (_dungeonRooms.Count < MaxNumberOfDungeonRooms)
                {
                    if (!_dungeonRoomFinder.TryFindDungeonRoom(MapPath, pos, out DungeonRoom dungeonRoom)) continue;

                    dungeonRoom = _dungeonRoomPathConstructor.ConstructPath(dungeonRoom);

                    // dungeonRoom = _dungeonRoomTransformer.TransformDungeonRoom(dungeonRoom);
                    
                    _dungeonRooms.Add(dungeonRoom);
                }
            }
        }

        public DungeonRoom GetRandomDungeonRoom() => _dungeonRooms[_systemRandom.GetRandomInt(0, _dungeonRooms.Count)];
    }
}
