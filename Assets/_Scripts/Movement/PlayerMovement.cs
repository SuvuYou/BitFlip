using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) => Context = context;

    private Dictionary<Direction, Vector2> _directionsLookup = new()
    {
        { Direction.Up, Vector2.up },
        { Direction.Down, Vector2.down },
        { Direction.Left, Vector2.left },
        { Direction.Right, Vector2.right }
    };

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
        CollisionCheck();
        UpdateDirection();

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

        return _directionsLookup[Context.CurrentDirection];
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

        if (!_isIdle) return;

        Context.SetDirection(_nextDirection);
        _currentVelocity = Vector2.zero;
        SnapToGrid();

        _isIdle = false;
        _cayoteTimer.Stop();
    }

    private void CollisionCheck() 
    { 
        if (IsFacingWall(_directionsLookup[Context.CurrentDirection]))
        {
            _isIdle = true;

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

        return hit.collider != null;
    }

    private void OnDrawGizmos() 
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawSphere(_colliderTransform.position + _directionsLookup[Context.CurrentDirection].ToVector3WithZ(z: 0) * (_raycastDistance - 0.25f), 0.1f);
    }
}