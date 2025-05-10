using UnityEngine;

public class PlayerHealth : MonoBehaviour, IEntityHealthComponent, IConsumer<PlayerContextData>, IConsumer<IHealthContextData>
{
    public PlayerContextData PlayerContext { get; private set; }
    public IHealthContextData HealthContext { get; private set; }

    void IConsumer<PlayerContextData>.Inject(PlayerContextData context) => PlayerContext = context;
    void IConsumer<IHealthContextData>.Inject(IHealthContextData context) => HealthContext = context;

    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _damageCooldown = 1;

    public EntityHealth Health { get; private set; }

    private void Start()
    {
        Health = new EntityHealth(new EntityHealthStats(EntityType.Player, _maxHealth, _damageCooldown), HealthContext.HealthState);
    }

    private void Update()
    {
        Health.TickDamageImmunityTimer();
    }

    public void Damage(int damage) => Health.Damage(damage);
}
