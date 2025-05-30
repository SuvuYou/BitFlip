using SwapSystem;
using UnityEngine;

public class PaletteMappingSwap : MonoBehaviour, ISwappable
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

    [SerializeField] private PaletteMappingSwapSO _paletteMappingSwapSO;

    private SwapVariant _currentVariant;

    private void OnEnable()
    {
        (this as ISwappable).RegisterAsInitial();
    }
}