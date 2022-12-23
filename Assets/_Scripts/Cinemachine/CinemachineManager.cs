using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera menuCamera;
    [SerializeField] private CinemachineVirtualCamera trackCamera;
    [SerializeField] private CinemachineVirtualCamera wallCamera;

    public void SwitchToTrackCamera()
    {
        trackCamera.Priority = 1;
        menuCamera.Priority = 0;
        wallCamera.Priority = 0;
    }

    public void SwitchToMenuCamera()
    {
        trackCamera.Priority = 0;
        menuCamera.Priority = 1;
        wallCamera.Priority = 0;
    }

    public void SwitchToWallCamera()
    {
        trackCamera.Priority = 0;
        menuCamera.Priority = 0;
        wallCamera.Priority = 1;
    }
}
