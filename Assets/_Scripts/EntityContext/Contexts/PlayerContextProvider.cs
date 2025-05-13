using System;

public class PlayerContextData : IContextData
{
    public Action OnContextInjected { get; set; }

    public Action OnEnterAttackMode { get; set; }
    public Action OnExitAttackMode { get; set; }

    public EntityMovementState MovementState { get; private set; }

    public PlayerContextData() 
    {
        MovementState = new EntityMovementState();     
    }
}

public class PlayerContextProvider : ContextProvider<PlayerContextData>
{
    protected override void Awake() 
    {
        _contextData = new PlayerContextData();

        base.Awake();
    }
}