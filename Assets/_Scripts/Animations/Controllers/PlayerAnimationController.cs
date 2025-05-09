using UnityEngine;

public class PlayerAnimationController : BaseAnimationController, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) 
    {
        Context = context;

        context.MovementState.OnChangeDirection += SwitchMoventAnimation;
        context.MovementState.OnHitWall += SwitchIdleAnimation;
    }

    private static readonly int IDLE_RIGHT = Animator.StringToHash("Idle_Right");
    private static readonly int IDLE_UP  = Animator.StringToHash("Idle_Up");
    private static readonly int IDLE_DOWN = Animator.StringToHash("Idle_Down");

    private static readonly int DASH_RIGHT = Animator.StringToHash("Fly_Right");
    private static readonly int DASH_UP = Animator.StringToHash("Fly_Up");
    private static readonly int DASH_DOWN = Animator.StringToHash("Fly_Down");

    private void SwitchMoventAnimation(Direction newDirection) => _switchAnimationState(
        newDirection switch 
        { 
            Direction.Up => DASH_UP, 
            Direction.Down => DASH_DOWN, 
            Direction.Left => DASH_RIGHT, 
            Direction.Right => DASH_RIGHT,
            _ => DASH_UP
        }
    );

    private void SwitchIdleAnimation(Direction fromDirection) 
    {
        _switchAnimationState(
            fromDirection switch 
            { 
                Direction.Up => IDLE_UP, 
                Direction.Down => IDLE_DOWN, 
                Direction.Left => IDLE_RIGHT, 
                Direction.Right => IDLE_RIGHT,
                _ => DASH_UP
            }
        );
    }
}
