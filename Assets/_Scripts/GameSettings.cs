using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public float fuelLossHit;
    public float fuelCanValue;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
