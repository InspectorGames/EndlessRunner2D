using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera menuCamera;
    [SerializeField] private CinemachineVirtualCamera menuCameraAnim;
    [SerializeField] private CinemachineVirtualCamera trackCamera;
    [SerializeField] private CinemachineVirtualCamera wallCamera;

    [SerializeField] private CinemachineImpulseSource screenShakeHit;
    [SerializeField] private CinemachineImpulseSource screenShakeLand;
    [SerializeField] private CinemachineImpulseSource screenShakeMineWall;

    private bool isShakingMineWall = false;

    private void Update()
    {
        if (isShakingMineWall)
        {
            ScreenShakeWallMine();
        }
    }

    public void ScreenShakeHit()
    {
        screenShakeHit.GenerateImpulseWithVelocity(new Vector3(1, 0) * 0.1f);
    }

    public void ScreenShakeLand()
    {
        screenShakeLand.GenerateImpulseWithVelocity(new Vector3(0, 1) * 0.05f);
    }

    public void ScreenShakeLandHard()
    {
        screenShakeLand.GenerateImpulseWithVelocity(new Vector3(0, 1) * 0.1f);
    }

    private float wallMineTimer = 0;
    private void ScreenShakeWallMine()
    {
        wallMineTimer += Time.deltaTime;
        if(wallMineTimer > 0.2f)
        {
            screenShakeMineWall.GenerateImpulseWithVelocity(new Vector3(0,1) * 0.05f);
            wallMineTimer = 0;
        }
    }

    public void StartScreenShakeWallMine()
    {
        isShakingMineWall = true;
    }

    public void StopScreenShakeWallMine()
    {
        isShakingMineWall = false;
    }


    public void SwitchToMenuCameraAnim()
    {
        menuCameraAnim.Priority = 1;
        trackCamera.Priority = 0;
        menuCamera.Priority = 0;
        wallCamera.Priority = 0;
    }

    public void SwitchToTrackCamera()
    {
        menuCameraAnim.Priority = 0;
        trackCamera.Priority = 1;
        menuCamera.Priority = 0;
        wallCamera.Priority = 0;
    }

    public void SwitchToMenuCamera()
    {
        menuCameraAnim.Priority = 0;
        trackCamera.Priority = 0;
        menuCamera.Priority = 1;
        wallCamera.Priority = 0;
    }

    public void SwitchToWallCamera()
    {
        menuCameraAnim.Priority = 0;
        trackCamera.Priority = 0;
        menuCamera.Priority = 0;
        wallCamera.Priority = 1;
    }

    public void SwitchToTrackCameraTutorial()
    {
        trackCamera.Priority = 1;
        wallCamera.Priority = 0;
    }

    public void SwitchToWallCameraTutorial()
    {
        trackCamera.Priority = 0;
        wallCamera.Priority = 1;
    }
}
