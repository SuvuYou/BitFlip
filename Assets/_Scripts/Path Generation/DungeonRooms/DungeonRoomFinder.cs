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
        public bool TryFindDungeonRoom(Path path, Vector2Int forceContainPosition, Vector2Int minSize, Vector2Int maxSize, out DungeonRoom dungeonRoom)
        {
            dungeonRoom = null;
            
            if (path.GetTileByPosition(forceContainPosition.x, forceContainPosition.y).IsIncludedInDungeonRoom) return false;

            List<(Vector2Int bottomLeft, Vector2Int topRight)> validCandidates = new();

            for (int width = minSize.x; width <= minSize.y; width++)
            {
                for (int height = maxSize.x; height <= maxSize.y; height++)
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

                                    if (posX < 0 || posY < 0 || posX >= path.Width || posY >= path.Height)
                                    {
                                        candidateValid = false;
                                        break;
                                    }

                                    if (path.GetTileByPosition(posX, posY).IsIncludedInDungeonRoom)
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

                            if (path.GetTileByPosition(bottomLeft.x, bottomLeft.y).Type == TileType.Path
                                || path.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y).Type == TileType.Path
                                || path.GetTileByPosition(bottomLeft.x, bottomLeft.y + height - 1).Type == TileType.Path
                                || path.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + height - 1).Type == TileType.Path)
                                continue;

                            int edgePathCount = 0;

                            for (int i = 0; i < width; i++)
                            {
                                if (path.GetTileByPosition(bottomLeft.x + i, bottomLeft.y).Type == TileType.Path)
                                    edgePathCount++;

                                if (path.GetTileByPosition(bottomLeft.x + i, bottomLeft.y + height - 1).Type == TileType.Path)
                                    edgePathCount++;
                            }

                            for (int j = 1; j < height - 1; j++)
                            {
                                if (path.GetTileByPosition(bottomLeft.x, bottomLeft.y + j).Type == TileType.Path)
                                    edgePathCount++;
                                if (path.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + j).Type == TileType.Path)
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
                // var chosenCandidate = validCandidates[new Random().Next(validCandidates.Count)];

                var candidate = validCandidates[0];

                dungeonRoom = new DungeonRoom(DungeonRoomType.DedlyWall, (candidate.bottomLeft, candidate.topRight));

                int xCount = candidate.topRight.x - candidate.bottomLeft.x;
                int yCount = candidate.topRight.y - candidate.bottomLeft.y;

                Tile[,] tiles = new Tile[xCount, yCount];

                for (int i = 0; i < xCount; i++)
                {
                    for (int j = 0; j < yCount; j++)
                    {
                        int posX = i + candidate.bottomLeft.x;
                        int posY = j + candidate.bottomLeft.y;

                        tiles[i, j] = path.GetTileByPosition(posX, posY).Clone() as Tile;
                        tiles[i, j].SetAsDungeonRoomTile();
                    }
                }

                dungeonRoom.SetTiles(tiles);

                return true;
            }

            return false;
        }
    }
}
