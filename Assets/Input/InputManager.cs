using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action OnActionStarted;
    public static Action OnActionCanceled;

    public void OnAction(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnActionStarted?.Invoke();
        }

        if (ctx.canceled)
        {
            OnActionCanceled?.Invoke();
        }
    }
}
