using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpiingState : PlayerBaseState
{
    private readonly int JumpHash = Animator.StringToHash("Jump");

    private Vector3 momentum;

    private const float CrossFadeDuration = 0.1f;

    public PlayerJumpiingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);

        momentum = stateMachine.Controller.velocity;
        momentum.y = 0f;

        stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);

        stateMachine.LedgeDetector.OnLedgeDetect += HandleLedgeDetected;
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);

        if (stateMachine.Controller.velocity.y <= 0)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
            return;
        }

        FaceTarget();
    }

    public override void Exit()
    {
        stateMachine.LedgeDetector.OnLedgeDetect -= HandleLedgeDetected;
    }

    private void HandleLedgeDetected(Vector3 ledgeForward)
    {
        stateMachine.SwitchState(new PlayerHanginState(stateMachine, ledgeForward));
    }
}
