using System.Threading.Tasks;
using DG.Tweening;
using TileAnimationSystem;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MapRendering
{
    public class DungeonDoorView : MonoBehaviour
    {
        [SerializeField] private Sprite _lockedDoorSprite;
        [SerializeField] private Sprite _unlockedDoorSprite;
        [SerializeField] private Sprite _keySprite;
        [SerializeField] private SwappableTilemapRenderer _tilemapRenderer;
        [SerializeField] private BasePaletteMappingSO _paletteMappingSO;

        private Vector3Int _tilePos;

        public void Init(Vector2Int tilePos)
        {
            _tilePos = new Vector3Int(tilePos.x, tilePos.y, 0);
        }

        public async Task PlayOpenAnimation()
        {
            _tilemapRenderer.SetColor(_tilePos, new Color(1f, 1f, 1f, 0f));

            await ShakeTile(duration: 1.0f, intensity: 0.1f);

            _tilemapRenderer.SetColor(_tilePos, Color.white);

            await AnimateKeyToDoorCenter();

            _tilemapRenderer.SetColor(_tilePos, new Color(1f, 1f, 1f, 0f));
        }

        public async Task PlayLockAnimation()
        {
            await JumpTileDown(duration: 0.3f, height: 0.3f);
        }

        private async Task ShakeTile(float duration, float intensity)
        {
            var worldPos = TileAnimationManager.Instance.TilesRenderer.Tilemap.GetCellCenterWorld(_tilePos);
            var fakeTile = new GameObject("ShakeTileFX");
            fakeTile.transform.position = worldPos;

            var spriteRenderer = fakeTile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = TileAnimationManager.Instance.TilesRenderer.Tilemap.GetSprite(_tilePos);
            spriteRenderer.sortingOrder = 999;

            await fakeTile.transform.DOShakePosition(duration, intensity, vibrato: 20, randomness: 90, fadeOut: true).ToUniTask();
            Tweener shakeTween = transform.DOShakePosition(1f);
            await shakeTween.ToUniTask();

            Destroy(fakeTile);
        }

        private async Task AnimateKeyToDoorCenter()
        {
            var keyGO = new GameObject("KeyFX");
            var spriteRenderer = keyGO.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _keySprite;
            spriteRenderer.material = _paletteMappingSO.PaletteMaterial;
            spriteRenderer.sortingOrder = 999;

            Vector3 startPos = TileAnimationManager.Instance.TilesRenderer.Tilemap.GetCellCenterWorld(_tilePos) + Vector3.up * 1.2f;
            Vector3 targetPos = TileAnimationManager.Instance.TilesRenderer.Tilemap.GetCellCenterWorld(_tilePos);

            keyGO.transform.position = startPos;

            await keyGO.transform.DOMove(targetPos, 0.6f).SetEase(Ease.InQuad).ToUniTask();

            await keyGO.transform
                .DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
                .ToUniTask();

            Destroy(keyGO);
        }

        private async Task JumpTileUp(float duration, float height)
        {
            var data = new TileAnimationData
            {
                TilePosition = _tilePos,
                Duration = duration,
                Height = height,
                MovementCurve = t => t * height,
                Sprite = TileAnimationManager.Instance.TilesRenderer.Tilemap.GetSprite(_tilePos),
                Material = _paletteMappingSO.PaletteMaterial,
                HideTilemapTile = false
            };

            await TileAnimationManager.Instance.AnimateTile(data);
        }

        private async Task JumpTileDown(float duration, float height)
        {
            var data = new TileAnimationData
            {
                TilePosition = _tilePos,
                Duration = duration,
                Height = height,
                MovementCurve = t => height * (1 - Mathf.Cos(t * Mathf.PI)),
                Sprite = TileAnimationManager.Instance.TilesRenderer.Tilemap.GetSprite(_tilePos),
                Material = _paletteMappingSO.PaletteMaterial,
                HideTilemapTile = false
            };

            await TileAnimationManager.Instance.AnimateTile(data);
        }
    }
}
