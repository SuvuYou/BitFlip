

using System;

public class PlayerContextData : IContextData
{
    public Action<Direction> OnDirectionChanged;

    private Direction _currentDirection;
    public Direction CurrentDirection => _currentDirection;

    private bool _isFacingRight;
    public bool IsFacingRight => _isFacingRight;

    public void SetDirection(Direction direction) 
    {
        _currentDirection = direction;

        _isFacingRight = direction == Direction.Right;
    
        OnDirectionChanged?.Invoke(direction);
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