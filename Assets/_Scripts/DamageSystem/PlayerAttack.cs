using UnityEngine;

public class PlayerCollisionAttack : MonoBehaviour, IEntityAttackComponent, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    private Timer _attackDurationTimer;

    public void Inject(PlayerContextData context) => Context = context;

    [SerializeField] private int _damage = 1;
    [SerializeField] private float _attackDuration = 3f;

    public EntityAttack Attack { get; private set; }

    private void Start()
    {
        Attack = new EntityAttack(new EntityAttackStats(EntityType.Enemy, _damage));

        _attackDurationTimer = new Timer(_attackDuration);

        Context.MovementState.OnHitWall += (Direction direction) => ExitAttackMode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Attack.HandleCollision(other);  
    }

    private void Update()
    {
        _attackDurationTimer.Update(Time.deltaTime);

        if (_attackDurationTimer.IsFinished) ExitAttackMode();

        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && !_attackDurationTimer.IsRunning)
        {
            EnterAttackMode();
        }
    }

    private void ExitAttackMode() 
    {
        Attack.State.SetAttackType(EntityAttack.AttackType.None);

        _attackDurationTimer.Stop();

        Context.OnExitAttackMode?.Invoke();
    } 

    private void EnterAttackMode() 
    {
        Attack.State.SetAttackType(EntityAttack.AttackType.PlayerBaseAttack);

        _attackDurationTimer.Reset();
        _attackDurationTimer.Start();

        Context.OnEnterAttackMode?.Invoke();
    }
}