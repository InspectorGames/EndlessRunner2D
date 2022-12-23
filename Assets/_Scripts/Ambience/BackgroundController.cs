using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private GameObject trackBackground;
    [SerializeField] private GameObject wallBackground;

    public void NextBackground(int newTrackX, int newWallX)
    {
        trackBackground.transform.position = new Vector3(newTrackX, 0);
        wallBackground.transform.position = new Vector3(newWallX, 0);
    }
}
