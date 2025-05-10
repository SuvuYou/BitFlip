public class PlayerContextData : IContextData
{
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