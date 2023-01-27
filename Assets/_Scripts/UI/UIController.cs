using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Reactor;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("--Main Menu UI--")]
    
    [Space(1)]
    
    [Header("Record")]
    [SerializeField] private TextMeshProUGUI recordText;

    [Space(20)]

    [Header("--In Game UI--")]

    [Space(1)]

    [Header("Fuel")]
    [SerializeField] private Progressor minecartFuelProgressor;
    [SerializeField] private Progressor excavatorFuelProgressor;
    [SerializeField] private ExcavatorFuelCansUI excavatorsFuelCans;
    [SerializeField] private GameObject excavatorFuelButton;

    [Space(1)]

    [Header("Wall")]
    [SerializeField] private Animator touchScreenIndicator;

    [Space(1)]

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Space(1)]

    [Header("Game Over")]
    [SerializeField] private TextMeshProUGUI countdown;

    [Space(20)]

    [Header("--Game Over UI--")]

    [Space(1)]

    [Header("Normal Record")]
    [SerializeField] private GameObject normalRecordContainer;
    [SerializeField] private TextMeshProUGUI record;
    [SerializeField] private TextMeshProUGUI score;

    [Space(1)]

    [Header("New Record")]
    [SerializeField] private GameObject newRecordContainer;
    [SerializeField] private TextMeshProUGUI newRecord;


    private bool isTutorial;

    private void Start()
    {
        UpdateRecordText();
    }

    private void Update()
    {
        UpdateMinecartFuelProgressor();

        if (!isTutorial)
        {
            if(GameManager.Instance.CurrentState == GameState.Wall)
            {
                UpdateExcavatorFuelProgressor();
            }
        }
        else
        {
            UpdateExcavatorFuelProgressor();
        }

        if (canCountdown)
        {
            Timer();
        }
    }

    public void SetIsTutorial(bool value)
    {
        isTutorial = value;
    }

    private void OnEnable()
    {

        EventManager.MainMenuLoaded += UpdateRecordText;


        EventManager.GameStarted += UpdateExcavatorFuelCans;
        EventManager.GameStarted += UpdateScoreText;
        EventManager.GameRestarted += ShowExcavatorFuelButton;
        EventManager.GameRestarted += UpdateScoreText;

        //EventManager.ExcavatorFuelCollected += UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelCollected.Enqueue(1, UpdateExcavatorFuelCans);
        //EventManager.ExcavatorFuelConsumed += UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelConsumed.Enqueue(1, UpdateExcavatorFuelCans);

        EventManager.EnterWall.Enqueue(1, HideExcavatorFuelButton);
        EventManager.Wall.Enqueue(1, SwitchToExcavatorProgressBar);
        EventManager.Wall.Enqueue(2, UpdateExcavatorFuelProgressor);
        EventManager.Wall.Enqueue(3, TouchScreenIndicatorAnimation);
        EventManager.EnterTrack.Enqueue(1, SwitchToExcavatorCans);
        EventManager.EnterTrack.Enqueue(2, UpdateExcavatorFuelCans);
        EventManager.EnterTrack.Enqueue(3, ShowExcavatorFuelButton);
        EventManager.WallScored += UpdateScoreText;

        EventManager.GameRestarted += SwitchToExcavatorCans;
        EventManager.GameRestarted += UpdateRecordText;
        EventManager.GameRestarting += StopCountdown;

        EventManager.GameOver += StopCountdown;
        EventManager.GameOverCountdownStarted += StartCountdown;
        EventManager.GameOverCountdownStopped += StopCountdown;

        EventManager.GameOver += ShowNormalRecord;
        EventManager.NewRecord += ShowNewRecord;

        TutorialManager.WaitWall += SwitchToExcavatorProgressBar;
        TutorialManager.WaitWall += HideExcavatorFuelButton;
        TutorialManager.WaitWall += TouchScreenIndicatorAnimation;
    }

    private void OnDisable()
    {

        EventManager.MainMenuLoaded -= UpdateRecordText;


        EventManager.GameStarted -= UpdateExcavatorFuelCans;
        EventManager.GameStarted -= UpdateScoreText;
        EventManager.GameRestarted -= ShowExcavatorFuelButton;
        EventManager.GameRestarted -= UpdateScoreText;

        //EventManager.ExcavatorFuelCollected -= UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelCollected.Dequeue(1);
        //EventManager.ExcavatorFuelConsumed -= UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelConsumed.Dequeue(1);

        EventManager.EnterWall.Dequeue(1);
        EventManager.Wall.Dequeue(1);
        EventManager.Wall.Dequeue(2);
        EventManager.Wall.Dequeue(3);
        EventManager.EnterTrack.Dequeue(1);
        EventManager.EnterTrack.Dequeue(2);
        EventManager.EnterTrack.Dequeue(3);
        EventManager.WallScored -= UpdateScoreText;

        EventManager.GameRestarted -= SwitchToExcavatorCans;
        EventManager.GameRestarted -= UpdateRecordText;
        EventManager.GameRestarting -= StopCountdown;

        EventManager.GameOver -= StopCountdown;
        EventManager.GameOverCountdownStarted -= StartCountdown;
        EventManager.GameOverCountdownStopped -= StopCountdown;

        EventManager.GameOver -= ShowNormalRecord;
        EventManager.NewRecord -= ShowNewRecord;

        TutorialManager.WaitWall -= SwitchToExcavatorProgressBar;
        TutorialManager.WaitWall -= HideExcavatorFuelButton;
        TutorialManager.WaitWall -= TouchScreenIndicatorAnimation;
    }

    #region MainMenu
    private void UpdateRecordText()
    {
        if (isTutorial) return;
        recordText.text = "" + GameManager.Instance.CurrentRecord.GetMaxScore();
    }
    #endregion


    #region InGame

    public void SwitchToExcavatorProgressBar()
    {
        excavatorFuelProgressor.gameObject.SetActive(true);
        excavatorsFuelCans.gameObject.SetActive(false);
    }

    public void SwitchToExcavatorCans()
    {
        excavatorFuelProgressor.gameObject.SetActive(false);
        excavatorsFuelCans.gameObject.SetActive(true);
    }

    private void UpdateMinecartFuelProgressor()
    {
        minecartFuelProgressor.SetProgressAt(playerMovement.GetCurrentMinecartFuel() / 100);
    }

    private void UpdateExcavatorFuelProgressor()
    {
        excavatorFuelProgressor.SetProgressAt(playerMovement.GetCurrentExcavatorFuel() / 100);
    }

    private void UpdateExcavatorFuelCans()
    {
        excavatorsFuelCans.SetFuel(playerMovement.GetCurrentExcavatorFuelCans());
        excavatorsFuelCans.SetExtraFuel(playerMovement.HasExtraFuelCan());
    }

    private void UpdateScoreText()
    {
        scoreText.text = ""+GameManager.Instance.CurrentRecord.GetCurrentScore();
    }

    private void HideExcavatorFuelButton()
    {
        excavatorFuelButton.gameObject.SetActive(false);
    }

    private void ShowExcavatorFuelButton()
    {
        excavatorFuelButton.gameObject.SetActive(true);
    }

    private void StartCountdown()
    {
        remainingSeconds = 3;
        timer = 0;
        canCountdown = true;
        countdown.gameObject.SetActive(true);
        countdown.text = "" + remainingSeconds;
        countdown.gameObject.GetComponent<Animator>().Play("Countdown");
    }

    private void StopCountdown()
    {
        canCountdown = false;
        countdown.gameObject.SetActive(false);
    }

    private bool canCountdown = false;
    private float timer;
    private int remainingSeconds = 3;
    private void Timer()
    {
        timer += Time.deltaTime;
        if(timer >= 1)
        {
            remainingSeconds--;
            countdown.text = "" + remainingSeconds;
            timer = 0;
        }
    }

    private void ShowNormalRecord()
    {
        newRecordContainer.SetActive(false);
        normalRecordContainer.SetActive(true);

        record.text = "" + GameManager.Instance.CurrentRecord.GetMaxScore();
        score.text = "" + GameManager.Instance.CurrentRecord.GetCurrentScore();

        GameManager.Instance.CurrentRecord.CheckNewRecord();
    }

    private void ShowNewRecord()
    {
        newRecordContainer.SetActive(true);
        normalRecordContainer.SetActive(false);

        newRecord.text = "" + GameManager.Instance.CurrentRecord.GetCurrentScore();
    }

    private void TouchScreenIndicatorAnimation()
    {
        touchScreenIndicator.Play("Touch");
    }

    #endregion
}
