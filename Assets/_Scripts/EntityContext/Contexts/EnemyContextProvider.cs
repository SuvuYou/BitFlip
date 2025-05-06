

using System;
using System.Collections.Generic;

public class EnemyContextData : IContextData
{
    public Action<SwapSystem.SwapVariant> OnSwap;

    public Dictionary<SwapSystem.SwapVariant, SwappableEnemyStats> VariantsLookup { get; private set; }
    public SwapSystem.SwapVariant CurrentVariant { get; private set; }

    public EntityMovementState MovementState { get; private set; }

    public EnemyContextData() 
    {
        MovementState = new EntityMovementState();
    }

    public void SetVariantsLookup(Dictionary<SwapSystem.SwapVariant, SwappableEnemyStats> variantsLookup) => VariantsLookup = variantsLookup;

    public void SetCurrentVariant(SwapSystem.SwapVariant currentVariant) 
    {
        CurrentVariant = currentVariant;

        OnSwap?.Invoke(currentVariant);
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