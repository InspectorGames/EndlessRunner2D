using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExcavatorFuelCansUI : MonoBehaviour
{
    [SerializeField] private Sprite fuel;
    [SerializeField] private Sprite noFuel;
    [SerializeField] private List<Image> fuelImages;
    [SerializeField] private Image extraFuel;
    [SerializeField] private Sprite extraFuelSprite;

    private void Start()
    {
        SetAllNoFuel();
    }

    public void SetAllNoFuel()
    {
        for(int i = 0; i < fuelImages.Count; i++)
        {
            fuelImages[i].sprite = noFuel;
        }
    }

    public void SetFuel(int value)
    {
        for(int i = 0; i < value; i++)
        {
            if (i > fuelImages.Count) return;
            fuelImages[i].sprite = fuel;
        }

        for(int i = value; i < fuelImages.Count; i++)
        {
            fuelImages[i].sprite = noFuel;
        }
    }

    public void SetExtraFuel(bool value)
    {
        if (value)
        {
            extraFuel.sprite = extraFuelSprite;
        }
        else
        {
            extraFuel.sprite = noFuel;
        }
    }
}
