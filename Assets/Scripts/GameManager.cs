using System;
using UnityEngine;

public enum GameState
{
    Track,
    Wall
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private int trackLength;
    [SerializeField] private int wallLength;
    [SerializeField] private GameState state;
    
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MapGenerator mapGenerator;

    public static Action<GameState> OnGameStateExit;
    public static Action<GameState> OnGameStateEnter;

    private void Start()
    {
        OnGameStateEnter?.Invoke(state);
        mapGenerator.GenerateMap(trackLength);
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (state)
        {
            case GameState.Track:
                UpdateTrackState();
                break;
            case GameState.Wall:
                UpdateWallState();
                break;
        }
    }

    public void SwitchState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Track:
                playerMovement.ExitWallPhase();
                break;
            case GameState.Wall:
                mapGenerator.GenerateMap(trackLength);
                playerTransform.position = new Vector3(-wallLength, playerTransform.position.y);
                playerMovement.EnterWallPhase();
                break;
        }
        OnGameStateExit?.Invoke(state);
        state = newState;
        OnGameStateEnter?.Invoke(state);
    }

    private void UpdateTrackState()
    {
        if(playerTransform.position.x > trackLength)
        {
            SwitchState(GameState.Wall);
        }
    }

    private void UpdateWallState()
    {
        if (playerTransform.position.x > 0)
        {
            SwitchState(GameState.Track);
        }
    }
}
