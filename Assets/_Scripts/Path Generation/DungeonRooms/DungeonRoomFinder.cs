using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoomFinder
    {
        public bool TryFindDungeonRoom(Path path, Vector2Int forceContainPosition, Vector2Int minSize, Vector2Int maxSize, Vector2Int roomBorderSize, out DungeonRoom dungeonRoom);
    }

    public class VarietyDungeonRoomFinder : IDungeonRoomFinder
    {
        private PseudoRandom.SystemRandomManager _random;

        public VarietyDungeonRoomFinder() 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);
        }

        public bool TryFindDungeonRoom(Path path, Vector2Int forceContainPosition, Vector2Int minSize, Vector2Int maxSize, Vector2Int roomBorderSize, out DungeonRoom dungeonRoom)
        {
            dungeonRoom = null;
            
            if (path.Tiles.GetTileByPosition(forceContainPosition.x, forceContainPosition.y).StateData.IsIncludedInDungeonRoom) return false;

            List<((Vector2Int bottomLeft, Vector2Int topRight), List<Vector2Int> exitPositions)> validCandidates = new();

            for (int width = maxSize.x; width >= minSize.x; width--)
            {
                for (int height = maxSize.y; height >= minSize.y; height--)
                {
                    for (int offsetX = 0; offsetX < width; offsetX++)
                    {
                        for (int offsetY = 0; offsetY < height; offsetY++)
                        {
                            Vector2Int bottomLeft = new (forceContainPosition.x - offsetX, forceContainPosition.y - offsetY);
                            Vector2Int topRight = new (bottomLeft.x + width, bottomLeft.y + height);
                            
                            Vector2Int StemLengthBorderLowerBounds = new (bottomLeft.x + roomBorderSize.x + (path.Tiles.StemLength - 1), bottomLeft.y + roomBorderSize.y + (path.Tiles.StemLength - 1));
                            Vector2Int StemLengthBorderUpperBounds = new (topRight.x + path.Tiles.Width - 1 - roomBorderSize.x - (path.Tiles.StemLength - 1), topRight.y + path.Tiles.Height - 1 - roomBorderSize.y - (path.Tiles.StemLength - 1));

                            bool candidateValid = true;
                            int singlePathTileCount = 0;

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

                                    var tile = path.Tiles.GetTileByPosition(posX, posY);

                                    if (tile.StateData.IsIncludedInDungeonRoom || tile.StateData.IsBorder)
                                    {
                                        candidateValid = false;
                                        break;
                                    }

                                    if (tile.StateData.ConnectionType == TileConnectionType.Corner && !BoundsHelper.IsWithinBounds(posX, posY, StemLengthBorderLowerBounds, StemLengthBorderUpperBounds))
                                    {
                                        candidateValid = false;
                                        break;
                                    }

                                    if (tile.StateData.Type == TileType.Path && tile.StateData.ConnectionType == TileConnectionType.Single)
                                    {
                                        singlePathTileCount++;
                                    }

                                    foreach (var direction in DirectionExtentions.AllDirections)
                                    {
                                        var directionVector = direction.ToVector();

                                        if (path.Tiles.GetTileByPosition(posX + directionVector.x, posY + directionVector.y).StateData.IsIncludedInDungeonRoom)
                                        {
                                            candidateValid = false;
                                            break;
                                        }
                                    }
                                }

                                if (!candidateValid)
                                    break;
                            }

                            if (!candidateValid || singlePathTileCount <= 3)
                                continue;

                            if (path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y + height - 1).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + height - 1).StateData.Type == TileType.Path)
                                continue;

                            List<Vector2Int> exitPositions = new ();

                            for (int i = 0; i < width; i++)
                            {
                                if (path.Tiles.GetTileByPosition(bottomLeft.x + i, bottomLeft.y).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x + i, bottomLeft.y));

                                if (path.Tiles.GetTileByPosition(bottomLeft.x + i, bottomLeft.y + height - 1).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x + i, bottomLeft.y + height - 1));
                            }

                            for (int j = 1; j < height - 1; j++)
                            {
                                if (path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y + j).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x, bottomLeft.y + j));
                                
                                if (path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + j).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x + width - 1, bottomLeft.y + j));
                            }

                            if (exitPositions.Count % 2 == 0)
                            {
                                validCandidates.Add(((bottomLeft, topRight), exitPositions)); 
                            }
                        }
                    }
                }
            }

            if (validCandidates.Count > 0)
            {
                var candidate = validCandidates[_random.GetRandomInt(0, validCandidates.Count)];

                var bounds = candidate.Item1;
                var exitPositions = candidate.Item2;

                dungeonRoom = new DungeonRoom(DungeonRoomType.DedlyWall, (bounds.bottomLeft, bounds.topRight), exitPositions);

                dungeonRoom.SetTiles(path.Tiles.CopyTilesRegion(bounds, roomBorderSize));

                dungeonRoom.FindEnterExitPositionPairs();

                return true;
            }

            return false;
        }
    }

    public class SimpleDungeonRoomFinder : IDungeonRoomFinder
    {
        private PseudoRandom.SystemRandomManager _random;

        public SimpleDungeonRoomFinder() 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);
        }

        public bool TryFindDungeonRoom(Path path, Vector2Int forceContainPosition, Vector2Int minSize, Vector2Int maxSize, Vector2Int roomBorderSize, out DungeonRoom dungeonRoom)
        {
            dungeonRoom = null;
            
            if (path.Tiles.GetTileByPosition(forceContainPosition.x, forceContainPosition.y).StateData.IsIncludedInDungeonRoom) return false;

            List<((Vector2Int bottomLeft, Vector2Int topRight), List<Vector2Int> exitPositions)> validCandidates = new();

            for (int width = maxSize.x; width >= minSize.x; width--)
            {
                for (int height = maxSize.y; height >= minSize.y; height--)
                {
                    for (int offsetX = 0; offsetX < width; offsetX++)
                    {
                        for (int offsetY = 0; offsetY < height; offsetY++)
                        {
                            Vector2Int bottomLeft = new (forceContainPosition.x - offsetX, forceContainPosition.y - offsetY);
                            Vector2Int topRight = new (bottomLeft.x + width, bottomLeft.y + height);
                            
                            Vector2Int StemLengthBorderLowerBounds = new (bottomLeft.x + roomBorderSize.x + (path.Tiles.StemLength - 1), bottomLeft.y + roomBorderSize.y + (path.Tiles.StemLength - 1));
                            Vector2Int StemLengthBorderUpperBounds = new (topRight.x + path.Tiles.Width - 1 - roomBorderSize.x - (path.Tiles.StemLength - 1), topRight.y + path.Tiles.Height - 1 - roomBorderSize.y - (path.Tiles.StemLength - 1));

                            bool candidateValid = true;
                            int singlePathTileCount = 0;

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

                                    var tile = path.Tiles.GetTileByPosition(posX, posY);

                                    if (tile.StateData.IsIncludedInDungeonRoom || tile.StateData.IsBorder)
                                    {
                                        candidateValid = false;
                                        break;
                                    }

                                    if (tile.StateData.ConnectionType == TileConnectionType.Corner && !BoundsHelper.IsWithinBounds(posX, posY, StemLengthBorderLowerBounds, StemLengthBorderUpperBounds))
                                    {
                                        candidateValid = false;
                                        break;
                                    }

                                    if (tile.StateData.Type == TileType.Path && tile.StateData.ConnectionType == TileConnectionType.Single)
                                    {
                                        singlePathTileCount++;
                                    }

                                    foreach (var direction in DirectionExtentions.AllDirections)
                                    {
                                        var directionVector = direction.ToVector();

                                        if (path.Tiles.GetTileByPosition(posX + directionVector.x, posY + directionVector.y).StateData.IsIncludedInDungeonRoom)
                                        {
                                            candidateValid = false;
                                            break;
                                        }
                                    }
                                }

                                if (!candidateValid)
                                    break;
                            }

                            if (!candidateValid || singlePathTileCount <= 3)
                                continue;

                            if (path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y + height - 1).StateData.Type == TileType.Path
                                || path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + height - 1).StateData.Type == TileType.Path)
                                continue;

                            List<Vector2Int> exitPositions = new ();

                            for (int i = 0; i < width; i++)
                            {
                                if (path.Tiles.GetTileByPosition(bottomLeft.x + i, bottomLeft.y).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x + i, bottomLeft.y));

                                if (path.Tiles.GetTileByPosition(bottomLeft.x + i, bottomLeft.y + height - 1).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x + i, bottomLeft.y + height - 1));
                            }

                            for (int j = 1; j < height - 1; j++)
                            {
                                if (path.Tiles.GetTileByPosition(bottomLeft.x, bottomLeft.y + j).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x, bottomLeft.y + j));
                                
                                if (path.Tiles.GetTileByPosition(bottomLeft.x + width - 1, bottomLeft.y + j).StateData.Type == TileType.Path)
                                    exitPositions.Add(new Vector2Int(bottomLeft.x + width - 1, bottomLeft.y + j));
                            }

                            if (exitPositions.Count == 2)
                            {
                                validCandidates.Add(((bottomLeft, topRight), exitPositions)); 
                            }

                            if (exitPositions.Count > 2)
                            {
                                bool isValid = true;

                                foreach (var exitPosition in exitPositions)
                                {
                                    Direction adjasentTileDirection = Direction.None;

                                    if (exitPosition.x == bottomLeft.x) adjasentTileDirection = Direction.Right;
                                    else if (exitPosition.x == bottomLeft.x + width - 1) adjasentTileDirection = Direction.Left;
                                    else if (exitPosition.y == bottomLeft.y) adjasentTileDirection = Direction.Up;
                                    else if (exitPosition.y == bottomLeft.y + height - 1) adjasentTileDirection = Direction.Down;

                                    var previousTilePosition = exitPosition + adjasentTileDirection.ToVector();

                                    var connectionType = path.Tiles.GetTileByPosition(previousTilePosition).StateData.ConnectionType;

                                    if (connectionType != TileConnectionType.Intersection && connectionType != TileConnectionType.T_Junction)
                                    {
                                        isValid = false;
                                    }
                                }

                                if (isValid) validCandidates.Add(((bottomLeft, topRight), exitPositions));
                            }
                        }
                    }
                }
            }

            if (validCandidates.Count > 0)
            {
                var candidate = validCandidates[_random.GetRandomInt(0, validCandidates.Count)];

                var bounds = candidate.Item1;
                var exitPositions = candidate.Item2;

                dungeonRoom = new DungeonRoom(DungeonRoomType.DedlyWall, (bounds.bottomLeft, bounds.topRight), exitPositions);

                dungeonRoom.SetTiles(path.Tiles.CopyTilesRegion(bounds, roomBorderSize));

                return true;
            }

            return false;
        }
    }
}
