using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    [SerializeField] private Slider excavatorFuelSlider;
    [SerializeField] private Slider minecartFuelSlider;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void SetMinecartFuelUI(float value)
    {
        minecartFuelSlider.value = value;
    }

    public void SetExcavatorFuelUI(float value)
    {
        excavatorFuelSlider.value = value;
    }
}
