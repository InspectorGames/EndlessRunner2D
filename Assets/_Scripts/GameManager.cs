using System;
using UnityEngine;
using Doozy.Runtime.Signals;

public enum GameState
{
    NoState,
    InMenu,
    Starting,
    EnterTrack,
    Track,
    EnterWall,
    Wall,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private GameState currentState;

    public static event Action<GameState> OnGameStateExit;
    public static event Action<GameState> OnGameStateEnter;

    public static GameManager Instance { get { return instance; } }
    public GameState CurrentState { get { return currentState; } }

    [SerializeField] private SignalSender gameOverSignal;

    [Header("Controllers")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerExcavator playerExcavator;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private BackgroundController backgroundController;
    [SerializeField] private CinemachineManager cinemachineManager;
    [SerializeField] private ParallaxEffect parallaxEffect;


    [Space]

    [Header("Starting Game Settings")]
    [SerializeField] private int enterGameLength;

    [Space]

    [Header("Track/Wall Settings")]
    [SerializeField] private int trackLength;
    [SerializeField] private int enterWallLength;
    [SerializeField] private int wallLength;
    [SerializeField] private int enterTrackLength;

    private GameRecord gameRecord = new GameRecord(0);
    public GameRecord CurrentRecord { get { return gameRecord; } }
    
    private int trackEnd;
    private int enterWallEnd;
    private int wallEnd;
    private int enterTrackEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        EventManager.GameOver += GameOver;
    }

    private void OnDisable()
    {
        EventManager.GameOver -= GameOver;
    }

    private void Start()
    {
        ChangeState(GameState.InMenu);
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Starting:
                UpdateStarting();
                break;
            case GameState.EnterTrack:
                UpdateEnterTrack();
                break;
            case GameState.Track:
                UpdateTrack();
                break;
            case GameState.EnterWall:
                UpdateEnterWall();
                break;
            case GameState.Wall:
                UpdateWall();
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;
        OnGameStateExit?.Invoke(currentState);
        currentState = newState;
        switch (newState)
        {
            case GameState.InMenu:
                HandleInMenu();
                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.EnterTrack:
                HandleEnterTrack();
                break;
            case GameState.Track:
                HandleTrack();
                break;
            case GameState.EnterWall:
                HandleEnterWall();
                break;
            case GameState.Wall:
                HandleWall();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
        }
        OnGameStateEnter?.Invoke(newState);
    }

    public void RestartGame()
    {
        ResetGame();
        ChangeState(GameState.Starting);
    }

    public void StartGame()
    {
        ChangeState(GameState.Starting);
    }

    public void BackToMainMenu()
    {
        ChangeState(GameState.InMenu);
    }

    private void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    #region Handle Methods
    private void HandleInMenu()
    {
        GameRecordData data = SaveSystem.LoadGameRecord();
        if (data != null && data.GetMaxScore() > gameRecord.GetMaxScore())
        {
            gameRecord.SetMaxScore(data.GetMaxScore());
        }

        ResetGame();
        cinemachineManager.SwitchToMenuCamera();
        AudioManager.instance.PlayMusic("menutlvz");

        EventManager.OnMainMenuLoaded();
    }
    private void HandleStarting() //Invoked from the main menu
    {
        //Initial Configurations
        mapGenerator.GenerateMap(enterGameLength, trackLength, false);
        //backgroundController.NextBackground(enterGameLength, enterWallEnd + enterTrackLength);

        //Change MenuCamera to GameCamera
        cinemachineManager.SwitchToTrackCamera();

        //Calculate right fuel consumption for the lenght of the wall
        playerMovement.CalculateWallMultiplier(wallLength);
        //Enable movement
        playerMovement.SetCanMove(true);
        //Stop consuming fuel for the initial animation
        playerMovement.StopConsumingMFuel(true);

        AudioManager.instance.StopMusic();

        EventManager.OnGameStarted();
    }
    private void HandleEnterTrack()
    {
        cinemachineManager.SwitchToTrackCamera();

        gameRecord.IncrementCurrentScore();

        //Remove input from player
        BindingManager.Instance.PlayerInput(false);

        EventManager.OnEnterTrack();
    }
    private void HandleTrack()
    {
        //Give back input to the player
        BindingManager.Instance.PlayerInput(true);

        //Start consuming minecart fuel
        playerMovement.StopConsumingMFuel(false);
        
        //Calculate new end positions
        CalculateEndPositions();

        mapGenerator.DrawWall(enterWallEnd, wallLength);
    }
    private void HandleEnterWall() 
    {
        cinemachineManager.SwitchToWallCamera();

        //Remove input from player
        BindingManager.Instance.PlayerInput(false);

        //Stop consuming minecart fuel
        playerMovement.StopConsumingMFuel(true);

        EventManager.OnEnterWall();
    }
    private void HandleWall()
    {
        //Give back player input
        BindingManager.Instance.PlayerInput(true);

        //Generate new set of MapObjects
        mapGenerator.GenerateMap(enterTrackEnd, trackLength, true);
        backgroundController.NextBackground(wallEnd, enterWallEnd);

        EventManager.OnWall();
    }
    private void HandleGameOver()
    {
        PauseGame(true);
        gameOverSignal.SendSignal();
    }

