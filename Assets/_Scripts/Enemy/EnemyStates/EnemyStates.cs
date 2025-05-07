using System.Collections.Generic;
using UnityEngine;

public class IdleState : EnemyStateBase
{
    private float _idleTimer;
    private const float _idleDuration = 2f;

    public IdleState(SwappableEnemy enemy) : base(enemy) {}

    public override void Enter()
    {
        _idleTimer = 0f;
    }

    public override void Update()
    {
        _idleTimer += Time.deltaTime;

        if (_idleTimer >= _idleDuration) _enemy.SetState(new ScoutState(_enemy));
    }
}

public class ScoutState : EnemyStateBase
{
    public ScoutState(SwappableEnemy enemy) : base(enemy) {}

    private Timer _scoutTimer = new();

    public override void Enter()
    {
        _scoutTimer.Reset();
        _scoutTimer.Start();
    }

    public override void Update()
    {
        _scoutTimer.Update(Time.deltaTime);

        // foreach (Direction dir in DirectionExtentions.AllDirections)
        // {
        //     if (_enemy.EnemyMovementComponent.IsFacingTarget(dir.ToVector()))
        //     {
        //         _enemy.SetState(new AttackState(_enemy, dir));
                
        //         return;
        //     }
        // }

        if (_scoutTimer.IsFinished) 
        {
            _enemy.SetState(new WanderState(_enemy));
        }
    }
}

public class WanderState : EnemyStateBase
{
    private Direction _nextDirection;

    public WanderState(SwappableEnemy enemy) : base(enemy) {}

    public override void Enter()
    {
        List<Direction> possibleDirections = new(4);

        foreach (var dir in DirectionExtentions.AllDirections)
        {
            if (!_enemy.EnemyMovementComponent.IsFacingWall(dir.ToVector())) possibleDirections.Add(dir);
        }

        if (possibleDirections.Count == 0)
        {
            _enemy.SetState(new IdleState(_enemy));

            return;
        }

        int idx = _enemy.Random.GetRandomInt(0, possibleDirections.Count);

        _nextDirection = possibleDirections[idx]; 
    }

    public override void Update() 
    {
        _enemy.EnemyMovementComponent.MoveInDirection(_nextDirection);

        if (_enemy.Context.MovementState.IsIdle) _enemy.SetState(new IdleState(_enemy));
    }
}

public class AttackState : EnemyStateBase
{
    private Direction _direction;

    public AttackState(SwappableEnemy enemy, Direction direction) : base(enemy)
    {
        _direction = direction;
    }

    public override void Enter()
    {
        // Do attack movement
        // _enemy.Dash(_direction);
        _enemy.SetState(new ScoutState(_enemy));
    }
}