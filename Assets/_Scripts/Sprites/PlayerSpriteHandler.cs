using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteHandler : MonoBehaviour, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) => Context = context;

    [SerializeField] private Transform _spriteTransform;
    [SerializeField] private Vector3 _defaultLocalSpritePosition = Vector3.zero;

    [SerializeField] private UglySerializableDictionary<Direction, Vector3> _spriteWallOffsetSerialized;

    private Dictionary<Direction, Vector3> _spriteWallOffset;

    private void Start()
    {
        _spriteWallOffset = _spriteWallOffsetSerialized.ToDictionary();

        Context.MovementState.OnChangeDirection += ResetSpritePotion;
        Context.MovementState.OnHitWall += SnapSpriteToWall;
    }

    private void SnapSpriteToWall(Direction direction) 
    {  
        Vector3 offset = _spriteWallOffset.ContainsKey(direction) ? _spriteWallOffset[direction] : Vector3.zero;

        _spriteTransform.position = new Vector3(Context.MovementState.ClosestWallPoint.x, Context.MovementState.ClosestWallPoint.y, 0) + offset;
    }

    private void ResetSpritePotion(Direction direction) => _spriteTransform.localPosition = _defaultLocalSpritePosition;
}