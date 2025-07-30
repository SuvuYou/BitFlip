using UnityEngine;

public class CollisionPropagator : MonoBehaviour, IConsumer<ICollisionContextData>
{
    public ICollisionContextData CollisionContext { get; private set; }

    void IConsumer<ICollisionContextData>.Inject(ICollisionContextData context) => CollisionContext = context;

    private void OnTriggerEnter2D(Collider2D other) => CollisionContext.OnTriggerEnter?.Invoke(other);
    private void OnCollisionEnter2D(Collision2D other) => CollisionContext.OnCollisionEnter?.Invoke(other);
}