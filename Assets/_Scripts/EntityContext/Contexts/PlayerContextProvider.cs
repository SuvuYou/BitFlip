using System.Collections.Generic;

public class PlayerContextData : IContextData
{
    public Queue<Direction> DirectionQueue { get; } = new (2);

    public EntityMovementState MovementState { get; private set; }
    public EntityHealthState HealthState { get; private set; }

    public PlayerContextData() 
    {
        HealthState = new EntityHealthState();
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