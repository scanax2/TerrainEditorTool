using UnityEngine;

public class HandleInputMono : MonoBehaviour
{
    public bool IsHoldLeftMouse { get; private set; }
    public bool IsHoldLeftShift { get; private set; }


    void Update()
    {
        IsHoldLeftMouse = IsHoldButton(KeyCode.Mouse0);
        IsHoldLeftShift = IsHoldButton(KeyCode.LeftShift);
    }

    private bool IsHoldButton(KeyCode keyCode)
    {
        if (Input.GetKey(keyCode))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
