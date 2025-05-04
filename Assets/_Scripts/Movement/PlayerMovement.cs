using UnityEngine;

public class PlayerMovement : MonoBehaviour, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) => Context = context;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _colliderTransform;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _raycastDistance = 1f;
    [SerializeField] private float _cayoteMovementTime = 0.1f;

    [SerializeField] private LayerMask _layerMask;

    private Timer _cayoteTimer;

    private bool _isIdle = true;
    private Vector2 _currentVelocity = Vector2.zero;
    private Direction _nextDirection = Direction.Up;

    private void Start()
    {
        _cayoteTimer = new Timer(_cayoteMovementTime);
    }

    private void Update()
    {
        GetInput();
        UpdateDirection();
        CollisionCheck();

        ApplyVelocity();
        Move();
    }

    private void ApplyVelocity()
    {
        _currentVelocity += GetMovementDirection() * (_acceleration * Time.deltaTime);

        _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, _maxSpeed);
    }

    private void Move()
    {
        _playerTransform.position += (Vector3)_currentVelocity * Time.deltaTime;
    }

    private Vector2 GetMovementDirection()
    {
        if (_isIdle) return Vector2.zero;

        return Context.CurrentDirection.ToVector();
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            TryChangeDirection(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TryChangeDirection(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            TryChangeDirection(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TryChangeDirection(Direction.Right);
        }
    }

    private void TryChangeDirection(Direction direction)
    {
        if (Context.CurrentDirection == direction && !_isIdle) return;

        _nextDirection = direction;

        _cayoteTimer.Reset();
        _cayoteTimer.Start();
    }

    private void UpdateDirection()
    {
        if (!_cayoteTimer.IsRunning) return;

        _cayoteTimer.Update(Time.deltaTime);

        if (_cayoteTimer.IsFinished) 
        {
            _cayoteTimer.Stop();

            return;
        }

        if (!_isIdle || _nextDirection == Context.CurrentDirection) return;

        Context.SetDirection(_nextDirection);
        _isIdle = false;

        _currentVelocity = Vector2.zero;

        SnapToGrid();
        
        _cayoteTimer.Stop();
    }

    private void CollisionCheck() 
    { 
        if (IsFacingWall(Context.CurrentDirection.ToVector().ToVector3WithZ(z: 0)))
        {
            _isIdle = true;
            Context.HitWall(Context.CurrentDirection);

            _currentVelocity = Vector2.zero;
            SnapToGrid();
        }
    }

    private void SnapToGrid() 
    { 
        _playerTransform.position = new Vector3(Mathf.Round(_playerTransform.position.x), Mathf.Round(_playerTransform.position.y), _playerTransform.position.z); 
    }

    private bool IsFacingWall(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _layerMask);

        Context.SetWallRaycastHit(hit);

        return hit.collider != null;
    }

    private void OnDrawGizmos() 
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawSphere(_colliderTransform.position + Context.CurrentDirection.ToVector().ToVector3WithZ(z: 0f) * _raycastDistance, 0.1f);
    }
}