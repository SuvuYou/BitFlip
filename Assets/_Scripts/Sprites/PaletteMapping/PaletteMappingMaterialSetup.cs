using UnityEngine;

public class PaletteMappingMaterialSetup : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private BasePaletteMappingSO _paletteMappingSO;

    private void Start()
    {
        Debug.Log($"PaletteMappingMaterialSetup {_renderer} {_paletteMappingSO}");

        if (_renderer != null && _paletteMappingSO != null)
        {
            _renderer.material = _paletteMappingSO.PaletteMaterial;
            
        }
    }
}