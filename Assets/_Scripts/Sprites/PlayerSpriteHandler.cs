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

        Context.OnDirectionChanged += ResetSpritePotion;
        Context.OnHitWall += SnapSpriteToWall;
    }

    private void SnapSpriteToWall(Direction direction) 
    { 
        if (Context.WallRaycastHit.collider != null) 
        { 
            Vector3 offset = _spriteWallOffset.ContainsKey(direction) ? _spriteWallOffset[direction] : Vector3.zero;

            _spriteTransform.position = new Vector3(Context.WallRaycastHit.point.x, Context.WallRaycastHit.point.y, 0) + offset;
        }
    }

    private void ResetSpritePotion(Direction direction) => _spriteTransform.localPosition = _defaultLocalSpritePosition;
}