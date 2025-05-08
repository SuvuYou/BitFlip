using UnityEngine;

public class PlayerHealth : MonoBehaviour, IEntityHealthComponent, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) => Context = context;

    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _damageCooldown = 1;

    public EntityHealth Health { get; private set; }

    private void Start()
    {
        Health = new EntityHealth(new EntityHealthStats(EntityType.Player, _maxHealth, _damageCooldown), Context.HealthState);
    }

    private void Update()
    {
        Health.TickDamageImmunityTimer();
    }

    public void Damage(int damage) => Health.Damage(damage);
}
