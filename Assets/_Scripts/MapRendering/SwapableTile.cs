using System.Collections.Generic;
using UnityEngine;

namespace SwapSystem
{
    public class SwappableRuleTile : MonoBehaviour, ISwappable
    {
        public void Swap(SwapVariant variant) => _currentVariant = variant;
        public bool IsCurrentVariantEqualTo(SwapVariant variant) => _currentVariant == variant;

        [SerializeField] private UglySerializableDictionary<SwapVariant, RuleTile> _variants;

        private Dictionary<SwapVariant, RuleTile> _variantsDictionary;
        private SwapVariant _currentVariant;

        public void Init() 
        {
            _variantsDictionary = _variants.ToDictionary();

            (this as ISwappable).Register(staticPositionY: (int)transform.position.y);
        } 

        public RuleTile GetActiveVariant() => _variantsDictionary[_currentVariant];
    }
}
