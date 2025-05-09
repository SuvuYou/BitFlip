using UnityEngine;

namespace PathGeneration
{
    public interface IPathModification
    {
        void Modify();
        void Undo();
    }

    public class Explore : IPathModification
    {
        private readonly Path _path;
        
        private readonly TileType[] _cachedTypes;
        private readonly Direction[] _cachedDirections;
        private readonly int[] _cachedRouteIndices;
        private readonly (Vector2Int currentPos, Direction currentFacingDirection) _cachedState;

        private readonly Direction _exploreDirection;

        private readonly int _routeIndex;

        public Explore(Path path, (Vector2Int currentPos, Direction currentFacingDirection) cachedState, Direction exploreDirection, int routeIndex)
        {
            this._path = path;
            this._cachedState = cachedState;
            this._exploreDirection = exploreDirection;
            this._routeIndex = routeIndex;

            _cachedRouteIndices = new int[path.StemLength];
            _cachedTypes = new TileType[path.StemLength];
            _cachedDirections = new Direction[path.StemLength];
        }

        public void Modify()
        {
            Vector2Int currentPos = _cachedState.currentPos;

            for (int i = 0; i < _path.StemLength; i++)
            {
                Vector2Int nextPos = currentPos + _exploreDirection.ToVector();

                _cachedRouteIndices[i] = _path.Tiles.GetTileByPosition(nextPos).StateData.RouteIndices;
                _cachedTypes[i] = _path.Tiles.GetTileByPosition(nextPos).StateData.Type;
                _cachedDirections[i] = _path.Tiles.GetTileByPosition(nextPos).StateData.PreviousFacingDirection;

                _path.Tiles.SetTile(nextPos.x, nextPos.y, TileType.Path, _exploreDirection, _routeIndex);

                currentPos = nextPos;

                if (i == 0)
                    _path.PathValidator.AddPathRoot(_path.Tiles.GetTileByPosition(nextPos));
            }

            _path.SetCurrentState((currentPos, _exploreDirection));
        }

        public void Undo()
        {
            Vector2Int currentPos = _cachedState.currentPos;

            for (int i = 0; i < _path.StemLength; i++)
            {
                var routeIndex = _cachedRouteIndices[i];
                var tileType = _cachedTypes[i];
                var direction = _cachedDirections[i];

                Vector2Int nextPos = currentPos + _exploreDirection.ToVector();
                _path.Tiles.SetTile(nextPos.x, nextPos.y, tileType, direction, routeIndex);

                currentPos = nextPos;

                if (i == 0)
                    _path.PathValidator.PopPathRoot();
            }

            _path.SetCurrentState(_cachedState);
        }
    }
}
