using System;
using System.Collections.Generic;
using UnityEngine;

public class BindingManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private Action TrackActionStarted;
    private Action TrackActionCanceled;

    private Action WallActionStarted;
    private Action WallActionCanceled;
    

    private void Awake()
    {
        PlayerMovement.OnPlayerMovementStateEnter += OnPlayerMovementStateEnter;
        PlayerMovement.OnPlayerMovementStateExit += OnPlayerMovementStateExit;

        GameManager.OnGameStateEnter += OnGameStateEnter;
        GameManager.OnGameStateExit += OnGameStateExit;
    }

    private void TActionStarted()
    {
        TrackActionStarted?.Invoke();
    }

    private void TActionCanceled()
    {
        TrackActionCanceled?.Invoke();
    }

    private void WActionStarted()
    {
        WallActionStarted?.Invoke();
    }
    
    private void WActionCanceled()
    {
        WallActionCanceled?.Invoke();
    }

    private void OnGameStateEnter(GameState newState)
    {
        switch (newState)
        {
            case GameState.Track:
                InputManager.OnActionStarted += TActionStarted;
                InputManager.OnActionCanceled += TActionCanceled;
                break;
            case GameState.Wall:
                InputManager.OnActionStarted += WActionStarted;
                InputManager.OnActionCanceled += WActionCanceled;

                WallActionStarted += playerMovement.SpeedBoost;
                break;
        }
    }

    private void OnGameStateExit(GameState newState)
    {
        switch (newState)
        {
            case GameState.Track:
                InputManager.OnActionStarted -= TActionStarted;
                InputManager.OnActionCanceled -= TActionCanceled;
                break;
            case GameState.Wall:
                InputManager.OnActionStarted -= WActionStarted;
                InputManager.OnActionCanceled -= WActionCanceled;

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