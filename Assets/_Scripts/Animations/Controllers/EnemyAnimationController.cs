using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : BaseAnimationController, IConsumer<EnemyContextData>
{
    public EnemyContextData Context { get; private set; }

    public void Inject(EnemyContextData context) 
    {
        Context = context;

        Context.SetWindupTime(_animator.GetClipLength("Idle_To_Fly_Right") - 0.25f);

        Debug.Log(_animator.GetClipLength("Idle_To_Fly_Right"));

        context.OnWindupMovementStart += SwitchTransitionAnimation;
        context.MovementState.OnChangeDirection += SwitchMoventAnimation;
        context.MovementState.OnHitWall += SwitchIdleAnimation;
    }

    private static readonly int IDLE_RIGHT = Animator.StringToHash("Idle_Right");
    private static readonly int IDLE_UP  = Animator.StringToHash("Idle_Up");
    private static readonly int IDLE_DOWN = Animator.StringToHash("Idle_Down");

    private static readonly int DASH_RIGHT = Animator.StringToHash("Fly_Right");
    private static readonly int DASH_UP = Animator.StringToHash("Fly_Up");
    private static readonly int DASH_DOWN = Animator.StringToHash("Fly_Down");

    private static readonly int IDLE_TO_DASH_RIGHT_TRANSITION = Animator.StringToHash("Idle_To_Fly_Right");
    private static readonly int IDLE_TO_DASH_UP_TRANSITION = Animator.StringToHash("Idle_To_Fly_Up");
    private static readonly int IDLE_TO_DASH_DOWN_TRANSITION = Animator.StringToHash("Idle_To_Fly_Down");

    private Dictionary<int, string> HASH_TO_ANIMATION_NAME_LOOKUP = new()
    {
        { IDLE_RIGHT, "Idle_Right" },
        { IDLE_UP, "Idle_Up" },
        { IDLE_DOWN, "Idle_Down" },

        { DASH_RIGHT, "Fly_Right" },
        { DASH_UP, "Fly_Up" },
        { DASH_DOWN, "Fly_Down" },

        { IDLE_TO_DASH_RIGHT_TRANSITION, "Idle_To_Fly_Right" },
        { IDLE_TO_DASH_UP_TRANSITION, "Idle_To_Fly_Up" },
        { IDLE_TO_DASH_DOWN_TRANSITION, "Idle_To_Fly_Down" },
    };

    private void SwitchTransitionAnimation(Direction newDirection) 
    {
        int newTransitionHash = GetTransitionAnimationHash(newDirection);

        _switchAnimationState(newTransitionHash, lockTime: _animator.GetClipLength(HASH_TO_ANIMATION_NAME_LOOKUP[newTransitionHash]), mode: SwitchAnimationMode.ReplaceOverride);
    } 

    private void SwitchMoventAnimation(Direction newDirection) 
    {
        int newDashHash = GetDashAnimationHash(newDirection);

        _switchAnimationState(newDashHash, mode: SwitchAnimationMode.ReplaceOverride);
    } 

    private void SwitchIdleAnimation(Direction fromDirection) 
    {
        _switchAnimationState(GetIdleAnimationHash(fromDirection), mode: SwitchAnimationMode.ReplaceOverride);
    }

    private int GetIdleAnimationHash(Direction fromDirection) => fromDirection switch 
    { 
        Direction.Up => IDLE_UP, 
        Direction.Down => IDLE_DOWN, 
        Direction.Left => IDLE_RIGHT, 
        Direction.Right => IDLE_RIGHT,
        _ => IDLE_UP
    };

    private int GetDashAnimationHash(Direction newDirection) => newDirection switch 
    { 
        Direction.Up => DASH_UP, 
        Direction.Down => DASH_DOWN, 
        Direction.Left => DASH_RIGHT, 
        Direction.Right => DASH_RIGHT,
        _ => DASH_UP
    };

    private int GetTransitionAnimationHash(Direction newDirection) => newDirection switch 
    { 
        Direction.Up => IDLE_TO_DASH_UP_TRANSITION, 
        Direction.Down => IDLE_TO_DASH_DOWN_TRANSITION, 
        Direction.Left => IDLE_TO_DASH_RIGHT_TRANSITION, 
        Direction.Right => IDLE_TO_DASH_RIGHT_TRANSITION,
        _ => IDLE_TO_DASH_UP_TRANSITION
    };
}
