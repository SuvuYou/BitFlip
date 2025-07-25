using System.Collections.Generic;
using UnityEngine;

namespace SwapSystem
{   
    [CreateAssetMenu(fileName = "SwappableTileData", menuName = "ScriptableObjects/SwappableTileData")]
    public class SwappableTileDataSO : ScriptableObject
    {
        [SerializeField] public UglySerializableDictionary<SwappableTileData, RuleTile> Variants; 
        [SerializeField] public UglySerializableDictionary<PathGeneration.TileType, bool> Collidables;

        public Dictionary<SwappableTileData, RuleTile> VariantsDictionary { get; private set; }
        public Dictionary<PathGeneration.TileType, bool> CollidablesDictionary { get; private set; }

        public void SetupDictionaries() 
        {
            VariantsDictionary = Variants.ToDictionary();
            CollidablesDictionary = Collidables.ToDictionary();
        }
    }
}
