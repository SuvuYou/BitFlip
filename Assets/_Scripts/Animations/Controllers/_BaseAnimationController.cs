using UnityEngine;

public class BaseAnimationController : MonoBehaviour
{
    [Header("Base References")]
    [SerializeField] protected Animator _animator;

    protected int _currentState;

    protected float _lockedUntillTime = 0;

    protected void _switchAnimationState(int newState, float lockTime = 0f, bool shouldOverride = false)
    {
        if (Time.time < _lockedUntillTime && !shouldOverride) return;
        if (_currentState == newState) return;

        _animator.CrossFade(newState, 0, 0);
        _currentState = newState;

        _lockedUntillTime = Time.time + lockTime;
    }
}
