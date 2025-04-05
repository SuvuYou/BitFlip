using System;
using UnityEngine;

namespace SwapSystem
{
    public enum SwapVariant { Light, Dark };

    public static class SwapExtensions
    {
        public static SwapVariant Opposite(this SwapVariant varient) => 
            varient switch
            {
                SwapVariant.Light => SwapVariant.Dark,
                SwapVariant.Dark => SwapVariant.Light,
                _ => SwapVariant.Light
            };
    }

    public interface ISwappable
    {
        public abstract void Swap(SwapVariant variant);

        public abstract bool IsCurrentVariantEqualTo(SwapVariant variant);

        public virtual void Register() => SwappableEntitiesManager.Instance.Register(this);
        public virtual void Unregister() => SwappableEntitiesManager.Instance.Unregister(this);

        public virtual void Register(int staticPositionY) => SwappableEntitiesManager.Instance.Register(this, staticPositionY);
        public virtual void Unregister(int staticPositionY) => SwappableEntitiesManager.Instance.Unregister(this, staticPositionY);

        // Conditioning
        // Used to determine if the swappable entity should be swapped
        public virtual bool ConditionByGameobject(Func<GameObject, bool> condition) => false;
    }
}
