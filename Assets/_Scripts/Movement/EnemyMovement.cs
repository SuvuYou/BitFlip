using UnityEngine;

public class EnemyMovement : MonoBehaviour, IConsumer<EnemyContextData>
{
    public EnemyContextData Context { get; private set; }

    public void Inject(EnemyContextData context) => Context = context;

    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private LayerMask _searchTargetMask;

    [SerializeField] private Transform _entityTransform;
    [SerializeField] private Transform _colliderTransform;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _dashMultiplier = 2f;
    [SerializeField] private float _wallRaycastDistance = 0.5f;
    [SerializeField] private float _targetRaycastDistance = 20f;

    private EntityMovement _movement;

    private void Start()
    {
        _movement = new EntityMovement
        (
            new EntityMovementStats(_entityTransform, _colliderTransform, _maxSpeed, _acceleration, _dashMultiplier, _wallRaycastDistance, _wallMask),
            Context.MovementState
        );
    }
    
    public void MoveInDirection(Direction direction) 
    {
        _movement.SetDirection(direction);
        _movement.TryMoveInDirection();
    }

    public void DashInDirection(Direction direction) 
    {
        _movement.SetDirection(direction);
        _movement.TryDashInDirection();
    }

    public bool IsFacingWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _wallRaycastDistance, _wallMask);

        return hit.collider != null;
    }

    public bool IsFacingTarget(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            _colliderTransform.position,
            new Vector2(0.25f, 0.25f),
            0,
            direction,
            _targetRaycastDistance,
            _wallMask | _searchTargetMask
        );

        foreach (var hit in hits)
        {
            // If we hit a wall before hitting the player
            if ((_wallMask.value & (1 << hit.collider.gameObject.layer)) != 0)
                return false;

            // If we hit a player and no wall was in the way
            if ((_searchTargetMask.value & (1 << hit.collider.gameObject.layer)) != 0)
                return true;
        }

        return false;
    }
} 