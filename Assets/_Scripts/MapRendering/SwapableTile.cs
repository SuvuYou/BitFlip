using System.Collections.Generic;
using UnityEngine;

namespace SwapSystem
{
    [CreateAssetMenu(menuName = "CustomTiles/SwappableRuleTile")]
    public class SwappableRuleTile : RuleTile, ISwappable
    {
        public void Swap(SwapVariant variant) => _currentVariant = variant;

        [SerializeField] private UglySerializableDictionary<SwapVariant, RuleTile> _variants;

        private Dictionary<SwapVariant, RuleTile> _variantsDictionary;

        private SwapVariant _currentVariant;

        public void Init() => _variantsDictionary = _variants.ToDictionary();

        public RuleTile GetActiveVariant() => _variantsDictionary[_currentVariant];
    }
}
