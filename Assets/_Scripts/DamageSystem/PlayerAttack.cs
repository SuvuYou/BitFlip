using UnityEngine;

public class PlayerCollisionAttack : MonoBehaviour, IEntityAttackComponent, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) => Context = context;

    [SerializeField] private int _damage = 1;

    public EntityAttack Attack { get; private set; }

    private void Start()
    {
        Attack = new EntityAttack(new EntityAttackStats(EntityType.Enemy, _damage));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Attack.HandleCollision(other);  
    }

    // TODO: On player input set attack type to PlayerBaseAttack
}