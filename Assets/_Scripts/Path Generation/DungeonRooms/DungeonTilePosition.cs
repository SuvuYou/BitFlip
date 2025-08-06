using UnityEngine;

namespace PathGeneration
{
    public class DungeonTilePosition
    {
        public Vector2Int Local { get; private set; }
        public Vector2Int Global { get; private set; }

        public DungeonTilePosition(Vector2Int local, Vector2Int global)
        {
            Local = local;
            Global = global;
        }
    }
}
