using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Components;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private AudioClip sfx;
    [SerializeField] private UIButton button;

    private void Awake()
    {
        button = GetComponent<UIButton>();
        button.onPointerDownEvent.AddListener(() =>
        {
            AudioManager.instance.PlaySFX(sfx);
        });
    }
}
