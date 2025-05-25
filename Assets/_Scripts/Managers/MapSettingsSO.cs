
using UnityEngine;

[CreateAssetMenu(fileName = "MapSettingsSO", menuName = "ScriptableObjects/MapSettingsSO")]
public class MapSettingsSO : ScriptableObject
{
    public int MapWidth = 31, MapHeight = 31, MapStemLength = 2, MaxNumberOfDungeonRooms = 2;

    public Vector2Int MinDungeonRoomSize, MaxDungeonRoomSize;

    public Vector2Int MapBorderSize = new (4, 2);

    public Vector2Int DungeonRoomBorderSize = new (1, 1);
}