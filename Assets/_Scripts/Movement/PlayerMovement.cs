using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum Direction { Idle, Up, Down, Left, Right };

    private Dictionary<Direction, Vector2> _directionsLookup = new()
    {
        { Direction.Up, Vector2.up },
        { Direction.Down, Vector2.down },
        { Direction.Left, Vector2.left },
        { Direction.Right, Vector2.right },
        { Direction.Idle, Vector2.zero }
    };

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _colliderTransform;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _raycastDistance = 1f;
    [SerializeField] private LayerMask _layerMask;

    private Vector2 _currentVelocity = Vector2.zero;
    private Direction _currentDirection;

    private void Update()
    {
        GetInput();
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
        return _directionsLookup[_currentDirection];
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ChangeDirection(Direction.Up);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ChangeDirection(Direction.Down);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            ChangeDirection(Direction.Left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ChangeDirection(Direction.Right);
        }
        else if (Input.GetKey(KeyCode.K))
        {
            ChangeDirection(Direction.Idle);
        }
    }

    private void ChangeDirection(Direction direction)
    {
        if (_currentDirection == direction) return;

        if (_directionsLookup[_currentDirection] == -_directionsLookup[direction]) return;

        if (IsFacingWall(_directionsLookup[direction])) return;

        _currentDirection = direction;

        _currentVelocity = Vector2.zero;

        SnapToGrid();
    }

    private void SnapToGrid() 
    { 
        _playerTransform.position = new Vector3(Mathf.Round(_playerTransform.position.x), Mathf.Round(_playerTransform.position.y), _playerTransform.position.z); 
    }

    private void CollisionCheck() 
    { 
        if (IsFacingWall(_directionsLookup[_currentDirection]))
        {
            ChangeDirection(Direction.Idle);
        }
    }

    private bool IsFacingWall(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.5f, 0.5f), 0, direction, _raycastDistance - 0.25f, _layerMask);

        return hit.collider != null;
    }

    private void OnDrawGizmos() 
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawSphere(_colliderTransform.position + _directionsLookup[_currentDirection].ToVector3WithZ(z: 0) * (_raycastDistance - 0.25f), 0.1f);
    }
}