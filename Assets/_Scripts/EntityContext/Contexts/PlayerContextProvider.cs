

using System;

public class PlayerContextData : IContextData
{
    public Action<Direction> OnDirectionChanged;
    public Action<bool> OnIdleChanged;

    private Direction _currentDirection;
    public Direction CurrentDirection => _currentDirection;

    private bool _isIdle = true;
    public bool IsIdle => _isIdle;

    private bool _isFacingRight;
    public bool IsFacingRight => _isFacingRight;

    public void SetDirection(Direction direction) 
    {
        _currentDirection = direction;

        _isFacingRight = direction == Direction.Right;
    
        OnDirectionChanged?.Invoke(direction);
    }

    public void SetIdle(bool newIdle)
    {
        _isIdle = newIdle;

        if(_isIdle)
        {
            _isFacingRight = CurrentDirection == Direction.Left;
        }

        OnIdleChanged?.Invoke(newIdle);
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