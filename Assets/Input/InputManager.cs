using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action OnJumpStarted;
    public static Action OnJumpCanceled;

    public static Action OnConsumeFuelStarted;

    [SerializeField] private ButtonScript jumpButton;
    [SerializeField] private ButtonScript fuelButton;

    private void OnEnable()
    {
        jumpButton.PointerDown += JumpStarted;
        jumpButton.PointerUp += JumpCanceled;

        fuelButton.PointerDown += ConsumeFuel;
    }

    private void OnDisable()
    {
        jumpButton.PointerDown -= JumpStarted;
        jumpButton.PointerUp -= JumpCanceled;

        fuelButton.PointerDown -= ConsumeFuel;
    }

    private void JumpStarted()
    {
        OnJumpStarted?.Invoke();
    }

    private void JumpCanceled()
    {
        OnJumpCanceled?.Invoke();
    }

    private void ConsumeFuel()
    {
        OnConsumeFuelStarted?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            JumpStarted();
        }

        if (ctx.canceled)
        {
            JumpCanceled();
        }
    }

    public void OnConsumeFuel(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            ConsumeFuel();
        }
    }
}
