using UnityEngine;

public interface IDamageDealer
{
    public int Damage { get; }
}

public class CollisionDamageDealer : MonoBehaviour, IDamageDealer
{
    [field: SerializeField] public int Damage { get; private set; } = 1;
}