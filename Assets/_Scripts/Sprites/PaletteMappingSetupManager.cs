using UnityEngine;

public class PaletteMappingSetupManager : MonoBehaviour
{
    [SerializeField] private BasePaletteMappingSO _paletteMappingSO;

    private void Start()
    {
        _paletteMappingSO.SetupMaterial();
    }
}