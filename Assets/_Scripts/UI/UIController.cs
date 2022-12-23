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

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateRecordText();
    }

    private void Update()
    {
        UpdateMinecartFuelProgressor();

        if(GameManager.Instance.CurrentState == GameState.Wall)
        {
            UpdateExcavatorFuelProgressor();
        }
    }

    private void OnEnable()
    {
        EventManager.MainMenuLoaded += UpdateRecordText;


        EventManager.GameStarted += UpdateExcavatorFuelCans;
        EventManager.GameStarted += UpdateScoreText;
        EventManager.GameRestarted += UpdateScoreText;

        //EventManager.ExcavatorFuelCollected += UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelCollected.Enqueue(1, UpdateExcavatorFuelCans);
        //EventManager.ExcavatorFuelConsumed += UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelConsumed.Enqueue(1, UpdateExcavatorFuelCans);

        EventManager.EnterWall.Enqueue(1, HideExcavatorFuelButton);
        EventManager.Wall.Enqueue(1, SwitchToExcavatorProgressBar);
        EventManager.Wall.Enqueue(2, UpdateExcavatorFuelProgressor);
        EventManager.EnterTrack.Enqueue(1, SwitchToExcavatorCans);
        EventManager.EnterTrack.Enqueue(2, UpdateExcavatorFuelCans);
        EventManager.EnterTrack.Enqueue(3, ShowExcavatorFuelButton);
        EventManager.WallScored += UpdateScoreText;

        EventManager.GameRestarted += SwitchToExcavatorCans;
        EventManager.GameRestarted += UpdateRecordText;
    }

    private void OnDisable()
    {
        EventManager.MainMenuLoaded -= UpdateRecordText;


        EventManager.GameStarted -= UpdateExcavatorFuelCans;
        EventManager.GameStarted -= UpdateScoreText;
        EventManager.GameRestarted -= UpdateScoreText;

        //EventManager.ExcavatorFuelCollected -= UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelCollected.Dequeue(1);
        //EventManager.ExcavatorFuelConsumed -= UpdateExcavatorFuelCans;
        EventManager.ExcavatorFuelConsumed.Dequeue(1);

        EventManager.EnterWall.Dequeue(1);
        EventManager.Wall.Dequeue(1);
        EventManager.Wall.Dequeue(2);
        EventManager.EnterTrack.Dequeue(1);
        EventManager.EnterTrack.Dequeue(2);
        EventManager.EnterTrack.Dequeue(3);
        EventManager.WallScored -= UpdateScoreText;

        EventManager.GameRestarted -= SwitchToExcavatorCans;
        EventManager.GameRestarted -= UpdateRecordText;
    }

    #region MainMenu
    private void UpdateRecordText()
    {
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

    #endregion
}
