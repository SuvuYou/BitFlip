using SwapSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SwappableEnemy")]
public class SwappableEnemyStats : ScriptableObject
{
    public SwapVariant Variant;
    public Sprite DisplaySprite;
    public bool IsKillable;

    // Timers
    public float IdleTime;
    public float ScoutTime;
    public float ScoutInterval;
    public float WanderTime;
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

    [field: SerializeField] public EnemyMovement EnemyMovementComponent { get; private set; }
    public IEnemyAttack AttackComponent { get; private set; }

    [SerializeField] private SpriteRenderer _displaySpriteRenderer;
    [SerializeField] private UglySerializableDictionary<SwapVariant, SwappableEnemyStats> _variants;

    private PseudoRandom.SystemRandomManager _random;
    public PseudoRandom.SystemRandomManager Random => _random;

    private void SwapSprite(Sprite newSprite) => _displaySpriteRenderer.sprite = newSprite;

    private IEnemyState _currentState;

    private void Awake() 
    {
        Context.SetVariantsLookup(_variants.ToDictionary());

        (this as ISwappable).Register();

        _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);

        AttackComponent = new EnemyAttackDash(this);
        _currentState = new IdleState(this);
    } 

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
}

