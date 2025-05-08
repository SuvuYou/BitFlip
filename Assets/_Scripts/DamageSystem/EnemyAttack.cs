using UnityEngine;

public class EnemyCollisionAttack : MonoBehaviour, IEntityAttackComponent, IConsumer<EnemyContextData>
{
    public EnemyContextData Context { get; private set; }

    public void Inject(EnemyContextData context) => Context = context;

    [SerializeField] private int _damage = 1;

    public EntityAttack Attack { get; private set; }

    private void Start()
    {
        Attack = new EntityAttack(new EntityAttackStats(EntityType.Player, _damage));

        Attack.State.SetAttackType(EntityAttack.AttackType.EnemyBaseAttack);

        Context.OnSwap += (SwapSystem.SwapVariant newVariant, SwappableEnemyStats newVariantStats) => Attack.State.SetGuardType(newVariantStats.IsKillable ? EntityAttack.GuardType.None : EntityAttack.GuardType.Absolute);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Attack.HandleCollision(other);
    }
}
