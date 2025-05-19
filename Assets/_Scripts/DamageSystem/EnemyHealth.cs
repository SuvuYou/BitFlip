using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEntityHealthComponent, IConsumer<EnemyContextData>, IConsumer<IHealthContextData>
{
    public EnemyContextData EnemyContext { get; private set; }
    public IHealthContextData HealthContext { get; private set; }

    void IConsumer<EnemyContextData>.Inject(EnemyContextData context) => EnemyContext = context;
    void IConsumer<IHealthContextData>.Inject(IHealthContextData context) => HealthContext = context;

    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private float _damageCooldown = 1;

    public EntityHealth Health { get; private set; }

    private void Start()
    {
        Health = new EntityHealth(new EntityHealthStats(EntityType.Enemy, _maxHealth, _damageCooldown), HealthContext.HealthState);

        HealthContext.HealthState.OnDie += () => Destroy(transform.root.gameObject);
    }

    private void Update()
    {
        Health.TickDamageImmunityTimer();
    }

    public void Damage(int damage) => Health.Damage(damage);
}
