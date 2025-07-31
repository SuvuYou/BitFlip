using System;
using UnityEngine;

public class GameEvent<T> : ScriptableObject
{
    private event Action<T> _listeners;

    public void Raise(T data) => _listeners?.Invoke(data);

    public void Register(Action<T> listener) => _listeners += listener;
    public void Unregister(Action<T> listener) => _listeners -= listener;
}
