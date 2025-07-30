using UnityEngine;

public class MovementCollisionPropagator : MonoBehaviour, IConsumer<ICollisionContextData>, IConsumer<PlayerContextData>
{
    public ICollisionContextData CollisionContext { get; private set; }
    public PlayerContextData PlayerContext { get; private set; }

    void IConsumer<ICollisionContextData>.Inject(ICollisionContextData context) => CollisionContext = context;
    void IConsumer<PlayerContextData>.Inject(PlayerContextData context) => PlayerContext = context;

    private void Awake()
    {
        PlayerContext.MovementState.OnHitWall += OnCollisionTrigger;
    }

    private void OnCollisionTrigger(Direction direction, RaycastHit2D lastHit)
    {
        CollisionContext.OnRaycastHitWall?.Invoke(lastHit, direction);
    }
}