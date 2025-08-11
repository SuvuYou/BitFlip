using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TileAnimationSystem
{
    public class ActiveTileAnimation
    {
        public readonly SpriteRenderer Renderer;
        private readonly TileAnimationData _data;
        private readonly Vector3 _baseWorldPosition;
        private float _elapsed;

        private TileAnimationManager _animationManager;
        private readonly TaskCompletionSource<bool> _task = new();

        public Task AnimationTask => _task.Task;

        public ActiveTileAnimation(TileAnimationData data, SpriteRenderer renderer, TileAnimationManager animationManager)
        {
            _data = data;
            Renderer = renderer;
            _animationManager = animationManager;
            _baseWorldPosition = _animationManager.TilesRenderer.Tilemap.GetCellCenterWorld(data.TilePosition);

            Renderer.sprite = _data.Sprite;
            Renderer.material = _data.Material;
            Renderer.transform.position = _baseWorldPosition;

            if (_data.HideTilemapTile) _animationManager.TilesRenderer.SetColor(_data.TilePosition, new Color(1f, 1f, 1f, 0f));
        }

        public bool Update(float deltaTime)
        {
            _elapsed += deltaTime;

            float t = Mathf.Clamp01(_elapsed / _data.Duration);
            float offset = _data.MovementCurve?.Invoke(t) ?? 0f;

            Renderer.transform.position = _baseWorldPosition + Vector3.up * offset;

            if (t >= 1f && !_data.Loop)
            {
                if (_data.HideTilemapTile) _animationManager.TilesRenderer.SetColor(_data.TilePosition, Color.white);

                _task.TrySetResult(true);
                return true;
            }

            return false;
        }
    }

    public class TileAnimationData
    {
        public Vector3Int TilePosition;
        public float Duration;
        public float Height;
        public float Delay;
        public bool Loop;
        public Func<float, float> MovementCurve;
        public Sprite Sprite;
        public Material Material;
        public bool HideTilemapTile = true;
    }
}
