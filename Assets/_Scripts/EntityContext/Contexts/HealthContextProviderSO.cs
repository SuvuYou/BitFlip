using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HealthContextDataSO")]
public class HealthContextDataSO : ContextDataSO, IHealthContextData
{
    public EntityHealthState HealthState { get; private set; }

    public void SetHealthState(EntityHealthState state) => HealthState = state;
}

public class HealthContextProviderSO : ContextProvider<IHealthContextData> 
{
    [SerializeField] private HealthContextDataSO _healthContextDataSO;

    protected override void Awake() 
    {
        _contextData = _healthContextDataSO;
        _contextData.SetHealthState(new EntityHealthState());

        base.Awake();
    }
}
