using System;
using UnityEngine;

public class EntityMovement
{
    private EntityMovementStats _stats;
    private EntityMovementState _state;

    public EntityMovement(EntityMovementStats stats, EntityMovementState state = null)
    {
        _stats = stats;

        if (state != null) _state = state;
        else _state = new EntityMovementState();
    }

    public void TryMoveInDirection()
    {
        bool isCollidingWithWall = IsFacingWall();

        if (isCollidingWithWall && _state.IsIdle) return;

        if (isCollidingWithWall) 
        {
            OnFacingWall();

            return;
        }

        ApplyVelocity();
        Move();

        return;
    }

    public void SetDirection(Direction direction) => _state.SetCurrentDirection(direction);

    private void ApplyVelocity()
    {
        _state.SetIsIdle(false);

        var currentVelocity = _state.CurrentVelocity + _state.CurrentDirection.ToVectorFloat() * (_stats.Acceleration * Time.deltaTime);

        _state.SetCurrentVelocity(Vector2.ClampMagnitude(currentVelocity, _stats.MaxSpeed));
    }

    private void Move()
    {
        _stats.EntityTransform.position += (Vector3)_state.CurrentVelocity * Time.deltaTime;
    }

    private void OnFacingWall()
    {
        SnapToGrid();

        _state.SetClosestWallPoint(_stats.ColliderTransform.position + _state.CurrentDirection.ToVector().ToVector3WithZ(z: 0f) * _stats.RaycastDistance);

        _state.TriggerOnHitWall(_state.CurrentDirection);
        _state.SetCurrentVelocity(Vector2.zero);
        _state.SetIsIdle(true);
    }

    private void SnapToGrid() 
    { 
        _stats.EntityTransform.position = new Vector3(Mathf.Round(_stats.EntityTransform.position.x), Mathf.Round(_stats.EntityTransform.position.y), _stats.EntityTransform.position.z); 
    }

    private bool IsFacingWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(_stats.ColliderTransform.position, new Vector2(0.25f, 0.25f), 0, _state.CurrentDirection.ToVector(), _stats.RaycastDistance, _stats.WallLayerMask);

        return hit.collider != null;
    }
}

public struct EntityMovementStats
{
    public Transform EntityTransform;
    public Transform ColliderTransform;

    public float MaxSpeed;
    public float Acceleration;
    public float RaycastDistance;

    public LayerMask WallLayerMask;

    public EntityMovementStats(Transform entityTransform, Transform colliderTransform, float maxSpeed, float acceleration, float raycastDistance, LayerMask wallLayerMask)
    {
        EntityTransform = entityTransform;
        ColliderTransform = colliderTransform;
        MaxSpeed = maxSpeed;
        Acceleration = acceleration;
        RaycastDistance = raycastDistance;
        WallLayerMask = wallLayerMask;
    }
}

public class EntityMovementState
{
    public event Action<Direction> OnHitWall;

    public Direction CurrentDirection { get; private set; }
    public Vector2 ClosestWallPoint { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    public bool IsIdle { get; private set; }
    
    public void SetIsIdle(bool isIdle) => IsIdle = isIdle;

    public void SetCurrentDirection(Direction direction) => CurrentDirection = direction;

    public void SetClosestWallPoint(Vector2 closestWallPoint) => ClosestWallPoint = closestWallPoint;

    public void TriggerOnHitWall(Direction direction) => OnHitWall?.Invoke(direction);

    public void SetCurrentVelocity(Vector2 velocity) => CurrentVelocity = velocity;
}