using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InEditorPaletteSwapSO", menuName = "ScriptableObjects/InEditorPaletteSwapSO")]
public class InEditorPaletteSwapSO : ScriptableObject
{
    [field: SerializeField] public Material PaletteMaterial;

    [SerializeField] private List<Color> _palette = new ();

    private readonly int PALETTE_PROPERTY_ID = Shader.PropertyToID("_PaletteTexture");

    public Texture2D Texture { get; private set; }

    private void OnValidate()
    {
        if (_palette.Count == 0) return;

        Texture = GenerateTexture();

        UpdateTexture();
    }

    public void UpdateTexture() 
    {
        PaletteMaterial.SetTexture(PALETTE_PROPERTY_ID, Texture);
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new (width: _palette.Count, height: 1, textureFormat: TextureFormat.RGBA32, mipChain: false);

        for (int i = 0; i < _palette.Count; i++)
        {
            texture.SetPixel(i, 0, _palette[i]);
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture; 
    }
}