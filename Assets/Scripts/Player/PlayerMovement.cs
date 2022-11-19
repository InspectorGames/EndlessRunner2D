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

    public static event Action<PlayerVerticalMovementState> OnPlayerMovementStateEnter;
    public static event Action<PlayerVerticalMovementState> OnPlayerMovementStateExit;
    #endregion
    [SerializeField] private float maxMinecartFuel;
    [SerializeField] private float initialMinecartlFuel;
    [SerializeField] private float drainMult = 1;
    private float currentMinecartFuel;
    private bool isInWallPhase = false;
    [SerializeField] private float maxExcavatorFuel;
    private float currentExcavatorFuel;
    [SerializeField] private float minecartSpeed = 0;
    [SerializeField] private float excavatorSpeed;
    [SerializeField] private float excavatorMinSpeed;
    [SerializeField] private float excavatorMaxSpeed;
    [SerializeField] private float excavatorSpeedBoost;
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

    private void Awake()
    {
        SwitchState(FallingState);
        currentMinecartFuel = initialMinecartlFuel;
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);

        if (!isInWallPhase)
        {
            TrackHorizontalMovement();
        }
        else
        {
            WallHorizontalMovement();
        }
    }

    public void TrackHorizontalMovement()
    {
        if (currentMinecartFuel <= 0) return;
        Vector2 nextPos = Vector2.MoveTowards(transform.position, transform.position + transform.right, minecartSpeed * Time.fixedDeltaTime);

        //ConsumeMinecartFuel(Vector2.Distance(transform.position, nextPos));
        
        transform.position = nextPos;
    }

    private void ConsumeMinecartFuel(float distance)
    {
        currentMinecartFuel -= distance * drainMult;
        InGameUI.instance.SetMinecartFuelUI(currentMinecartFuel / maxMinecartFuel);
    }

    public void WallHorizontalMovement()
    {
        if(currentExcavatorFuel <= 0 && currentMinecartFuel <= 0) return;
        Vector2 nextPos = Vector2.MoveTowards(transform.position, transform.position + transform.right, excavatorSpeed * Time.fixedDeltaTime);

        if(currentExcavatorFuel > 0)
        {
            //currentExcavatorFuel -= Time.fixedDeltaTime * 10;
            InGameUI.instance.SetExcavatorFuelUI(currentExcavatorFuel / maxExcavatorFuel);
        }
        else
        {
            //currentMinecartFuel -= Time.fixedDeltaTime * 20;
            InGameUI.instance.SetMinecartFuelUI(currentMinecartFuel / maxMinecartFuel);
        }


        transform.position = nextPos;
        SpeedSlow();
    }

    public void EnterWallPhase()
    {
        isInWallPhase = true;
    }

    public void ExitWallPhase()
    {
        isInWallPhase = false;
        currentExcavatorFuel = 0;
        InGameUI.instance.SetExcavatorFuelUI(currentExcavatorFuel / maxExcavatorFuel);
    }

    public void AddExcavatorFuel()
    {
        if (currentExcavatorFuel >= maxExcavatorFuel) return;

        currentExcavatorFuel += 25;
        InGameUI.instance.SetExcavatorFuelUI(currentExcavatorFuel / maxExcavatorFuel);
    }
    public void AddMinecartFuel()
    {
        currentMinecartFuel += GameSettings.instance.fuelCanValue + GameSettings.instance.fuelcanExtra;
        if (currentMinecartFuel > maxMinecartFuel) currentMinecartFuel = maxMinecartFuel;
    }

    public void RemoveMinecartFuel()
    {
        Debug.Log("Hit Obstacle!");
        currentMinecartFuel -= GameSettings.instance.fuelCanValue;
        InGameUI.instance.SetMinecartFuelUI(currentMinecartFuel / maxMinecartFuel);
        if (currentMinecartFuel < 0) currentMinecartFuel = 0;
    }

    public void SpeedBoost()
    {
        if (excavatorSpeed >= excavatorMaxSpeed) return;
        excavatorSpeed += excavatorSpeedBoost;
    }

    private void SpeedSlow()
    {
        if (excavatorSpeed < excavatorMinSpeed) return;
        excavatorSpeed -= Time.deltaTime;
    }

    public void StartJump()
    {
        SwitchState(JumpingState);
    }

    public void StopJump()
    {
        SwitchState(FallingState);
    }

    public void FastFall()
    {
        SwitchState(FastFallingState);
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

    private void OnEnable()
    {
        Fuel.TakeMinecFuel += AddMinecartFuel;
        Fuel.TakeExcFuel += AddExcavatorFuel;

        //PlayerCollisions.HitObstacle += RemoveMinecartFuel;
    }

    private void OnDisable()
    {
        Fuel.TakeMinecFuel -= AddMinecartFuel;
        Fuel.TakeExcFuel -= AddExcavatorFuel;

        //PlayerCollisions.HitObstacle -= RemoveMinecartFuel;
    }
}


public enum PlayerVerticalMovementState
{
    InGround,
    Jumping,
    Falling,
    FastFalling
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
