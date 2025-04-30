

using System;

public class PlayerContextData : IContextData
{
    private Direction _currentDirection;
    public Direction CurrentDirection => _currentDirection;

    public void SetDirection(Direction direction) 
    {
        _currentDirection = direction;
    
        OnDirectionChanged?.Invoke(direction);
    }

    public Action<Direction> OnDirectionChanged;
}

public class PlayerContextProvider : ContextProvider<PlayerContextData>
{
    protected override void Awake() 
    {
        _contextData = new PlayerContextData();

        base.Awake();
    }
}