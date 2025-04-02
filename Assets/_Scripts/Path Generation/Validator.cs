using System.Collections.Generic;

namespace PathGeneration
{
    public class Validator
    {
        private bool _hasInvalidatedPathRootChild;

        private Stack<Tile> _pathRoots = new();
        private Stack<Tile> _invalidPathRootsStack = new();

        public void AddPathRoot(Tile rootTile) => _pathRoots.Push(rootTile);

        public void PopPathRoot() => _pathRoots.Pop();

        public void InvalidateLastPathRoot()
        {
            if (_hasInvalidatedPathRootChild)
            {
                Tile tile = _invalidPathRootsStack.Pop();
                tile.Revalidate();
            }

            Tile invalidTile = _pathRoots.Peek();

            invalidTile.Invalidate();

            _invalidPathRootsStack.Push(invalidTile);

            _hasInvalidatedPathRootChild = true;
        }

        public void ReleaseInvalidationStreak() => _hasInvalidatedPathRootChild = false;
    }
}
