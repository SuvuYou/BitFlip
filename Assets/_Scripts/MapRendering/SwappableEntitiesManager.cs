using System.Collections.Generic;

namespace SwapSystem
{
    public class SwappableEntitiesManager : Singlton<SwappableEntitiesManager>
    {
        private SwappableGroup _dynamicSwappableEntities = new ();

        private Dictionary<int, SwappableGroup> _staticSwappableEntities = new ();

        public void Register(ISwappable swappableEntity) => _dynamicSwappableEntities.Add(swappableEntity);
        public void Unregister(ISwappable swappableEntity) => _dynamicSwappableEntities.Remove(swappableEntity);

        public void Register(ISwappable swappableEntity, int staticPositionY) => _staticSwappableEntities[staticPositionY].Add(swappableEntity);
        public void Unregister(ISwappable swappableEntity, int staticPositionY) => _staticSwappableEntities[staticPositionY].Remove(swappableEntity);

        public void SwapVariant() => _dynamicSwappableEntities.SwapVariant();

        private struct SwappableGroup
        {
            public List<ISwappable> swappableEntities;
            public SwapVariant CurrentVariant;

            public readonly void Add(ISwappable swappableEntity) => swappableEntities.Add(swappableEntity);
            public readonly void Remove(ISwappable swappableEntity) => swappableEntities.Remove(swappableEntity);

            public void SwapVariant()
            {
                CurrentVariant = CurrentVariant.Opposite();

                foreach (ISwappable swappableEntity in swappableEntities)
                {
                    swappableEntity.Swap(CurrentVariant);
                }
            }
        }
    }
}