using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HealthContextDataSO")]
public class HealthContextDataSO : ContextDataSO, IHealthContextData
{
    public EntityHealthState HealthState { get; private set; }

    public void SetHealthState(EntityHealthState state) => HealthState = state;
}