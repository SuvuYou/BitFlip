using System.Collections.Generic;
using UnityEngine;

namespace SwapSystem
{
    public class SwapableBackground : MonoBehaviour, ISwappable
    {
        public void Swap(SwapVariant variant) 
        {
            _previousVariant = _currentVariant;
            _currentVariant = variant;

            _backgroundSpriteRenderer.color = _variantsDictionary[_previousVariant];
            _foregroundSpriteRenderer.color = _variantsDictionary[_currentVariant];
        }

        public bool IsCurrentVariantEqualTo(SwapVariant variant) => _currentVariant == variant;

        [SerializeField] private UglySerializableDictionary<SwapVariant, Color> _variants;

        [SerializeField] private SpriteRenderer _backgroundSpriteRenderer;

        [SerializeField] private Transform _foregroundTransform;
        [SerializeField] private SpriteRenderer _foregroundSpriteRenderer;

        private Dictionary<SwapVariant, Color> _variantsDictionary;
        private SwapVariant _previousVariant;
        private SwapVariant _currentVariant;

        private void Awake() 
        {
            _variantsDictionary = _variants.ToDictionary();

            (this as ISwappable).RegisterAsInitial();

            SwappableEntitiesManager.Instance.OnSwapAtYLevelComplete += MoveSwapableBackgroundToYLevel;
            SwappableEntitiesManager.Instance.OnSwapComplete += CompleteSwap;
        } 

        private void MoveSwapableBackgroundToYLevel(int yLevel) 
        {
            _foregroundTransform.position = new Vector3(_foregroundTransform.position.x, yLevel, _foregroundTransform.position.z);
        }
        
        private void CompleteSwap() 
        {
            _previousVariant = _currentVariant;
            _backgroundSpriteRenderer.color = _variantsDictionary[_previousVariant];
            _foregroundSpriteRenderer.color = _variantsDictionary[_currentVariant];
        }
    }
}
