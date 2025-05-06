

using System;
using System.Collections.Generic;

public class PlayerContextData : IContextData
{
    public Action<Direction> OnDirectionChanged;

    public Queue<Direction> DirectionQueue { get; } = new (2);

    public EntityMovementState MovementState { get; private set; }

    private bool _isFacingRight;
    public bool IsFacingRight => _isFacingRight;

    public PlayerContextData() 
    {
        MovementState = new EntityMovementState();

        MovementState.OnHitWall += SwitchFacingDirectionOnHitWall;
    }

    public void SetDirection(Direction direction) 
    {
        _isFacingRight = direction == Direction.Right;
    
        OnDirectionChanged?.Invoke(direction);
    }

    public void SwitchFacingDirectionOnHitWall(Direction fromDirection)
    {
        _isFacingRight = fromDirection == Direction.Left;
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