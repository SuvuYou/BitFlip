using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileAnimationSystem
{
    public class TileAnimationManager : MonoBehaviour
    {
        public static TileAnimationManager Instance { get; private set; }

        [field: SerializeField] public Tilemap TilesMap { get; private set; }
        [SerializeField] private SpriteRenderer _tileEffectPrefab;

        private readonly List<ActiveTileAnimation> _activeAnimations = new();
        private TileSpritePool _spritePool;

        private void Awake()
        {
            Instance = this;
            _spritePool = TileSpritePool.CreateInstance(_tileEffectPrefab);
        }

        public void AnimateTile(TileAnimationData data)
        {
            var renderer = _spritePool.GetFromPool();
            var animation = new ActiveTileAnimation(data, renderer, this);
            _activeAnimations.Add(animation);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            for (int i = _activeAnimations.Count - 1; i >= 0; i--)
            {
                var anim = _activeAnimations[i];
                bool finished = anim.Update(dt);

                if (finished)
                {
                    _spritePool.ReturnToPool(anim.Renderer);
                    _activeAnimations.RemoveAt(i);
                }
            }
        }
    }
}
