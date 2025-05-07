using System.Collections.Generic;
using UnityEngine;

public class IdleState : EnemyStateBase
{
    private Timer _idleTimer;

    public IdleState(SwappableEnemy enemy) : base(enemy) 
    {
        _idleTimer = new(enemy.Context.CurrentVariantStats.IdleTime);
    }

    public override void Enter()
    {
        _idleTimer.Start();
        _idleTimer.Reset();
    }

    public override void Update()
    {
        _idleTimer.Update(Time.deltaTime);

        if (_idleTimer.IsFinished) _enemy.SetState(new ScoutState(_enemy));
    }
}

public class ScoutState : EnemyStateBase
{
    private Timer _scoutTimer = new();
    private Timer _scoutIntervalTimer = new();

    public ScoutState(SwappableEnemy enemy) : base(enemy) 
    {
        _scoutTimer = new(enemy.Context.CurrentVariantStats.ScoutTime);
        _scoutIntervalTimer = new(enemy.Context.CurrentVariantStats.ScoutInterval);
    }

    public override void Enter()
    {
        _scoutTimer.Reset();
        _scoutTimer.Start();
        
        _scoutIntervalTimer.Reset();
        _scoutIntervalTimer.Start();
    }

    private List<Direction> SeachTargets()
    {
        List<Direction> possibleDirections = new(4);

        foreach (var dir in DirectionExtentions.AllDirections)
        {
            if (_enemy.EnemyMovementComponent.IsFacingTarget(dir.ToVector())) possibleDirections.Add(dir);
        }

        return possibleDirections;
    }

    public override void Update()
    {
        _scoutTimer.Update(Time.deltaTime);
        _scoutIntervalTimer.Update(Time.deltaTime);

        if (_scoutIntervalTimer.IsFinished)
        {
            var possibleDirections = SeachTargets();

            if (possibleDirections.Count > 0)
            {
                int idx = _enemy.Random.GetRandomInt(0, possibleDirections.Count);

                _enemy.SetState(new AttackState(_enemy, possibleDirections[idx]));
            }

            _scoutIntervalTimer.Reset();
        }

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
        _enemy.AttackComponent.StartAttack(_direction, onComplete: () => _enemy.SetState(new IdleState(_enemy)));
    }

    public override void Update() 
    {
        _enemy.AttackComponent.AttackTick();
    }
}