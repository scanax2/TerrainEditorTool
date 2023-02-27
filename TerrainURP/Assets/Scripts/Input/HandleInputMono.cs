using System;
using UnityEngine;


public class HandleInputMono : MonoBehaviour
{
    public Action OnLeftMouseButtonStartHold { get; }
    public Action OnLeftMouseButtonEndHold { get; }
    public Action OnLeftShiftStartHold { get; }
    public Action OnLeftShiftEndHold { get; }


    void Update()
    {
        HandleButtonHold(KeyCode.Mouse0, OnLeftMouseButtonStartHold, OnLeftShiftEndHold);
        HandleButtonHold(KeyCode.LeftShift, OnLeftShiftStartHold, OnLeftShiftEndHold);
    }

    private void HandleButtonHold(KeyCode keyCode, Action onStart, Action onEnd)
    {
        if (Input.GetKey(keyCode))
        {
            onStart?.Invoke();
        }
        else
        {
            onEnd?.Invoke();
        }
    }
}
