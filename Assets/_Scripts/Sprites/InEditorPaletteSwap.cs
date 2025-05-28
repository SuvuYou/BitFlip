using System.Collections.Generic;
using UnityEngine;

public class InEditorPaletteSwap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private InEditorPaletteSwapSO _inEditorPaletteSwapSO;

    private void Start()
    {
        _inEditorPaletteSwapSO.UpdateTexture();
    }
    
    private void OnEnable()
    {
        _sprite.material = _inEditorPaletteSwapSO.PaletteMaterial;
    }
}