using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour
{
    public event Action PointerDown;
    public event Action PointerUp;

    public void OnPointerDown()
    {
        PointerDown?.Invoke();
    }

    public void OnPointerUp()
    {
        PointerUp?.Invoke();
    }
}
