using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathGeneration
{
    public class AStarTileData
    {
        public Tile Tile;
        public Vector2Int Position;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;

        public AStarTileData(Tile tile, Vector2Int position)
        {
            Tile = tile;
            Position = position;
        }
    }

    public class Pathfinder
    {
        private TilesMatrix _grid;

        public Pathfinder(TilesMatrix grid)
        {
            _grid = grid;
        }

        public bool IsPathFindable(Vector2Int startPos, Vector2Int targetPos)
        {
            var startTile = _grid.GetTileByPosition(startPos);
            var targetTile = _grid.GetTileByPosition(targetPos);

            var openList = new List<AStarTileData>();
            var closedSet = new HashSet<Vector2Int>();

            var startNode = new AStarTileData(startTile, startPos) { GCost = 0, HCost = GetDistance(startPos, targetPos) };

            openList.Add(startNode);

            var allNodes = new Dictionary<Vector2Int, AStarTileData>
            {
                { startPos, startNode }
            };

            while (openList.Count > 0)
            {
                var currentNode = openList.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();
                openList.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                if (currentNode.Position == targetPos)
                    return true;

                foreach (var direction in DirectionExtentions.AllDirections)
                {
                    var neighborTilePosition = currentNode.Position + direction.ToVector();

                    if (_grid.IsOutOfBounds(neighborTilePosition))
                        continue;

                    var neighborTile = _grid.GetTileByPosition(neighborTilePosition);

                    if (neighborTile.StateData.Type == TileType.Wall || closedSet.Contains(neighborTilePosition))
                        continue;

                    var tentativeGCost = currentNode.GCost + GetDistance(currentNode.Position, neighborTilePosition);

                    if (!allNodes.TryGetValue(neighborTilePosition, out var neighborNode))
                    {
                        neighborNode = new AStarTileData(neighborTile, neighborTilePosition);
                        allNodes[neighborTilePosition] = neighborNode;
                    }

                    if (tentativeGCost < neighborNode.GCost || !openList.Contains(neighborNode))
                    {
                        neighborNode.GCost = tentativeGCost;
                        neighborNode.HCost = GetDistance(neighborTilePosition, targetPos);

                        if (!openList.Contains(neighborNode))
                            openList.Add(neighborNode);
                    }
                }
            }

            return false;
        }

        private int GetDistance(Vector2Int a, Vector2Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return dx + dy;
        }
    }
}
