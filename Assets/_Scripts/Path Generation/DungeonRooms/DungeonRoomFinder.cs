using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoomFinder
    {
        public bool TryFindDungeonRoom(Path path, Vector2Int forceContainPosition, Vector2Int minSize, Vector2Int maxSize, out DungeonRoom dungeonRoom);
    }

    public class DungeonRoomFinder : IDungeonRoomFinder
    {
        private PseudoRandom.SystemRandomManager _random;

        public DungeonRoomFinder() 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);
        }

        public bool TryFindDungeonRoom(Path path, Vector2Int forceContainPosition, Vector2Int minSize, Vector2Int maxSize, out DungeonRoom dungeonRoom)
        {
            dungeonRoom = null;
            
            if (path.Tiles.GetTileByPosition(forceContainPosition.x, forceContainPosition.y).StateData.IsIncludedInDungeonRoom) return false;

            List<(Vector2Int bottomLeft, Vector2Int topRight)> validCandidates = new();

            for (int width = minSize.x; width <= maxSize.x; width++)
            {
                for (int height = minSize.y; height <= maxSize.y; height++)
                {
                    for (int offsetX = 0; offsetX < width; offsetX++)
                    {
                        for (int offsetY = 0; offsetY < height; offsetY++)
                        {
                            Vector2Int bottomLeft = new (forceContainPosition.x - offsetX, forceContainPosition.y - offsetY);
                            Vector2Int topRight = new (bottomLeft.x + width, bottomLeft.y + height);

                            bool candidateValid = true;

                            for (int i = 0; i < width; i++)
                            {
                                for (int j = 0; j < height; j++)
                                {
                                    int posX = bottomLeft.x + i;
                                    int posY = bottomLeft.y + j;

                                    if (posX < 0 || posY < 0 || posX >= path.Tiles.Width || posY >= path.Tiles.Height)
                                    {
                                        candidateValid = false;
                                        break;
                                    }

                                    if (path.Tiles.GetTileByPosition(posX, posY).StateData.IsIncludedInDungeonRoom || path.Tiles.GetTileByPosition(posX, posY).StateData.IsBorder)
                                    {
                                        candidateValid = false;
                                        break;
                                    }
                                }
                                if (!candidateValid)
                                    break;
                            }

                            if (!candidateValid)
                                continue;

                            if (path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y + height - 1).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + height - 1).StateData.Type == TileType.Path)
                                continue;

                            int edgePathCount = 0;

                            for (int i = 0; i < width; i++)
                            {
                                if (path.Tiles.GetTileByPosition(bottomLeft.x + i, bottomLeft.y).StateData.Type == TileType.Path)
                                    edgePathCount++;

                                if (path.Tiles.GetTileByPosition(bottomLeft.x + i, bottomLeft.y + height - 1).StateData.Type == TileType.Path)
                                    edgePathCount++;
                            }

                            for (int j = 1; j < height - 1; j++)
                            {
                                if (path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y + j).StateData.Type == TileType.Path)
                                    edgePathCount++;
                                if (path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + j).StateData.Type == TileType.Path)
                                    edgePathCount++;
                            }

                            if (edgePathCount == 2)
                            {
                                validCandidates.Add((bottomLeft, topRight));                                
                            }
                        }
                    }
                }
            }

            if (validCandidates.Count > 0)
            {
                var candidate = validCandidates[_random.GetRandomInt(0, validCandidates.Count)];

                dungeonRoom = new DungeonRoom(DungeonRoomType.DedlyWall, (candidate.bottomLeft, candidate.topRight));

                dungeonRoom.SetTiles(path.Tiles.CopyTilesRegion(candidate));

                return true;
            }

            return false;
        }
    }
}
