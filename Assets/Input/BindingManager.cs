using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindingManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private void Awake()
    {
        PlayerMovement.OnPlayerMovementStateEnter += OnPlayerMovementStateEnter;
        PlayerMovement.OnPlayerMovementStateExit += OnPlayerMovementStateExit;
    }
    private void OnPlayerMovementStateEnter(PlayerVerticalMovementState newState)
    {
        switch (newState)
        {
            case PlayerVerticalMovementState.InGround:
                InputManager.OnActionStarted += playerMovement.StartJump;
                break;
            case PlayerVerticalMovementState.Jumping:
                InputManager.OnActionCanceled += playerMovement.StopJump;
                break;
            case PlayerVerticalMovementState.Falling:
                InputManager.OnActionStarted += playerMovement.FastFall;
                break;
            case PlayerVerticalMovementState.FlyingUp:
                InputManager.OnActionStarted += playerMovement.FlyingSmash;
                break;
            case PlayerVerticalMovementState.FlyingDown:
                InputManager.OnActionStarted += playerMovement.FlyingSmash;
                break;
        }
    }

    private void OnPlayerMovementStateExit(PlayerVerticalMovementState newState)
    {
        switch (newState)
        {
            case PlayerVerticalMovementState.InGround:
                InputManager.OnActionStarted -= playerMovement.StartJump;
                break;
            case PlayerVerticalMovementState.Jumping:
                InputManager.OnActionCanceled -= playerMovement.StopJump;
                break;
            case PlayerVerticalMovementState.Falling:
                InputManager.OnActionStarted -= playerMovement.FastFall;
                break;
            case PlayerVerticalMovementState.FlyingUp:
                InputManager.OnActionStarted -= playerMovement.FlyingSmash;
                break;
            case PlayerVerticalMovementState.FlyingDown:
                InputManager.OnActionStarted -= playerMovement.FlyingSmash;
                break;
        }
    }
}