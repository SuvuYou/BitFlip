using UnityEngine;

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
