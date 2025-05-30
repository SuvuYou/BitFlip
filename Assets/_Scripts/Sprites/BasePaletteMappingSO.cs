using UnityEngine;

public abstract class BasePaletteMappingSO : ScriptableObject
{
    [field: SerializeField] public Material PaletteMaterial { get; private set; }

    public abstract void SetupMaterial();
}