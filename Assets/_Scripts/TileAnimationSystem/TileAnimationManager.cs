using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileAnimationSystem
{
    public class TileAnimationManager : MonoBehaviour
    {
        public static TileAnimationManager Instance { get; private set; }


        [field: SerializeField] public SwappableTilemapRenderer TilesRenderer { get; private set; }
        [SerializeField] private SpriteRenderer _tileEffectPrefab;

        private readonly List<ActiveTileAnimation> _activeAnimations = new();
        private TileSpritePool _spritePool;

        private void Awake()
        {
            Instance = this;
            _spritePool = TileSpritePool.CreateInstance(_tileEffectPrefab);
        }

        public Task AnimateTile(TileAnimationData data)
        {
            var renderer = _spritePool.GetFromPool();
            var animation = new ActiveTileAnimation(data, renderer, this);
            _activeAnimations.Add(animation);

            return animation.AnimationTask;
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
