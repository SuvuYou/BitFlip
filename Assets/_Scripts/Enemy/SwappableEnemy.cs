using System.Collections.Generic;
using SwapSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SwappableEnemy")]
public class SwappableEnemyStats : ScriptableObject
{
    public SwapVariant Variant;
    public Sprite DisplaySprite;
    public bool IsKillable;
}

public class SwappableEnemy : MonoBehaviour, ISwappable, IConsumer<EnemyContextData>
{
    public EnemyContextData Context { get; private set; }

    public void Inject(EnemyContextData context) 
    {
        Context = context;

        context.OnSwap += (SwapVariant newVariant) => SwapSprite(Context.VariantsLookup[newVariant].DisplaySprite);
    }

    public void Swap(SwapVariant variant) => Context.SetCurrentVariant(variant);
    public bool IsCurrentVariantEqualTo(SwapVariant variant) => Context.CurrentVariant == variant;

    [SerializeField] private SpriteRenderer _displaySpriteRenderer;
    [SerializeField] private UglySerializableDictionary<SwapVariant, SwappableEnemyStats> _variants;

    private PseudoRandom.SystemRandomManager _random;

    private void Awake() 
    {
        Context.SetVariantsLookup(_variants.ToDictionary());

        (this as ISwappable).Register();

        _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);
    } 

    private void SwapSprite(Sprite newSprite) => _displaySpriteRenderer.sprite = newSprite;

    enum EnemyState { Idle, Scout, Wander, Attack };

    private EnemyState _state;

    private void Update() 
    {
        switch (_state)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Scout:
                break;
            case EnemyState.Wander:
                break;
            case EnemyState.Attack:
                break;
        }
    }

    private void Scout() 
    {
        // 1. search 4 directions for target
        // 1.5 if target found, check if wall in front
        // 2. if target found, transition to Attack, if not transition to Wander
    }

    private void Wander() 
    {
        // 1. search 4 directions for possible movement
        // 2 choose random one
        // 3. transition to Idle

        List<Direction> possibleDirections = new(4);

        foreach (Direction direction in DirectionExtentions.AllDirections)
        {
            if (!IsFacingWall(direction.ToVector())) possibleDirections.Add(direction);
        }

        if (possibleDirections.Count == 0) return;

        int randomDirectionIndex = _random.GetRandomInt(0, possibleDirections.Count);

        Direction randomDirection = possibleDirections[randomDirectionIndex];
    }

    private void Attack(Direction direction) 
    {
        // 1. dash in direction
        // 2. transition to Scout
    }

    private void Idle() 
    {
        // 1. wait
        // 2. transition to Scout
    }

    // Movement

    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private LayerMask _searchTargetMask;

    [SerializeField] private Transform _colliderTransform;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _raycastDistance = 1f;

    private void SearchTarget() 
    {
        foreach (Direction direction in DirectionExtentions.AllDirections)
        {
            if (!IsFacingTarget(direction.ToVector())) return;
        }
    }

    private void SeachPossibleMovementDirection()
    {
        foreach (Direction direction in DirectionExtentions.AllDirections)
        {
            if (!IsFacingWall(direction.ToVector())) return;
        }
    }

    private bool IsFacingWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _wallMask);

        return hit.collider != null;
    }

    private bool IsFacingTarget(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _searchTargetMask);

        return hit.collider != null;
    }

}

