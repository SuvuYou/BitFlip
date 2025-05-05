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

    private void Awake() 
    {
        Context.SetVariantsLookup(_variants.ToDictionary());

        (this as ISwappable).Register();
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
}

