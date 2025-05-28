using System.Collections.Generic;
using UnityEngine;

public class InEditorPaletteSwap : MonoBehaviour
{
    [SerializeField] private InEditorPaletteSwapSO _inEditorPaletteSwapSO;
    [SerializeField] private SpriteRenderer _sprite;

    private readonly int PALETTE_PROPERTY_ID = Shader.PropertyToID("_PaletteTexture");

    private void OnValidate()
    {
        // UpdateTexture(_inEditorPaletteSwapSO.Texture);
    }

    private void Start()
    {
        // UpdateTexture(_inEditorPaletteSwapSO.Texture);

        // _inEditorPaletteSwapSO.OnTextureUpdated += UpdateTexture;
    }

    private void UpdateTexture(Texture2D texture)
    {
        _sprite.sharedMaterial.SetTexture(PALETTE_PROPERTY_ID, texture);
    }
}