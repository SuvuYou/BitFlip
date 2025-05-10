using UnityEngine;
using UnityEngine.UI;

public class HealthBar: MonoBehaviour
{
    [SerializeField] private HealthContextDataSO _healthContextDataSO;
    [SerializeField] private RectTransform _healthBarContainer;
    [SerializeField] private Image _heartPrefab;

    private void Start() 
    {
        _healthContextDataSO.HealthState.OnGetHit += OnGetHit;
    }

    private void OnGetHit(int decrease) 
    {
        RemoveAllChildren();
        InstantiateHearts(_healthContextDataSO.HealthState.CurrentHealth);
    }

    private void RemoveAllChildren() 
    {
        foreach (Transform child in _healthBarContainer) 
        {
            Destroy(child.gameObject);
        }
    }

    private void InstantiateHearts(int numberOfHearts) 
    {
        for (int i = 0; i < numberOfHearts; i++)
        {
            Instantiate(_heartPrefab, _healthBarContainer);
        }
    }
}