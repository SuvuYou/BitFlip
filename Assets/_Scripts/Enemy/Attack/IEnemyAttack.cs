using System;

public interface IEnemyAttack
{
    public void StartAttack(Direction targetDirection, Action onComplete);
    public void AttackTick();
}