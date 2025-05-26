using UnityEngine;

class MapSettingsProvider : Singleton<MapSettingsProvider>
{
    [SerializeField] private MapSettingsSO _mapSettings;

    public MapSettingsSO MapSettings => _mapSettings;
}