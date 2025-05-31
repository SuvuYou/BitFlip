using SwapSystem;
using UnityEngine;

public class SwappablePaletteMapping : MonoBehaviour, ISwappable
{
    public void Swap(SwapVariant variant) 
    {
        if (_currentVariant != variant) 
        {
            _currentVariant = variant;

            _paletteMappingSwapSO.UpdateMaterial(_currentVariant);
        }
    } 
    
    public bool IsCurrentVariantEqualTo(SwapVariant variant) => _currentVariant == variant;

    [SerializeField] private SwappablePaletteMappingSO _paletteMappingSwapSO;

    private SwapVariant _currentVariant;

    private void OnEnable()
    {
        (this as ISwappable).RegisterAsInitial();
    }
}