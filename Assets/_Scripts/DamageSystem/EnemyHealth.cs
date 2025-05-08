using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEntityHealthComponent, IConsumer<EnemyContextData>
{
    public EnemyContextData Context { get; private set; }

    public void Inject(EnemyContextData context) => Context = context;

    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private float _damageCooldown = 1;

    public EntityHealth Health { get; private set; }

    private void Start()
    {
        Health = new EntityHealth(new EntityHealthStats(EntityType.Enemy, _maxHealth, _damageCooldown), Context.HealthState);
    }

    private void Update()
    {
        Health.TickDamageImmunityTimer();
    }

    public void Damage(int damage) => Health.Damage(damage);
}
