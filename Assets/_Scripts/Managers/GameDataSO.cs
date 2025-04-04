
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
public class GameDataSO : ScriptableObject
{
    public int MapWidth = 31, MapHeight = 31, MapStemLength = 2;

    public Vector2Int MapBorderSize = new (4, 2);
}