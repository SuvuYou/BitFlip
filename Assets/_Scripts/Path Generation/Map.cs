using UnityEngine;

namespace PathGeneration
{
    public class Map
    {
        public readonly Path MapPath;
        public readonly PseudoRandom.SystemRandomManager _systemRandom;

        public Map(int width, int height, Vector2Int borderSize = default, int stemLength = 2)
        {
            Vector2Int startPos = new (0 + borderSize.x, 0 + borderSize.y);
            Vector2Int endPos = new (width - 1 - borderSize.x, height - 1 - borderSize.y);

            MapPath = new Path(width, height, startPos, endPos, borderSize, stemLength);

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);
        }

        public void Generate()
        {
            MapPath.RandomWalk();
            // ExpandCorners();
        }

        private void ExpandCorners()
        {
            foreach (var (pos, tile) in MapPath.GetCornerTiles())
            {
                if (_systemRandom.GetRandomFloat() > 0.5f)
                {
                    var newPath = new Path(MapPath.Width, MapPath.Height, pos, pos, MapPath.BorderSize, MapPath.StemLength, MapPath.GetOccupiedPositions());
                    newPath.RandomWalk();
                    MapPath.Merge(newPath);
                }
            }
        }
    }
}
