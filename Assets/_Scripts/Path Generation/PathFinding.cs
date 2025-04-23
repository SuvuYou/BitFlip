using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public class AStarTileData
    {
        public Tile Tile;
        public Vector2Int Position;
        public int GCost;
        public int HCost;
        public int FCost;                 

        public AStarTileData(Tile tile, Vector2Int position)
        {
            Tile = tile;
            Position = position;
        }

        public void Initialize(Tile tile, Vector2Int position)
        {
            Tile = tile;
            Position = position;
            GCost = 0;
            HCost = 0;
            FCost = 0;
        }
    }

    public class Pathfinder
    {
        private TilesMatrix _grid;
        private Stack<AStarTileData> _nodePool = new();
        private List<AStarTileData> _openList;
        private HashSet<Vector2Int> _closedSet;
        private Dictionary<Vector2Int, AStarTileData> _allNodes;

        public Pathfinder(TilesMatrix grid)
        {
            _grid = grid;
            int capacity = grid.Width * grid.Height;

            _openList = new List<AStarTileData>(capacity);             
            _closedSet = new HashSet<Vector2Int>(capacity);
            _allNodes = new Dictionary<Vector2Int, AStarTileData>(capacity);
        }

        public bool IsPathFindable(Vector2Int startPos, Vector2Int targetPos)
        {
            _openList.Clear();
            _closedSet.Clear();
            _allNodes.Clear();

            var startNode = GetPooledNode(_grid.GetTileByPosition(startPos), startPos);

            startNode.GCost = 0;
            startNode.HCost = GetDistance(startPos, targetPos);
            startNode.FCost = startNode.GCost + startNode.HCost;

            _openList.Add(startNode);
            _allNodes[startPos] = startNode;

            while (_openList.Count > 0)
            {
                int bestIndex = 0;
                var currentNode = _openList[0];

                for (int i = 1; i < _openList.Count; i++)
                {
                    var node = _openList[i];
                    if (node.FCost < currentNode.FCost || 
                        (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    {
                        currentNode = node;
                        bestIndex = i;
                    }
                }

                _openList.RemoveAt(bestIndex);
                _closedSet.Add(currentNode.Position);

                if (currentNode.Position == targetPos) return true;

                foreach (var dir in DirectionExtentions.AllDirections)
                {
                    var neighbourPos = currentNode.Position + dir.ToVector();

                    if (neighbourPos == targetPos) return true;

                    if (_grid.IsOutOfBounds(neighbourPos) || _grid.IsOnTheBorder(neighbourPos))
                        continue;

                    if (_closedSet.Contains(neighbourPos))
                        continue;

                    var tile = _grid.GetTileByPosition(neighbourPos);
                    if (tile.StateData.Type == TileType.Path)
                        continue;

                    int tentativeG = currentNode.GCost + 1;

                    if (!_allNodes.TryGetValue(neighbourPos, out var neighbourNode))
                    {
                        neighbourNode = GetPooledNode(tile, neighbourPos);    
                        _allNodes[neighbourPos] = neighbourNode;
                    }

                    if (tentativeG < neighbourNode.GCost || !_openList.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = tentativeG;
                        neighbourNode.HCost = GetDistance(neighbourPos, targetPos);
                        neighbourNode.FCost = neighbourNode.GCost + neighbourNode.HCost; 

                        if (!_openList.Contains(neighbourNode))
                            _openList.Add(neighbourNode);
                    }
                }
            }

            return false;
        }

        private AStarTileData GetPooledNode(Tile tile, Vector2Int pos)
        {
            if (_nodePool.Count > 0)
            {
                var node = _nodePool.Pop();
                node.Initialize(tile, pos);
                return node;
            }
            return new AStarTileData(tile, pos);
        }

        private int GetDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}
