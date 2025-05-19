using SwapSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SwappableEnemy")]
public class SwappableEnemyStats : ScriptableObject
{
    public SwapVariant Variant;
    public Sprite DisplaySprite;
    public bool IsKillable;

    // Timers
    public float IdleTime;
    public float ScoutTime;
    public float ScoutInterval;
    public float WanderTime;
}
