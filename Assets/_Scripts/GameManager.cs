using Doozy.Runtime.Signals;
using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    NoState,
    InMenu,
    MainMenuAnimation,
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
    [SerializeField] private SignalSender playSignal;
    [SerializeField] private SignalSender backToMainMenuAnimEndedSignal;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private Animator circlePlayerAnimator;
    [SerializeField] private Animator circleCameraAnimator;
    [SerializeField] private GameObject mainMenu1Background;
    [SerializeField] private GameObject mainMenu2Background;

    [Header("Controllers")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerExcavator playerExcavator;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private BackgroundController backgroundController;
    [SerializeField] private CinemachineManager cinemachineManager;
    [SerializeField] private ParallaxEffect parallaxEffect;


    [Space]

    [Header("Starting Game Settings")]
    [SerializeField] private int mainMenuLength;
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
        EventManager.ObstacleHitted += cinemachineManager.ScreenShakeHit;
        EventManager.PlayerLand += cinemachineManager.ScreenShakeLand;
        EventManager.PlayerLandHard += cinemachineManager.ScreenShakeLandHard;
    }

    private void OnDisable()
    {
        EventManager.GameOver -= GameOver;
        EventManager.ObstacleHitted -= cinemachineManager.ScreenShakeHit;
        EventManager.PlayerLand -= cinemachineManager.ScreenShakeLand;
        EventManager.PlayerLandHard -= cinemachineManager.ScreenShakeLandHard;
    }

    private void Start()
    {
        ChangeState(GameState.InMenu);
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.MainMenuAnimation:
                UpdateMainMenuAnimation();
                break;
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
        if (currentState == newState && newState != GameState.Starting) return;
        OnGameStateExit?.Invoke(currentState);
        currentState = newState;
        switch (newState)
        {
            case GameState.InMenu:
                HandleInMenu();
                break;
            case GameState.MainMenuAnimation:
                HandleMainMenuAnimation();
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
        blackScreen.SetActive(true);
        circlePlayerAnimator.gameObject.SetActive(false);
        circleCameraAnimator.gameObject.SetActive(true);
        circleCameraAnimator.Play("PlayerCircleClose");
        AudioManager.instance.StopMusic();
        AudioManager.instance.PlayMusicLoop("ostA", "ostB");
        EventManager.OnGameRestarting();
        StartCoroutine(WaitRestartAnimation());
    }
    
    private IEnumerator WaitRestartAnimation()
    {
        Debug.Log("Restart Animation In Progress...");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        Debug.Log("Restart Animation Ended...");
        Time.timeScale = 1;
        ResetGame(false);
        ChangeState(GameState.Starting);
    }

    public void MenuAnimation()
    {
        ChangeState(GameState.MainMenuAnimation);
    }

    public void BackToMainMenu()
    {
        blackScreen.SetActive(true);
        circlePlayerAnimator.gameObject.SetActive(false);
        circleCameraAnimator.gameObject.SetActive(true);
        circleCameraAnimator.Play("PlayerCircleClose");
        StartCoroutine(BackToMainMenuAnimation());
    }

    private IEnumerator BackToMainMenuAnimation()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        backToMainMenuAnimEndedSignal.SendSignal();
        ResetGame(false);
        ChangeState(GameState.InMenu);
    }

    private void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    #region Handle Methods
    private void HandleInMenu()
    {
        mainMenu1Background.SetActive(true);
        mainMenu2Background.SetActive(true);

        circlePlayerAnimator.gameObject.SetActive(true);
        circleCameraAnimator.gameObject.SetActive(false);

        circlePlayerAnimator.Play("PlayerCircleOpen");

        GameRecordData data = SaveSystem.LoadGameRecord();
        if (data != null && data.GetMaxScore() > gameRecord.GetMaxScore())
        {
            gameRecord.SetMaxScore(data.GetMaxScore());
        }
        playerMovement.EnablePlayerMovementSFX(false);

        ResetGame(true);
        cinemachineManager.SwitchToMenuCamera();
        AudioManager.instance.PlayMusic("menuv2");

        EventManager.OnMainMenuLoaded();
    }

    private void HandleMainMenuAnimation()
    {
        AudioManager.instance.StopMusic();
        AudioManager.instance.PlayMusicLoop("ostA","ostB");

        blackScreen.SetActive(true);
        circlePlayerAnimator.gameObject.SetActive(true);
        circlePlayerAnimator.Play("PlayerCircleCloseMainMenu");
        playerMovement.SetCanMove(true);
        cinemachineManager.SwitchToMenuCameraAnim();
    }

    private void HandleStarting() //Invoked from the main menu
    {
        circleCameraAnimator.gameObject.SetActive(false);
        circlePlayerAnimator.gameObject.SetActive(true);
        circlePlayerAnimator.Play("PlayerCircleOpen");
        parallaxEffect.SetStopParallax(false);

        //initial configurations
        mapGenerator.GenerateMap(enterGameLength, trackLength, trackLength + 50, false);
        //backgroundController.NextBackground(enterGameLength, enterWallEnd + enterTrackLength);

        //Change MenuCamera to GameCamera
        cinemachineManager.SwitchToTrackCamera();

        //Calculate right fuel consumption for the lenght of the wall
        playerMovement.CalculateWallMultiplier(wallLength);
        //Enable movement
        playerMovement.SetCanMove(true);
        //Stop consuming fuel for the initial animation
        playerMovement.StopConsumingMFuel(true);

        

        EventManager.OnGameStarted();
    }
    private void HandleEnterTrack()
    {
        circlePlayerAnimator.gameObject.SetActive(false);
        cinemachineManager.SwitchToTrackCamera();

        playerMovement.EnablePlayerMovementSFX(true);

        gameRecord.IncrementCurrentScore();

        //Stop consuming minecart fuel
        playerMovement.StopConsumingMFuel(true);

        //Remove input from player
        BindingManager.Instance.PlayerInput(false);

        cinemachineManager.StopScreenShakeWallMine();

        EventManager.OnEnterTrack();
    }
    private void HandleTrack()
    {
        circlePlayerAnimator.gameObject.SetActive(false);

        blackScreen.SetActive(false);
        playerMovement.EnablePlayerMovementSFX(true);
        //Give back input to the player
        BindingManager.Instance.PlayerInput(true);

        //Start consuming minecart fuel
        playerMovement.StopConsumingMFuel(false);
        
        //Calculate new end positions
        CalculateEndPositions();

        mapGenerator.DrawWall(enterWallEnd, wallLength);

        EventManager.OnTrack();
    }
    private void HandleEnterWall() 
    {
        cinemachineManager.SwitchToWallCamera();

        //Remove input from player
        BindingManager.Instance.PlayerInput(false);

        //Stop consuming minecart fuel
        playerMovement.StopConsumingMFuel(true);
        playerMovement.PreventiveFastFall();

        EventManager.OnEnterWall();
    }
    private void HandleWall()
    {
        //Give back player input
        BindingManager.Instance.PlayerInput(true);
        //cinemachineManager.StartScreenShakeWallMine();

        //Generate new set of MapObjects
        mapGenerator.GenerateMap(enterTrackEnd, trackLength, enterTrackEnd + trackLength + 50, true);
        backgroundController.NextBackground(wallEnd, enterWallEnd);

        EventManager.OnWall();
    }
    private void HandleGameOver()
    {
        PauseGame(true);
        cinemachineManager.StopScreenShakeWallMine();
        //blackScreen.SetActive(true);
        //circlePlayerAnimator.gameObject.SetActive(true);
        //circlePlayerAnimator.Play("PlayerCircleClose");
        gameOverSignal.SendSignal();
    }

    #endregion

    #region Update Methods

    private void UpdateMainMenuAnimation()
    {
        //Check if player has traveled all "mainMenuLength"
        if (playerMovement.transform.position.x >= 0)
        {
            playSignal.SendSignal();
            mainMenu1Background.SetActive(false);
            mainMenu2Background.SetActive(false);
            ChangeState(GameState.Starting);
        }

    }

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

    private void ResetGame(bool toMainMenu)
    {
        gameRecord.UpdateMaxScore();
        gameRecord.ResetCurrentScore();
        //Reset background parallaxelements
        parallaxEffect.ResetElements();
        parallaxEffect.SetStopParallax(true);
        cinemachineManager.StopScreenShakeWallMine();

        //Reset player movement values
        if (toMainMenu)
        {
            playerMovement.ResetPlayerMovement(new Vector2(-mainMenuLength, 1));
        }
        else
        {
            playerMovement.ResetPlayerMovement(Vector2.zero);
        }
        parallaxEffect.SetStopParallax(true);

        //Reset map generation parameters
        ResetEndPositions();
        mapGenerator.ClearAllMapObjects();

        EventManager.OnGameRestarted();
    }

    public void LowPassMusic()
    {
        AudioManager.instance.SetCutOffFrequencyMusic(1000);
    }

    public void NormalPassMusic()
    {
        AudioManager.instance.SetCutOffFrequencyMusic(22000);
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

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            SaveSystem.SaveGameRecord(gameRecord);
        }
    }

    public void PauseMenuLoaded()
    {
        EventManager.OnPauseMenuLoaded();
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

    public void CheckNewRecord()
    {
        if(currentScore > maxScore)
        {
            AudioManager.instance.PlaySFX("gameovernewrecord");
            EventManager.OnNewRecord();
        }
        else
        {
            AudioManager.instance.PlaySFX("gameover");
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
        AudioManager.instance.PlaySFX("score");
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