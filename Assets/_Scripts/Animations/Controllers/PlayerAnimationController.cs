using UnityEngine;

public class PlayerAnimationController : BaseAnimationController
{
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int DASH_RIGHT = Animator.StringToHash("Dash_Right");

    public void Idle() => _switchAnimationState(IDLE);
    public void DashRight() => _switchAnimationState(DASH_RIGHT);


}
