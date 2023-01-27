using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Components;

public class CheckToggle : MonoBehaviour
{
    public enum Toggle
    {
        Music,
        SFX
    }

    public Toggle check;
    private UIToggle toggle;

    private void Awake()
    {
        toggle = GetComponent<UIToggle>();
    }

    private void OnEnable()
    {
        EventManager.MainMenuLoaded += Check;
        EventManager.PauseMenuLoaded += Check;
    }

    private void OnDisable()
    {
        EventManager.MainMenuLoaded -= Check;
        EventManager.PauseMenuLoaded -= Check;
    }

    private void Check()
    {
        switch (check)
        {
            case Toggle.Music:
                if (AudioManager.instance.IsMusicMuted())
                {
                    toggle.isOn = false;
                }
                break;
            case Toggle.SFX:
                if (AudioManager.instance.IsSFXMuted())
                {
                    toggle.isOn = false;
                }
                break;
        }
    }

}
