using System.Collections;
using UnityEngine;

public class PlayerCollisionAttack : MonoBehaviour, IEntityAttackComponent, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    private Timer _attackDurationTimer;

    public void Inject(PlayerContextData context) => Context = context;

    private const float ATTACK_MODE_BUFFER_TIME = 0.05f;

    [SerializeField] private int _damage = 1;
    [SerializeField] private float _attackDuration = 3f;

    private bool _isInAttackMode = false;

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

        if (_attackDurationTimer.IsFinished && _attackDurationTimer.IsRunning) ExitAttackMode();

        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && !_attackDurationTimer.IsRunning && !Context.MovementState.IsIdle)
        {
            EnterAttackMode();
        }
    }

    private void ExitAttackMode() 
    {
        if (!_isInAttackMode) return;

        StartCoroutine(DisableAttackTypeAfterBufferTime());

        _attackDurationTimer.Stop();

        Context.OnExitAttackMode?.Invoke();

        _isInAttackMode = false;
    } 

    private IEnumerator DisableAttackTypeAfterBufferTime()
    {
        yield return new WaitForSeconds(ATTACK_MODE_BUFFER_TIME);

        Attack.State.SetAttackType(EntityAttack.AttackType.None);
    }

    private void EnterAttackMode() 
    {
        Attack.State.SetAttackType(EntityAttack.AttackType.PlayerBaseAttack);

        _attackDurationTimer.Reset();
        _attackDurationTimer.Start();

        Context.OnEnterAttackMode?.Invoke();

        _isInAttackMode = true;
    }
}