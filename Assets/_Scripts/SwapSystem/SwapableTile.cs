using System.Collections.Generic;
using UnityEngine;

// TODO: posible use double dictionary
namespace SwapSystem
{   
    public class SwappableRuleTile : MonoBehaviour, ISwappable
    {
        public void Swap(SwapVariant variant) => _currentVariant = variant;
        public bool IsCurrentVariantEqualTo(SwapVariant variant) => _currentVariant == variant;

        [SerializeField] private UglySerializableDictionary<SwappableTileData, RuleTile> _variants;

        private Dictionary<SwappableTileData, RuleTile> _variantsDictionary;

        private Dictionary<PathGeneration.TileType, bool> _collidableDictionary = new ()
        {
            { PathGeneration.TileType.Path, false },
            { PathGeneration.TileType.Wall, true },
            { PathGeneration.TileType.DeadlyWall, true },
        };

        private PathGeneration.TileType _currentTileType;
        private SwapVariant _currentVariant;

        public bool IsCollidable => _collidableDictionary[_currentTileType];

        public void Init() 
        {
            _variantsDictionary = _variants.ToDictionary();

            (this as ISwappable).Register(staticPositionY: (int)transform.position.y);
        } 

        public void SetTileType(PathGeneration.TileType type) => _currentTileType = type;

        public RuleTile GetActiveVariant() => _variantsDictionary[new SwappableTileData(_currentTileType, _currentVariant)];
    }
}

namespace SwapSystem
{
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