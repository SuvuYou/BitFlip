using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaletteMappingSO", menuName = "ScriptableObjects/PaletteMappingSO")]
public class PaletteMappingSO : BasePaletteMappingSO
{
    [SerializeField] private List<Color> _palette = new ();

    private readonly int PALETTE_PROPERTY_ID = Shader.PropertyToID("_PaletteTexture");

    private Texture2D _texture;

    private void OnValidate()
    {
        SetupMaterial();
    }

    public override void SetupMaterial() 
    {
        if (_palette.Count == 0) return;

        _texture = GenerateTexture();

        UpdateMaterial();
    } 

    public void UpdateMaterial() => PaletteMaterial.SetTexture(PALETTE_PROPERTY_ID, _texture);

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