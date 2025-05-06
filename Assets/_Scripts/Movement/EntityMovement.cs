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
        _stats.PlayerTransform.position += (Vector3)_state.CurrentVelocity * Time.deltaTime;
    }

    private void OnFacingWall()
    {
        _state.TriggerOnHitWall(_state.CurrentDirection);
        _state.SetCurrentVelocity(Vector2.zero);
        _state.SetIsIdle(true);

        SnapToGrid();
    }

    private void SnapToGrid() 
    { 
        _stats.PlayerTransform.position = new Vector3(Mathf.Round(_stats.PlayerTransform.position.x), Mathf.Round(_stats.PlayerTransform.position.y), _stats.PlayerTransform.position.z); 
    }

    private bool IsFacingWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(_stats.ColliderTransform.position, new Vector2(0.25f, 0.25f), 0, _state.CurrentDirection.ToVector(), _stats.RaycastDistance, _stats.WallLayerMask);

        _state.SetWallRaycastHit(hit);

        return hit.collider != null;
    }
}

public struct EntityMovementStats
{
    public Transform PlayerTransform;
    public Transform ColliderTransform;

    public float MaxSpeed;
    public float Acceleration;
    public float RaycastDistance;

    public LayerMask WallLayerMask;

    public EntityMovementStats(Transform playerTransform, Transform colliderTransform, float maxSpeed, float acceleration, float raycastDistance, LayerMask wallLayerMask)
    {
        PlayerTransform = playerTransform;
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

    public RaycastHit2D WallRaycastHit { get; private set; }
    public Direction CurrentDirection { get; private set; }
    public Vector2 CurrentVelocity { get; private set; }
    public bool IsIdle { get; private set; }

    public void SetIsIdle(bool isIdle) => IsIdle = isIdle;

    public void SetCurrentDirection(Direction direction) => CurrentDirection = direction;

    public void SetWallRaycastHit(RaycastHit2D raycastHit) => WallRaycastHit = raycastHit;

    public void TriggerOnHitWall(Direction direction) => OnHitWall?.Invoke(direction);

    public void SetCurrentVelocity(Vector2 velocity) => CurrentVelocity = velocity;
}