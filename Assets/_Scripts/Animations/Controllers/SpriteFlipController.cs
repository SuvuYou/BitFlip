using UnityEngine;

public class SpriteFlipController 
{
    private SpriteRenderer _spriteRenderer;

    public void Init(SpriteRenderer spriteRenderer) 
    {
        _spriteRenderer = spriteRenderer;
    }

    public void Flip(bool isFacingRight) => _spriteRenderer.flipX = !isFacingRight;
}
