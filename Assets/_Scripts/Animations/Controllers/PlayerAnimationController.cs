using UnityEngine;

public class PlayerAnimationController : BaseAnimationController, IConsumer<PlayerContextData>
{
    public PlayerContextData Context { get; private set; }

    public void Inject(PlayerContextData context) 
    {
        Context = context;

        context.OnDirectionChanged += SwitchMoventAnimation;
    }

    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int DASH_RIGHT = Animator.StringToHash("Fly Side Right");
    private static readonly int DASH_LEFT = Animator.StringToHash("Fly Side Right");
    private static readonly int DASH_UP = Animator.StringToHash("Fly Up");
    private static readonly int DASH_DOWN = Animator.StringToHash("Fly Down");

    public void Idle() => _switchAnimationState(IDLE);
    public void DashRight() => _switchAnimationState(DASH_RIGHT);

    private void SwitchMoventAnimation(Direction newDirection) => _switchAnimationState(
        newDirection switch 
        { 
            Direction.Up => DASH_UP, 
            Direction.Down => DASH_DOWN, 
            Direction.Left => DASH_LEFT, 
            Direction.Right => DASH_RIGHT,
            _ => IDLE
        }
    );
}
