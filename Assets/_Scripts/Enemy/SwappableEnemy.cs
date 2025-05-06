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
    public PseudoRandom.SystemRandomManager Random => _random;

    private void Awake() 
    {
        Context.SetVariantsLookup(_variants.ToDictionary());

        (this as ISwappable).Register();

        _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);
    } 

    private void SwapSprite(Sprite newSprite) => _displaySpriteRenderer.sprite = newSprite;

    private IEnemyState _currentState;

    public void SetState(IEnemyState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private void Update()
    {
        _currentState?.Update();
    }

    // Movement
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private LayerMask _searchTargetMask;

    [SerializeField] private Transform _colliderTransform;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _raycastDistance = 1f;

    private EntityMovement _movement;

    private void Start()
    {
        _movement = new EntityMovement
        (
            new EntityMovementStats(transform, _colliderTransform, _maxSpeed, _acceleration, _raycastDistance, _wallMask),
            Context.MovementState
        );
    }

    public void MoveInDirection() => _movement.TryMoveInDirection();

    public bool IsFacingWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _wallMask);

        return hit.collider != null;
    }

    public bool IsFacingTarget(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_colliderTransform.position, new Vector2(0.25f, 0.25f), 0, direction, _raycastDistance, _searchTargetMask);

        return hit.collider != null;
    }
}

