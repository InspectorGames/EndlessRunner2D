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
    
    [Header("Jump Stats")]
    
    [SerializeField] private float maxYJump;
    public float MaxYJump { get { return maxYJump; } }
    [SerializeField] private float jumpingGravity;
    public float JumpingGravity { get { return jumpingGravity; } }
    [SerializeField] private float jumpingForce;
    public float JumpingForce { get { return jumpingForce; } }
    [SerializeField] private float maxFallingVel;
    public float MaxFallingVel { get { return maxFallingVel; } }
    [SerializeField] private float fallingGravity;
    public float FallingGravity { get { return fallingGravity; } }
    [SerializeField] private float yGround;
    public float YGround { get { return yGround; } }
    private float verticalVelocity;

    [Space]

    [Header("Minecart Stats")]

    [SerializeField] private float maxMinecartFuel;
    [SerializeField] private float initialMinecartSpeed;
    [SerializeField] private float initialMinecartFuel;
    private float currentMinecartFuel;
    [SerializeField] private float minecartDeceleration;
    private bool stopConsumingMFuel = false;
    
    [Space]
    
    [Header("Excavator Stats")]

    [SerializeField] private int maxExcavatorFuelCans;
    private int currentExcavatorFuelCans;
    private bool hasExtraExcavatorFuel = false;
    private float currentExcavatorFuel;
    [SerializeField] private float initialExcavatorSpeed;
    [SerializeField] private float excavatorMinSpeed;
    [SerializeField] private float excavatorMaxSpeed;
    [SerializeField] private float excavatorRequiredSpeed;
    [SerializeField] private float excavatorSpeedBoost;
    [SerializeField] private float excavatorDeceleration;


    [Min(0)] private float currentSpeed;

    private bool isInWallPhase = false;
    private bool canMove = false;
    private float wallMult = 100;
    private void Awake()
    {
        SwitchState(FallingState);
        currentMinecartFuel = initialMinecartFuel;
        currentSpeed = initialMinecartSpeed;
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
        CheckGameOver();

        if (!canMove) return;
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
        Vector2 nextPos = Vector2.MoveTowards(transform.position, transform.position + transform.right, currentSpeed * Time.fixedDeltaTime);


        if(currentMinecartFuel > 0)
        {
            if(currentSpeed < initialMinecartSpeed) currentSpeed = initialMinecartSpeed;
            if (!stopConsumingMFuel)
            {
                ConsumeMinecartFuel(Vector2.Distance(transform.position, nextPos));
            }
        }
        transform.position = nextPos;
        SpeedSlow();
    }

    private void ConsumeMinecartFuel(float distance)
    {
        currentMinecartFuel -= distance;
    }

    public void WallHorizontalMovement()
    {
        Vector2 nextPos = Vector2.MoveTowards(transform.position, transform.position + transform.right, currentSpeed * Time.fixedDeltaTime);

        if(currentExcavatorFuel > 0)
        {
            currentExcavatorFuel -= Time.fixedDeltaTime * excavatorRequiredSpeed * wallMult;
        }
        else
        {
            currentMinecartFuel -= Time.fixedDeltaTime * excavatorRequiredSpeed * 2 * wallMult;
        }


        transform.position = nextPos;
        SpeedSlow();
    }

    public void EnterWallPhase()
    {
        currentSpeed = initialExcavatorSpeed;
        isInWallPhase = true;
        StopConsumingMFuel(false);
        CalculateExcavatorFuel();
    }

    public void ExitWallPhase()
    {
        currentSpeed = initialMinecartSpeed;
        isInWallPhase = false;
        currentExcavatorFuel = 0;
        StopConsumingMFuel(true);
    }
    #region Fuel
    private void CalculateExcavatorFuel()
    {
        float canValue = 100 / maxExcavatorFuelCans;
        currentExcavatorFuel = canValue * currentExcavatorFuelCans;
        currentExcavatorFuelCans = 0;
    }

    public void AddExcavatorFuel()
    {
        if (currentExcavatorFuelCans >= maxExcavatorFuelCans && !hasExtraExcavatorFuel)
        {
            hasExtraExcavatorFuel = true;
        }
        else
        {
            currentExcavatorFuelCans += 1;
        }
    }

    public void AddMinecartFuel()
    {
        currentMinecartFuel += GameSettings.instance.fuelCanValue;
        if (currentMinecartFuel > maxMinecartFuel) currentMinecartFuel = maxMinecartFuel;
    }

    public void AddMinecartFuel(float value)
    {
        currentMinecartFuel += value;
        if (currentMinecartFuel > maxMinecartFuel) currentMinecartFuel = maxMinecartFuel;
    }

    public void RemoveMinecartFuel()
    {
        currentMinecartFuel -= GameSettings.instance.fuelLossHit;
        if (currentMinecartFuel < 0) currentMinecartFuel = 0;
    }
    #endregion

    #region Speed
    public void SpeedBoost()
    {
        if (currentExcavatorFuel <= 0 && currentMinecartFuel <= 0 || currentSpeed >= excavatorMaxSpeed) return;
        currentSpeed += excavatorSpeedBoost;
    }

    private void SpeedSlow()
    {
        if(currentSpeed < 0)
        {
            currentSpeed = 0;
        }

        if (!isInWallPhase) // Track Phase
        {
            if(currentMinecartFuel <= 0 && currentSpeed > 0)
            {
                currentSpeed -= Time.fixedDeltaTime * minecartDeceleration;
            }
        }
        else //Wall Phase
        {
            if(currentExcavatorFuel <= 0 && currentMinecartFuel <= 0)
            {
                currentSpeed -= Time.fixedDeltaTime * excavatorDeceleration;
            }
            else if(currentSpeed > excavatorMinSpeed)
            {
                currentSpeed -= Time.fixedDeltaTime;
            }
        }
    }
    #endregion


    public void ResetPlayerMovement()
    {
        //Reset all variables to their initial values
        isInWallPhase = false;
        stopConsumingMFuel = false;
        hasExtraExcavatorFuel = false;
        canMove = false;

        transform.position = Vector3.up;

        currentMinecartFuel = initialMinecartFuel;
        currentSpeed = initialMinecartSpeed;
        currentExcavatorFuel = 0;
        currentExcavatorFuelCans = 0;
        
        SwitchState(FallingState);
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

    public void ConsumeExcavatorFuel()
    {
        if (hasExtraExcavatorFuel)
        {
            hasExtraExcavatorFuel = false;
            AddMinecartFuel(50);
            EventManager.OnExcavatorFuelConsumed();
        }
        else if(currentExcavatorFuelCans > 0)
        {
            currentExcavatorFuelCans--;
            AddMinecartFuel(50);
            EventManager.OnExcavatorFuelConsumed();
        }
    }

    public void StopConsumingMFuel(bool consume)
    {
        stopConsumingMFuel = consume;
    }

    public float GetVerticalVelocity()
    {
        return verticalVelocity;
    }

    public void SetVerticalVelocity(float velocity)
    {
        verticalVelocity = velocity;
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
    
    public void CheckGameOver()
    {
        if(currentSpeed <= 0 && !hasExtraExcavatorFuel && currentExcavatorFuelCans <= 0)
        {
            EventManager.OnGameOver();
        }
    }

    public void CalculateWallMultiplier(float wallLength)
    {
        wallMult = 100 / wallLength;
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public float GetCurrentMinecartFuel()
    {
        return currentMinecartFuel;
    }

    public float GetCurrentExcavatorFuel()
    {
        return currentExcavatorFuel;
    }

    public int GetCurrentExcavatorFuelCans()
    {
        return currentExcavatorFuelCans;
    }

    public int GetMaxExcavatorFuelCans()
    {
        return maxExcavatorFuelCans;
    }

    public bool HasExtraFuelCan()
    {
        return hasExtraExcavatorFuel;
    }

    private void OnEnable()
    {
        EventManager.Wall.Enqueue(0, EnterWallPhase);
        EventManager.EnterTrack.Enqueue(0, ExitWallPhase);

        EventManager.MinecartFuelCollected += AddMinecartFuel;
        //EventManager.ExcavatorFuelCollected += AddExcavatorFuel;
        EventManager.ExcavatorFuelCollected.Enqueue(0, AddExcavatorFuel);
        EventManager.ObstacleHitted += RemoveMinecartFuel;
    }

    private void OnDisable()
    {
        EventManager.Wall.Dequeue(0);
        EventManager.EnterTrack.Dequeue(0);

        EventManager.MinecartFuelCollected -= AddMinecartFuel;
        //EventManager.ExcavatorFuelCollected -= AddExcavatorFuel;
        EventManager.ExcavatorFuelCollected.Dequeue(0);
        EventManager.ObstacleHitted -= RemoveMinecartFuel;
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
    private float jumpingVel = 0;
    public override void EnterState(PlayerMovement playerMovement)
    {
        playerMovement.state = PlayerVerticalMovementState.Jumping;
        jumpingVel = playerMovement.JumpingForce;
        base.EnterState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        playerMovement.SetVerticalVelocity(jumpingVel);
        jumpingVel = 0;
        base.ExitState(playerMovement);
    }

    public override void FixedUpdateState(PlayerMovement playerMovement)
    {
        Vector2 playerPosition = playerMovement.transform.position;

        if(jumpingVel > 0)
        {
            jumpingVel -= playerMovement.JumpingGravity * Time.fixedDeltaTime;
        }
        else
        {
            jumpingVel = 0;
            playerMovement.SwitchState(playerMovement.FallingState);
        }

        playerMovement.transform.position = Vector2.MoveTowards(playerPosition, new Vector2(playerPosition.x, playerMovement.MaxYJump), jumpingVel * Time.fixedDeltaTime);

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
        fallingVel = - playerMovement.GetVerticalVelocity();
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

        if(fallingVel < playerMovement.MaxFallingVel && fallingVel >= 0)
        {
            fallingVel += playerMovement.FallingGravity * Time.fixedDeltaTime;
        }
        else if(fallingVel < 0)
        {
            fallingVel += 4 * playerMovement.FallingGravity * Time.fixedDeltaTime;
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
