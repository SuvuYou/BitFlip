using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaletteMappingSwapSO", menuName = "ScriptableObjects/PaletteMappingSwapSO")]
public class PaletteMappingSwapSO : BasePaletteMappingSO
{
    private readonly int PALETTE_PROPERTY_ID = Shader.PropertyToID("_PaletteTexture");

    [SerializeField] private UglySerializableDictionary<SwapSystem.SwapVariant, List<Color>> _paletteVarients = new ();

    [SerializeField] private SwapSystem.SwapVariant _defaultVariant;

    private Dictionary<SwapSystem.SwapVariant, List<Color>> _palettesLookup = new ();
    private Dictionary<SwapSystem.SwapVariant, Texture2D> _texturesLookup = new ();

    private SwapSystem.SwapVariant _currentVariant;

    private void OnValidate()
    {
        if (_palettesLookup.Count == 0) return;

        foreach (var kvp in _texturesLookup)
        {
            _texturesLookup[kvp.Key] = GenerateTexture(_palettesLookup[kvp.Key]); 
        }
        
        UpdateMaterial(_currentVariant);
    }

    public override void SetupMaterial() 
    {
        _palettesLookup = _paletteVarients.ToDictionary();

        UpdateMaterial(_defaultVariant);
    } 

    public void UpdateMaterial(SwapSystem.SwapVariant currentVariant) 
    {
        _currentVariant = currentVariant;

        if (!_texturesLookup.ContainsKey(currentVariant)) return;

        PaletteMaterial.SetTexture(PALETTE_PROPERTY_ID, _texturesLookup[currentVariant]);
    } 

    private Texture2D GenerateTexture(List<Color> palette)
    {
        Texture2D texture = new (width: palette.Count, height: 1, textureFormat: TextureFormat.RGBA32, mipChain: false);

        for (int i = 0; i < palette.Count; i++)
        {
            texture.SetPixel(i, 0, palette[i]);
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture; 
    }
}