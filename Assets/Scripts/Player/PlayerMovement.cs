using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region States
    public PlayerVerticalMovementState state;

    private PlayerMovementBaseState currentState;
    public PlayerInGroundState InGroundState = new PlayerInGroundState();
    public PlayerJumpingState JumpingState = new PlayerJumpingState();
    public PlayerFallingState FallingState = new PlayerFallingState();
    public PlayerFastFallingState FastFallingState = new PlayerFastFallingState();

    public PlayerFlyingUpState FlyingUpState = new PlayerFlyingUpState();
    public PlayerFlyingDownState FlyingDownState = new PlayerFlyingDownState();
    public PlayerFlyingSmashState FlyingSmashState = new PlayerFlyingSmashState();

    public static event Action<PlayerVerticalMovementState> OnPlayerMovementStateEnter;
    public static event Action<PlayerVerticalMovementState> OnPlayerMovementStateExit;
    #endregion
    [SerializeField] private float currentSpeed = 0;
    public float CurrentSpeed { get { return currentSpeed; } }
    [SerializeField] private float maxYJump;
    public float MaxYJump { get { return maxYJump; } }
    [SerializeField] private float jumpingForce;
    public float JumpingForce { get { return jumpingForce; } }
    [SerializeField] private float maxFallingVel;
    public float MaxFallingVel { get { return maxFallingVel; } }
    [SerializeField] private float fallingGravity;
    public float FallingGravity { get { return fallingGravity; } }
    [SerializeField] private float yGround;
    public float YGround { get { return yGround; } }
    [SerializeField] [Range(0f, 90f)] private float rampAngle;
    public float RampAngle { get { return rampAngle; } }
    [SerializeField] private float flyingGravity;
    public float FlyingGravity { get { return flyingGravity; } }

    private void Awake()
    {
        SwitchState(FallingState);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
        HorizontalMovement();
    }

    public void HorizontalMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position + transform.right, currentSpeed * Time.fixedDeltaTime);
    }

    public void StartJump()
    {
        SwitchState(JumpingState);
    }

    public void StopJump()
    {
        Debug.Log("Stop Jump");
        SwitchState(FallingState);
    }

    public void FastFall()
    {
        SwitchState(FastFallingState);
    }

    public void FlyingSmash()
    {
        SwitchState(FlyingSmashState);
    }

    public void HitFlyingThing()
    {
        currentState = FlyingUpState;
        FlyingUpState.UpImpulse(50);
    }

    #region State Methods
    public void SwitchState(PlayerMovementBaseState state)
    {
        if(currentState != null) currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public void EnterState()
    {
        OnPlayerMovementStateEnter?.Invoke(state);
    }

    public void ExitState()
    {
        OnPlayerMovementStateExit?.Invoke(state);
    }
    #endregion
}

public enum PlayerVerticalMovementState
{
    InGround,
    Jumping,
    Falling,
    FastFalling,
    FlyingUp,
    FlyingDown,
    FlyingSmash
}

public abstract class PlayerMovementBaseState
{
    public virtual void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.EnterState();
    }
    public virtual void ExitState(PlayerMovement playerMovement)
    {
        playerMovement.ExitState();
    }
    public abstract void UpdateState(PlayerMovement playerMovement);
    public abstract void FixedUpdateState(PlayerMovement playerMovement);
}

#region Ground

public class PlayerInGroundState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.state = PlayerVerticalMovementState.InGround;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {

    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }
}

public class PlayerJumpingState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.state = PlayerVerticalMovementState.Jumping;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector2 playerPosition = playerMovement.transform.position;
        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerMovement.MaxYJump), playerMovement.JumpingForce * Time.fixedDeltaTime);

        if (playerMovement.transform.position.y >= playerMovement.MaxYJump) playerMovement.SwitchState(playerMovement.FallingState);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }
}

public class PlayerFallingState : PlayerMovementBaseState
{
    private float fallingVel = 0;
    public override void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.state = PlayerVerticalMovementState.Falling;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        fallingVel = 0;
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector2 playerPosition = playerMovement.transform.position;

        if(fallingVel < playerMovement.MaxFallingVel)
        {
            fallingVel += playerMovement.FallingGravity * Time.fixedDeltaTime;
        }

        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerMovement.YGround), fallingVel * Time.fixedDeltaTime);

        if (playerMovement.transform.position.y <= playerMovement.YGround) playerMovement.SwitchState(playerMovement.InGroundState);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }
}

public class PlayerFastFallingState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.state = PlayerVerticalMovementState.FastFalling;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector2 playerPosition = playerMovement.transform.position;

        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerMovement.YGround), playerMovement.MaxFallingVel * Time.fixedDeltaTime);

        if (playerMovement.transform.position.y <= playerMovement.YGround) playerMovement.SwitchState(playerMovement.InGroundState);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }
}

#endregion

#region Flying

public class PlayerFlyingUpState : PlayerMovementBaseState
{
    private float verticalSpeed;
    public override void EnterState(PlayerMovement playerMovement)
    {
        verticalSpeed = playerMovement.CurrentSpeed * Mathf.Sin(Mathf.Deg2Rad * playerMovement.RampAngle);
        playerMovement.state = PlayerVerticalMovementState.FlyingUp;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector3 playerPosition = playerMovement.transform.position;

        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerPosition.y + 1), verticalSpeed * Time.fixedDeltaTime);

        verticalSpeed -= playerMovement.FlyingGravity * Time.fixedDeltaTime;
        if (verticalSpeed < 0) playerMovement.SwitchState(playerMovement.FlyingDownState);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }

    public void UpImpulse(float impulse)
    {
        verticalSpeed += impulse;
    }
}

public class PlayerFlyingDownState : PlayerMovementBaseState
{
    private float verticalSpeed;
    public override void EnterState(PlayerMovement playerMovement)
    {
        verticalSpeed = 0;
        playerMovement.state = PlayerVerticalMovementState.FlyingDown;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector3 playerPosition = playerMovement.transform.position;

        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerPosition.y + 1), verticalSpeed * Time.fixedDeltaTime);

        verticalSpeed -= playerMovement.FlyingGravity * Time.fixedDeltaTime;
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }
}

public class PlayerFlyingSmashState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.state = PlayerVerticalMovementState.FlyingSmash;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector3 playerPosition = playerMovement.transform.position;

        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerPosition.y + 1), playerMovement.MaxFallingVel * Time.fixedDeltaTime);

    }

    public override void UpdateState(PlayerMovement playerMovement)
    {

    }
}

#endregion