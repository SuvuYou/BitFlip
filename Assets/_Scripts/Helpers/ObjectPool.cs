using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private List<T> _pool;
    private T _prefab;
    private Transform _parent;
    private System.Func<T, bool> _isAvailable;

    public ObjectPool(T prefab, Transform parent, int initialPoolSize, System.Func<T, bool> availabilityPredicate)
    {
        this._prefab = prefab;
        this._parent = parent;
        this._isAvailable = availabilityPredicate;

        _pool = new List<T>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
        }
    }

    public T GetObject()
    {
        foreach (T obj in _pool)
        {
            if (_isAvailable(obj))
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        
        T newObj = GameObject.Instantiate(_prefab, _parent);
        _pool.Add(newObj);

        return newObj;
    }
}