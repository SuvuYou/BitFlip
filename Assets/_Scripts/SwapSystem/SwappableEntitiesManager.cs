using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwapSystem
{
    public class SwappableEntitiesManager : Singleton<SwappableEntitiesManager>
    {
        private List<ISwappable> _dynamicSwappableEntities = new ();
        private Dictionary<int, List<ISwappable>> _staticSwappableEntities = new ();

        private int _startStaticPositionY = 0, _endStaticPositionY = 0;

        public SwapVariant CurrentSupposedVariant = SwapVariant.Light;

        private SwapConditionByGameobjectPositionY _swapConditionByGameobjectPositionY = new();

        public event Action OnSwapComplete;
        public event Action<int> OnSwapAtYLevelComplete;

        public void Register(ISwappable swappableEntity) => _dynamicSwappableEntities.Add(swappableEntity);
        public void Unregister(ISwappable swappableEntity) => _dynamicSwappableEntities.Remove(swappableEntity);

        public void Register(ISwappable swappableEntity, int staticPositionY) => _staticSwappableEntities[staticPositionY].Add(swappableEntity);
        public void Unregister(ISwappable swappableEntity, int staticPositionY) => _staticSwappableEntities[staticPositionY].Remove(swappableEntity);

        private Coroutine _swapCoroutine;

        public void InitContainers(GameDataSO gameData)
        {
            for (int i = 0; i < gameData.MapHeight; i++) _staticSwappableEntities.Add(i, new List<ISwappable>());

            _startStaticPositionY = 0;
            _endStaticPositionY = gameData.MapHeight - 1;
        }

        public void SwapEntities(float layerSwapInterval = 0.01f)
        {
            if (_swapCoroutine != null)
                StopCoroutine(_swapCoroutine);

            _swapCoroutine = StartCoroutine(SwapVariantInRange(layerSwapInterval));
        }

        private void SwapVariantAt(int staticPositionY)
        {
            _swapConditionByGameobjectPositionY.ThresholdPositionY = staticPositionY;

            foreach (ISwappable swappableEntity in _dynamicSwappableEntities)
            {
                if (swappableEntity.IsCurrentVariantEqualTo(CurrentSupposedVariant)) continue;
                if (swappableEntity.ConditionByGameobject(_swapConditionByGameobjectPositionY.GameObjectYPositionCondition)) continue;

                swappableEntity.Swap(CurrentSupposedVariant);
            }

            foreach (ISwappable swappableEntity in _staticSwappableEntities[staticPositionY])
            {
                if (swappableEntity.IsCurrentVariantEqualTo(CurrentSupposedVariant)) continue;

                swappableEntity.Swap(CurrentSupposedVariant);
            }

            OnSwapAtYLevelComplete?.Invoke(staticPositionY);
        }

        private IEnumerator SwapVariantInRange(float layerSwapInterval)
        {
            CurrentSupposedVariant = CurrentSupposedVariant.Opposite();

            for (int i = _startStaticPositionY; i <= _endStaticPositionY; i++)
            {
                SwapVariantAt(i);
                yield return new WaitForSeconds(layerSwapInterval);
            }

            OnSwapComplete?.Invoke();
        }
    }

    public class SwapConditionByGameobjectPositionY
    {
        public int ThresholdPositionY { get; set; }

        public bool GameObjectYPositionCondition(GameObject gameObject)
        {
            return gameObject.transform.position.y > ThresholdPositionY;
        }
    }
}