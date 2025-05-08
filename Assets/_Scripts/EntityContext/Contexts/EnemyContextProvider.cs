

using System;
using System.Collections.Generic;

public class EnemyContextData : IContextData
{
    public Action<SwapSystem.SwapVariant, SwappableEnemyStats> OnSwap;

    public Dictionary<SwapSystem.SwapVariant, SwappableEnemyStats> VariantsLookup { get; private set; }
    public SwapSystem.SwapVariant CurrentVariant { get; private set; }

    public EntityMovementState MovementState { get; private set; }
    public EntityHealthState HealthState { get; private set; }

    public EnemyContextData() 
    {
        MovementState = new EntityMovementState();
        HealthState = new EntityHealthState();
    }

    public SwappableEnemyStats CurrentVariantStats => VariantsLookup[CurrentVariant];

    public void SetVariantsLookup(Dictionary<SwapSystem.SwapVariant, SwappableEnemyStats> variantsLookup) => VariantsLookup = variantsLookup;

    public void SetCurrentVariant(SwapSystem.SwapVariant currentVariant) 
    {
        CurrentVariant = currentVariant;

        OnSwap?.Invoke(currentVariant, CurrentVariantStats);
    }
}

public class EnemyContextProvider : ContextProvider<EnemyContextData>
{
    protected override void Awake() 
    {
        _contextData = new EnemyContextData();

        base.Awake();
    }
}