using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    public float fuelCanValue;
    public float fuelcanExtra;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
