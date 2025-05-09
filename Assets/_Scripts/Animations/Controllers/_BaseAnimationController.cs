using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationController : MonoBehaviour
{
    public enum SwitchAnimationMode { Queue, Replace, ReplaceOverride }

    [Header("Base References")]
    [SerializeField] protected Animator _animator;

    protected Queue<(int, float)> _animationQueue = new();

    protected int _currentState;

    protected float _lockedUntillTime = 0;

    protected virtual void Update()
    {
        if (_animationQueue.Count > 0 && Time.time >= _lockedUntillTime)
        {
            (int newState, float lockTime) = _animationQueue.Dequeue();

            _switchAnimationState(newState, lockTime);
        }
    }

    protected void _switchAnimationState(int newState, float lockTime = 0f, SwitchAnimationMode mode = SwitchAnimationMode.Replace)
    {
        if (mode == SwitchAnimationMode.Queue)
        {
            _animationQueue.Enqueue((newState, lockTime));

            return;
        }

        if (Time.time < _lockedUntillTime && mode != SwitchAnimationMode.ReplaceOverride) return;
        if (_currentState == newState) return;

        if (mode == SwitchAnimationMode.ReplaceOverride) _animationQueue.Clear();

        _animator.CrossFade(newState, 0, 0);
        _currentState = newState;

        _lockedUntillTime = Time.time + lockTime;
    }
}
