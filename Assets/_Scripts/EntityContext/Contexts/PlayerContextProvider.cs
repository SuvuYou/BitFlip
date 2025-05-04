

using System;
using UnityEngine;

public class PlayerContextData : IContextData
{
    public Action<Direction> OnDirectionChanged;
    public Action<Direction> OnHitWall;

    private Direction _currentDirection;
    public Direction CurrentDirection => _currentDirection;

    private RaycastHit2D _wallRaycastHit;
    public RaycastHit2D WallRaycastHit => _wallRaycastHit;

    private bool _isFacingRight;
    public bool IsFacingRight => _isFacingRight;

    public void SetDirection(Direction direction) 
    {
        _currentDirection = direction;

        _isFacingRight = direction == Direction.Right;
    
        OnDirectionChanged?.Invoke(direction);
    }

    public void HitWall(Direction fromDirection)
    {
        _isFacingRight = fromDirection == Direction.Left;

        OnHitWall?.Invoke(fromDirection);
    }

    public void SetWallRaycastHit(RaycastHit2D raycastHit) => _wallRaycastHit = raycastHit;
}

public class PlayerContextProvider : ContextProvider<PlayerContextData>
{
    protected override void Awake() 
    {
        _contextData = new PlayerContextData();

        base.Awake();
    }
}