using System;
using System.Collections.Generic;
using UnityEngine;

public class BindingManager : MonoBehaviour
{
    private static BindingManager instance;
    public static BindingManager Instance { get { return instance; } }

    [SerializeField] private PlayerMovement playerMovement;

    private Action TrackActionStarted;
    private Action TrackActionCanceled;

    private Action WallActionStarted;
    private Action WallActionCanceled;

    private bool playerInput = true;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    private void OnEnable()
    {
        PlayerMovement.OnPlayerMovementStateEnter += OnPlayerMovementStateEnter;
        PlayerMovement.OnPlayerMovementStateExit += OnPlayerMovementStateExit;

        GameManager.OnGameStateEnter += OnGameStateEnter;
        GameManager.OnGameStateExit += OnGameStateExit;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerMovementStateEnter -= OnPlayerMovementStateEnter;
        PlayerMovement.OnPlayerMovementStateExit -= OnPlayerMovementStateExit;

        GameManager.OnGameStateEnter -= OnGameStateEnter;
        GameManager.OnGameStateExit -= OnGameStateExit;
    }

    public void PlayerInput(bool playerInput)
    {
        this.playerInput = playerInput;
    }

    private void TActionStarted()
    {
        if (!playerInput) return;
        TrackActionStarted?.Invoke();
    }

    private void TActionCanceled()
    {
        if (!playerInput) return;
        TrackActionCanceled?.Invoke();
    }

    private void WActionStarted()
    {
        if (!playerInput) return;
        WallActionStarted?.Invoke();
    }
    
    private void WActionCanceled()
    {
        if (!playerInput) return;
        WallActionCanceled?.Invoke();
    }

    private void OnGameStateEnter(GameState newState)
    {
        switch (newState)
        {
            case GameState.Track:
                InputManager.OnJumpStarted += TActionStarted;
                InputManager.OnJumpCanceled += TActionCanceled;

                InputManager.OnConsumeFuelStarted += playerMovement.ConsumeExcavatorFuel;
                break;
            case GameState.Wall:
                InputManager.OnJumpStarted += WActionStarted;
                InputManager.OnJumpCanceled += WActionCanceled;

                WallActionStarted += playerMovement.SpeedBoost;
                break;
        }
    }

    private void OnGameStateExit(GameState newState)
    {
        switch (newState)
        {
            case GameState.Track:
                InputManager.OnJumpStarted -= TActionStarted;
                InputManager.OnJumpCanceled -= TActionCanceled;

                InputManager.OnConsumeFuelStarted -= playerMovement.ConsumeExcavatorFuel;
                break;
            case GameState.Wall:
                InputManager.OnJumpStarted -= WActionStarted;
                InputManager.OnJumpCanceled -= WActionCanceled;

                WallActionStarted -= playerMovement.SpeedBoost;
                break;
        }
    }

    private void OnPlayerMovementStateEnter(PlayerVerticalMovementState newState)
    {
        switch (newState)
        {
            case PlayerVerticalMovementState.InGround:
                TrackActionStarted += playerMovement.StartJump;
                break;
            case PlayerVerticalMovementState.Jumping:
                TrackActionCanceled += playerMovement.StopJump;
                break;
            case PlayerVerticalMovementState.Falling:
                TrackActionStarted += playerMovement.FastFall;
                break;
        }
    }

    private void OnPlayerMovementStateExit(PlayerVerticalMovementState newState)
    {
        switch (newState)
        {
            case PlayerVerticalMovementState.InGround:
                TrackActionStarted -= playerMovement.StartJump;
                break;
            case PlayerVerticalMovementState.Jumping:
                TrackActionCanceled -= playerMovement.StopJump;
                break;
            case PlayerVerticalMovementState.Falling:
                TrackActionStarted -= playerMovement.FastFall;
                break;
        }
    }
}