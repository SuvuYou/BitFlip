

using System;
using System.Collections.Generic;

public class EnemyContextData : IContextData
{
    public event Action<SwapSystem.SwapVariant, SwappableEnemyStats> OnSwap;

    public Dictionary<SwapSystem.SwapVariant, SwappableEnemyStats> VariantsLookup { get; private set; }
    public SwapSystem.SwapVariant CurrentVariant { get; private set; }

    public SwappableEnemyStats CurrentVariantStats => VariantsLookup[CurrentVariant];

    public void SetVariantsLookup(Dictionary<SwapSystem.SwapVariant, SwappableEnemyStats> variantsLookup) => VariantsLookup = variantsLookup;

    public void SetCurrentVariant(SwapSystem.SwapVariant currentVariant) 
    {
        CurrentVariant = currentVariant;

        OnSwap?.Invoke(currentVariant, CurrentVariantStats);
    }

    public EntityMovementState MovementState { get; private set; }

    public event Action<Direction> OnWindupMovementStart;
    public float WindupTime { get; private set; }

    public void TriggerWindupMovementEvent(Direction direction) => OnWindupMovementStart?.Invoke(direction);

    public void SetWindupTime(float windupTime) => WindupTime = windupTime;

    public EnemyContextData() 
    {
        MovementState = new EntityMovementState();
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