using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Components;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private AudioClip sfx;

    private void Awake()
    {
        GetComponent<UIButton>().onPointerDownEvent.AddListener(() =>
        {
            AudioManager.instance.PlaySFX(sfx);
        });
    }
}
