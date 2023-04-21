using Doozy.Runtime.Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialParts {
    Jump,
    FastFall,
    ExcavatorFuel,
    UseExcavatorFuel,
    MinecartFuel,
    Bars,
    Wall,
    EndTutorial,
    Obstacle
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerExcavatorV2 excavatorEffect;
    [SerializeField] private UIController uiController;
    [SerializeField] private CinemachineManager cinemachineManager;

    [Header("Cinematic Stuff")]
    [SerializeField] private SpriteRenderer courtainSprite;
    [SerializeField] private Animator excavatorCircleAnimator;
    [SerializeField] private Animator minecartCircleAnimator;

    [Space(5)]

    [SerializeField] private SignalSender WaitJumpSignal;
    [SerializeField] private SignalSender WaitFastFallSignal;
    [SerializeField] private SignalSender WaitExcavatorFuelSignal;
    [SerializeField] private SignalSender WaitObstacleHitSignal;
    [SerializeField] private SignalSender WaitUseExcavatorFuelSignal;
    [SerializeField] private SignalSender WaitMinecartFuelSignal;
    [SerializeField] private SignalSender WaitBarsSignal;
    [SerializeField] private SignalSender WaitWallSignal;
    [SerializeField] private SignalSender WaitEndTutorialSignal;


    public static event UnityAction WaitJump;
    public static void OnWaitJump() => WaitJump?.Invoke();
    public static event UnityAction WaitFastFall;
    public static void OnWaitFastFall() => WaitFastFall?.Invoke();
    public static event UnityAction WaitObstacle;
    public static void OnWaitObstacle() => WaitObstacle?.Invoke();
    public static event UnityAction WaitExcavatorFuel;
    public static void OnWaitExcavatorFuel() => WaitExcavatorFuel?.Invoke();
    public static event UnityAction WaitUseExcavatorFuel;
    public static void OnWaitUseExcavatorFuel() => WaitUseExcavatorFuel?.Invoke();
    public static event UnityAction WaitMinecartFuel;
    public static void OnWaitMinecartFuel() => WaitMinecartFuel?.Invoke();
    public static event UnityAction WaitBars;
    public static void OnBars() => WaitBars?.Invoke();
    public static event UnityAction WaitWall;
    public static void OnWaitWall() => WaitWall?.Invoke();
    public static event UnityAction EndTutorial;
    public static void OnEndTutorial() => EndTutorial?.Invoke();

    private void OnEnable()
    {
        WaitJump += ExplainJump;
        WaitFastFall += ExplainFastFall;
        WaitExcavatorFuel += ExplainExcavatorFuel;
        WaitObstacle += ExplainObstacle;
        WaitUseExcavatorFuel += ExplainUseExcavatorFuel;
        WaitMinecartFuel += ExplainMinecartFuel;
        WaitBars += ExplainBars;
        WaitWall += ExplainWall;
        EndTutorial += ToEndTutorial;

        EventManager.PlayerLand += cinemachineManager.ScreenShakeLand;
        EventManager.PlayerLandHard += cinemachineManager.ScreenShakeLandHard;
    }

    private void OnDisable()
    {
        WaitJump -= ExplainJump;
        WaitFastFall -= ExplainFastFall;
        WaitExcavatorFuel -= ExplainExcavatorFuel;
        WaitObstacle -= ExplainObstacle;
        WaitUseExcavatorFuel -= ExplainUseExcavatorFuel;
        WaitMinecartFuel -= ExplainMinecartFuel;
        WaitBars -= ExplainBars;
        WaitWall -= ExplainWall;
        EndTutorial -= ToEndTutorial;

        EventManager.PlayerLand -= cinemachineManager.ScreenShakeLand;
        EventManager.PlayerLandHard -= cinemachineManager.ScreenShakeLandHard;
    }

    private void Start()
    {
        playerMovement.SetCanMove(true);
        playerMovement.SetIsTutorial(true);
        playerMovement.EnablePlayerMovementSFX(true);
        uiController.SetIsTutorial(true);
        AudioManager.instance.PlayMusic("tutorialv3");
    }

    public void HideCurtain()
    {
        courtainSprite.color = new Color(0, 0, 0, 0);
    }

    private void ShowCurtain()
    {
        courtainSprite.color = new Color(0f, 0f, 0f, 0.95f);
    }


    private void ExplainJump()
    {
        Time.timeScale = 0;
        WaitJumpSignal.SendSignal();
    }

    public void PassedJump()
    {
        playerMovement.TutorialJump();
    }

    private void ExplainFastFall()
    {
        Time.timeScale = 0;
        playerMovement.TutorialFastFall();
        WaitFastFallSignal.SendSignal();
    }

    private void ExplainExcavatorFuel()
    {
        Time.timeScale = 0;
        ShowCurtain();
        excavatorCircleAnimator.Play("ExcavatorFuelCircleClosed");
        WaitExcavatorFuelSignal.SendSignal();
    }

    private void ExplainObstacle()
    {
        Time.timeScale = 0;
        WaitObstacleHitSignal.SendSignal();
    }

    private void ExplainUseExcavatorFuel()
    {
        Time.timeScale = 0;
        WaitUseExcavatorFuelSignal.SendSignal();
    }

    public void UsedExcavatorFuel()
    {
        playerMovement.TutorialUseExcavatorFuel();
    }

    private void ExplainMinecartFuel()
    {
        Time.timeScale = 0;
        ShowCurtain();
        minecartCircleAnimator.Play("MinecartFuelCircleClosed");
        WaitMinecartFuelSignal.SendSignal();
    }

    private void ExplainBars()
    {
        Time.timeScale = 0;
        WaitBarsSignal.SendSignal();
    }

    private void ExplainWall()
    {
        Time.timeScale = 0;
        cinemachineManager.SwitchToWallCameraTutorial();
        playerMovement.EnterWallPhase();
        playerMovement.CalculateWallMultiplier(100);
        excavatorEffect.SpawnEffectTutorial();
        WaitWallSignal.SendSignal();
    }

    private void ToEndTutorial()
    {
        Time.timeScale = 0;
        excavatorEffect.DestroyEffectTutorial();
        ShowCurtain();
        playerMovement.ExitWallPhase();
        cinemachineManager.SwitchToTrackCameraTutorial();
        SaveSystem.SaveGameRecord(new GameRecord(0));
        WaitEndTutorialSignal.SendSignal();
    }

}
