using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoom
    {
        public TilesMatrix Tiles { get; }
    }
}
