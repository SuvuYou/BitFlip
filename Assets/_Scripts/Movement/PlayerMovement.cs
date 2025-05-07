using UnityEngine;

public class PlayerMovement : MonoBehaviour, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) => Context = context;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _colliderTransform;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _dashMultiplier = 2f;
    [SerializeField] private float _raycastDistance = 1f;
    [SerializeField] private float _cayoteMovementTime = 0.1f;

    [SerializeField] private LayerMask _wallLayerMask;

    private Timer _cayoteTimer;

    private Direction _nextDirection = Direction.Up;

    private EntityMovement _movement;

    private void Start()
    {
        _cayoteTimer = new Timer(_cayoteMovementTime);

        _movement = new EntityMovement
        (
            new EntityMovementStats(_playerTransform, _colliderTransform, _maxSpeed, _acceleration, _dashMultiplier, _raycastDistance, _wallLayerMask),
            Context.MovementState
        ); 
    }

    private void Update()
    {
        GetInput();
        UpdateDirection();

        _movement.TryMoveInDirection();
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
        if (Context.MovementState.CurrentDirection == direction && !Context.MovementState.IsIdle) return;

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

        if (!Context.MovementState.IsIdle || _nextDirection == Context.MovementState.CurrentDirection) return;

        _movement.SetDirection(_nextDirection);
        Context.SetDirection(_nextDirection);

        _cayoteTimer.Stop();
    }

    private void OnDrawGizmos() 
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawSphere(_colliderTransform.position + Context.MovementState.CurrentDirection.ToVector().ToVector3WithZ(z: 0f) * _raycastDistance, 0.1f);
    }
}