public interface IEnemyState
{
    void Enter();
    void Exit();
    void Update();
}

public abstract class EnemyStateBase : IEnemyState
{
    protected SwappableEnemy _enemy;

    protected EnemyStateBase(SwappableEnemy enemy)
    {
        _enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}