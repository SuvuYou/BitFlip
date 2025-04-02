using UnityEngine;

namespace PathGeneration
{
    public class Map
    {
        public readonly Path MapPath;
        public readonly PseudoRandom.SystemRandomManager _systemRandom;

        public Map(int width, int height, int stemLength = 2)
        {
            MapPath = new Path(width, height, new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1), stemLength);

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
                    var newPath = new Path(MapPath.Width, MapPath.Height, pos, pos, MapPath.StemLength, MapPath.GetOccupiedPositions());
                    newPath.RandomWalk();
                    MapPath.Merge(newPath);
                }
            }
        }
    }
}
