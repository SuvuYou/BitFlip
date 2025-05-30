using UnityEngine;

public class PaletteMappingMaterialSetup : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private BasePaletteMappingSO _paletteMappingSO;

    private void Start()
    {
        if (_renderer != null && _paletteMappingSO != null)
        {
            _renderer.sharedMaterial = _paletteMappingSO.PaletteMaterial;
        }
    }
}