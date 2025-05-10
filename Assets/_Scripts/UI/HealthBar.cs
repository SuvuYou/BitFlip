using TMPro;
using UnityEngine;

public class HealthBar
{
    [SerializeField] private HealthContextDataSO _healthContextDataSO;
    // [SerializeField] private TextMeshProUGUI _healthText;

    private void Awake() 
    {
        _healthContextDataSO.HealthState.OnGetHit += OnGetHit;
    }

    private void OnGetHit(int decrease) 
    {
        
    }
}