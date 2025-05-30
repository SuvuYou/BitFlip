using UnityEngine;

public class PaletteMappingMaterialSetup : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private BasePaletteMappingSO _paletteMappingSO;

    private void OnEnable()
    {
        if (_renderer != null && _paletteMappingSO != null)
        {
            _renderer.material = _paletteMappingSO.PaletteMaterial;
        }
    }
}