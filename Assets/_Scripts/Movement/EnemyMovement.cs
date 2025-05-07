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
    [SerializeField] private float _raycastDistance = 1f;

    private EntityMovement _movement;

    private void Start()
    {
        _movement = new EntityMovement
        (
            new EntityMovementStats(_entityTransform, _colliderTransform, _maxSpeed, _acceleration, _raycastDistance, _wallMask),
            Context.MovementState
        );
    }
    
    public void MoveInDirection(Direction direction) 
    {
        _movement.SetDirection(direction);
        _movement.TryMoveInDirection();
    }

    public bool IsFacingWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _wallMask);

        return hit.collider != null;
    }

    public bool IsFacingTarget(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _searchTargetMask);

        return hit.collider != null;
    }
} 