    #endregion

    #region Update Methods
    private void UpdateStarting()
    {
        //Check if player has traveled all "enterGameLength"
        if (playerMovement.transform.position.x >= enterGameLength)
        {
            ChangeState(GameState.Track);
        }

    }


    private void UpdateEnterTrack()
    {
        //Check if player has traveled all "enterTrackLength"
        if (playerMovement.transform.position.x >= enterTrackEnd)
        {
            ChangeState(GameState.Track);
        }

    }


    private void UpdateTrack()
    {
        //Check if player has traveled all "trackLength"
        if (playerMovement.transform.position.x >= trackEnd)
        {
            ChangeState(GameState.EnterWall);
        }

    }


    private void UpdateEnterWall()
    {
        //Check if player has traveled all "enterWallLength"
        if(playerMovement.transform.position.x >= enterWallEnd)
        {
            ChangeState(GameState.Wall);
        }

    }


    private void UpdateWall()
    {
        //Check if player has traveled all "wallLength"
        if (playerMovement.transform.position.x >= wallEnd)
        {
            ChangeState(GameState.EnterTrack);
        }

    }

    #endregion

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void ResetGame()
    {
        gameRecord.UpdateMaxScore();
        gameRecord.ResetCurrentScore();

        //Reset background parallaxelements
        parallaxEffect.ResetElements();

        //Reset player movement values
        playerMovement.ResetPlayerMovement();

        //Reset map generation parameters
        ResetEndPositions();
        mapGenerator.ClearAllMapObjects();

        EventManager.OnGameRestarted();
    }

    private void ResetEndPositions()
    {
        trackEnd = 0;
        enterWallEnd = 0;
        wallEnd = 0;
        enterTrackEnd = 0;
    }

    private void CalculateEndPositions()
    {
        trackEnd = enterGameLength + enterTrackEnd + trackLength;
        enterWallEnd = trackEnd + enterWallLength;
        wallEnd = enterWallEnd + wallLength;
        enterTrackEnd = wallEnd + enterTrackLength;
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveGameRecord(gameRecord);
    }
}

public class GameRecord
{
    private int maxScore = 0;
    private int currentScore = 0;

    public GameRecord(int maxScore)
    {
        this.maxScore = maxScore;
    }
    public void UpdateMaxScore()
    {
        if(currentScore > maxScore)
        {
            maxScore = currentScore;
        }
    }

    public void SetMaxScore(int value)
    {
        maxScore = value;
    }

    public int GetMaxScore()
    {
        return maxScore;
    }

    public void IncrementCurrentScore()
    {
        currentScore++;
        EventManager.OnWallScored();
    }

    public void ResetCurrentScore()
    {
        currentScore = 0;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

}

[Serializable]
public class GameRecordData
{
    private int maxScore;

    public GameRecordData(GameRecord gameRecord)
    {
        maxScore = gameRecord.GetMaxScore();
    }

    public int GetMaxScore()
    {
        return maxScore;
    }
}