using UnityEngine;

public class PlayerAnimationController : BaseAnimationController, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) 
    {
        Context = context;

        context.MovementState.OnIdleChange += (bool isIdle) => 
        {
            if (isIdle) _switchAnimationState(GetIdleAnimationState(Context.MovementState.CurrentDirection));
            else _switchAnimationState(GetMovementAnimationState(Context.MovementState.CurrentDirection));
        };

        context.OnEnterAttackMode += () => GetSwitchAttackAnimationState(Context.MovementState.CurrentDirection);
        context.OnExitAttackMode += () =>
        {
            if(!Context.MovementState.IsIdle)
            {
                GetSwitchAttackAnimationState(Context.MovementState.CurrentDirection);
            }
        };
    }

    private static readonly int IDLE_RIGHT = Animator.StringToHash("Idle_Right");
    private static readonly int IDLE_UP  = Animator.StringToHash("Idle_Up");
    private static readonly int IDLE_DOWN = Animator.StringToHash("Idle_Down");

    private static readonly int DASH_RIGHT = Animator.StringToHash("Fly_Right");
    private static readonly int DASH_UP = Animator.StringToHash("Fly_Up");
    private static readonly int DASH_DOWN = Animator.StringToHash("Fly_Down");

    private static readonly int ATTACK_RIGHT = Animator.StringToHash("Attack_Side");
    private static readonly int ATTACK_UP = Animator.StringToHash("Attack_Up");
    private static readonly int ATTACK_DOWN = Animator.StringToHash("Attack_Down");

    private int GetMovementAnimationState(Direction newDirection) => newDirection switch 
        { 
            Direction.Up => DASH_UP, 
            Direction.Down => DASH_DOWN, 
            Direction.Left => DASH_RIGHT, 
            Direction.Right => DASH_RIGHT,
            _ => DASH_UP
        };

    private int GetIdleAnimationState(Direction fromDirection) => fromDirection switch 
        { 
            Direction.Up => IDLE_UP, 
            Direction.Down => IDLE_DOWN, 
            Direction.Left => IDLE_RIGHT, 
            Direction.Right => IDLE_RIGHT,
            _ => DASH_UP
        };

    private int GetSwitchAttackAnimationState(Direction fromDirection) => fromDirection switch 
        { 
            Direction.Up => ATTACK_UP, 
            Direction.Down => ATTACK_DOWN, 
            Direction.Left => ATTACK_RIGHT, 
            Direction.Right => ATTACK_RIGHT,
            _ => ATTACK_UP
        };
}
