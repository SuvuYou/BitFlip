using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileAnimationSystem
{
    public class TileSpritePool : MonoBehaviour
    {
        public static TileSpritePool CreateInstance(SpriteRenderer prefab)
        {
            var instance = new GameObject($"{typeof(SpriteRenderer).Name}Pool").AddComponent<TileSpritePool>();
            instance.Init(prefab);

            return instance;
        }

        private SpriteRenderer _prefab;
        private readonly Stack<SpriteRenderer> _pool = new();

        private void Init(SpriteRenderer prefab)
        {
            _prefab = prefab;
            _pool.Clear();
        }

        public SpriteRenderer GetFromPool()
        {
            if (_pool.Count > 0)
            {
                var obj = _pool.Pop();
                obj.gameObject.SetActive(true);
                return obj;
            }

            return Instantiate(_prefab, transform);
        }

        public void ReturnToPool(SpriteRenderer sr)
        {
            sr.gameObject.SetActive(false);
            _pool.Push(sr);
        }
    }
}
