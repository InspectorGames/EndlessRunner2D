using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Runtime.Reactor;
using TMPro;

public class InGameUI : MonoBehaviour
{

    [SerializeField] private ExcavatorFuelCansUI excavatorFuelCansUI;
    [SerializeField] private Progressor excavatorFuelUI;
    [SerializeField] private Progressor minecartFuelSlider;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SwitchToExcavatorProgressBar()
    {
        excavatorFuelUI.gameObject.SetActive(true);
        excavatorFuelCansUI.gameObject.SetActive(false);
    }

    public void SwitchToExcavatorCans()
    {
        excavatorFuelUI.gameObject.SetActive(false);
        excavatorFuelCansUI.gameObject.SetActive(true);
    }

    public void SetMinecartFuelUI(float value)
    {
        minecartFuelSlider.SetProgressAt(value);
    }

    public void SetExcavatorFuelUI(float value)
    {
        excavatorFuelUI.SetProgressAt(value);
    }

    public void SetExcavatorFuelCansUI(int value)
    {
        excavatorFuelCansUI.SetFuel(value);
    }

    public void SetExtraExcavatorFuel(bool value)
    {
        excavatorFuelCansUI.SetExtraFuel(value);
    }

    public void SetCurrentScore(int value)
    {
        scoreText.text = "" + value;
    }
}
