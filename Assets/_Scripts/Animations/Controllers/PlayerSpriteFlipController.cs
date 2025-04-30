using UnityEngine;

public class PlayerSpriteFlipController : ConsumerBase<PlayerContextData>
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private SpriteFlipController _spriteFlipController;

    private void Awake() 
    {
        _spriteFlipController = new SpriteFlipController();
        _spriteFlipController.Init(_spriteRenderer);
    }

    private void Update()
    {
        _spriteFlipController.Flip(Context.IsFacingRight);
    }
}
