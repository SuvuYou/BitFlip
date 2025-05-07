using System;

public class EnemyAttackDash : IEnemyAttack
{
    private SwappableEnemy _enemy;
    private Direction _targetDirection;
    private Action _onComplete;

    public EnemyAttackDash(SwappableEnemy enemy) 
    {
        _enemy = enemy;
    }

    public void StartAttack(Direction targetDirection, Action onComplete)
    {
        _targetDirection = targetDirection;
        _onComplete = onComplete;
    }

    public void AttackTick()
    {
        _enemy.EnemyMovementComponent.DashInDirection(_targetDirection);

        if (_enemy.Context.MovementState.IsIdle) _onComplete?.Invoke();
    }
}