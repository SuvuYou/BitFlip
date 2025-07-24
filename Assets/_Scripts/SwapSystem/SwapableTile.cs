using System.Collections.Generic;
using UnityEngine;

namespace SwapSystem
{   
    [CreateAssetMenu(fileName = "SwappableTileData", menuName = "ScriptableObjects/SwapSystem/SwappableTileData")]
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

    public class SwappableRuleTile : ISwappable
    {
        public void Swap(SwapVariant variant) => _currentVariant = variant;
        public bool IsCurrentVariantEqualTo(SwapVariant variant) => _currentVariant == variant;

        private SwappableTileDataSO _swappableTileDataSO;

        private PathGeneration.TileType _currentTileType;
        private SwapVariant _currentVariant;

        public bool IsCollidable => _swappableTileDataSO.CollidablesDictionary[_currentTileType];

        public readonly int X, Y;

        public SwappableRuleTile(int x, int y, SwappableTileDataSO swappableTileDataSO) 
        {
            X = x; Y = y;

            _swappableTileDataSO = swappableTileDataSO;

            (this as ISwappable).Register(staticPositionY: Y);
        } 

        public void SetTileType(PathGeneration.TileType type) => _currentTileType = type;

        public RuleTile GetActiveVariant() => _swappableTileDataSO.VariantsDictionary[new SwappableTileData(_currentTileType, _currentVariant)];
    }
}

namespace SwapSystem
{
    // TODO: posible use double dictionary
    [System.Serializable]
    public struct SwappableTileData
    {
        public PathGeneration.TileType Type;
        public SwapVariant Variant;

        public SwappableTileData(PathGeneration.TileType type, SwapVariant variant) => (Type, Variant) = (type, variant);

        public readonly override bool Equals(object obj)
        {
            if (obj is SwappableTileData other)
            {
                return Type == other.Type && Variant == other.Variant;
            }
            return false;
        }

        public readonly override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + Variant.GetHashCode();
                return hash;
            }
        }
    }
}