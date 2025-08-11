using System.Collections;
using System.Collections.Generic;
using TileAnimationSystem;
using UnityEngine;

public class TilemapWaveAnimator : MonoBehaviour
{
    [SerializeField] private BasePaletteMappingSO _paletteMappingSO;

    public float delayBetweenDiagonals = 0.1f;
    public float waveDuration = 1f;
    public float height = 1f;

    public void StartWave()
    {
        StartCoroutine(WaveCoroutine());
    }

    private IEnumerator WaveCoroutine()
    {
        var tilemap = TileAnimationManager.Instance.TilesRenderer.Tilemap;
        var bounds = tilemap.cellBounds;
        Dictionary<int, List<Vector3Int>> diagonals = new();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new(x, y, 0);
                if (!tilemap.HasTile(pos)) continue;

                int diag = x + y;
                if (!diagonals.ContainsKey(diag))
                    diagonals[diag] = new List<Vector3Int>();

                diagonals[diag].Add(pos);
            }
        }

        List<int> keys = new(diagonals.Keys);
        keys.Sort();

        foreach (int key in keys)
        {
            List<Vector3Int> positions = diagonals[key];
            positions.Shuffle();

            foreach (var pos in positions)
            {
                var sprite = tilemap.GetSprite(pos);
                if (sprite == null) continue;

                var data = new TileAnimationSystem.TileAnimationData
                {
                    TilePosition = pos,
                    Sprite = sprite,
                    Duration = waveDuration * Random.Range(0.8f, 1.2f),
                    Material = _paletteMappingSO.PaletteMaterial,
                    MovementCurve = t => Mathf.Sin(t * Mathf.PI) * height,
                    HideTilemapTile = true
                };

                TileAnimationManager.Instance.AnimateTile(data);
            }

            yield return new WaitForSeconds(delayBetweenDiagonals);
        }
    }
}
