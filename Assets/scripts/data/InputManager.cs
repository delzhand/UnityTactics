using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float inputDelay = 0;
    float interval = .15f;
    float initialInterval = .5f;
    public bool inputCleared = true;

    public MonoBehaviour Attach;

    public bool Accept;
    public bool Cancel;

    void Update()
    {
        inputDelay -= Time.deltaTime;
        if (!inputCleared && inputNeutral())
        {
            inputCleared = true;
            inputDelay = 0;
        }

        Accept = AcceptUp();
        Cancel = CancelUp();
    }

    private bool inputNeutral()
    {
        return Mathf.Abs(Input.GetAxis("Vertical")) < .75f &&
            Mathf.Abs(Input.GetAxis("Horizontal")) < .75f &&
            Mathf.Abs(Input.GetAxis("Tilt")) < .75f &&
            Mathf.Abs(Input.GetAxis("Zoom")) < .75f;

        //return Mathf.Abs(Input.GetAxis("Vertical")) < .75f &&
        //    Mathf.Abs(Input.GetAxis("Horizontal")) < .75f &&
        //    Input.GetAxis("Change") == 0;
    }

    public bool Up()
    {
        return intervalDelay(Input.GetAxis("Vertical") == 1);
    }

    public bool Down()
    {
        return intervalDelay(Input.GetAxis("Vertical") == -1);
    }

    public bool Right()
    {
        return intervalDelay(Input.GetAxis("Horizontal") == 1);
    }

    public bool Left()
    {
        return intervalDelay(Input.GetAxis("Horizontal") == -1);
    }

    public bool AcceptUp()
    {
        return intervalDelay(Input.GetButtonUp("Accept"));
    }

    public bool CancelUp()
    {
        return intervalDelay(Input.GetButtonUp("Cancel"));
    }

    public bool RotateLeft()
    {
        return intervalDelay(Input.GetButtonUp("RotateLeft"));
    }

    public bool RotateRight()
    {
        return intervalDelay(Input.GetButtonUp("RotateRight"));
    }

    public bool TiltUp()
    {
        return intervalDelay(Input.GetAxis("Tilt") == -1);
    }

    public bool TiltDown()
    {
        return intervalDelay(Input.GetAxis("Tilt") == 1);
    }

    public float Zoom()
    {
        return (Input.GetAxis("Zoom"));
    }

    //public bool ZoomIn()
    //{
    //    return intervalDelay(Input.GetAxis("Zoom") == -1);
    //}

    //public bool ZoomOut()
    //{
    //    return intervalDelay(Input.GetAxis("Zoom") == 1);
    //}

    private bool intervalDelay(bool inputValue)
    {
        if (inputValue && inputDelay < 0)
        {
            if (inputCleared)
            {
                inputDelay = initialInterval;
            }
            else
            {
                inputDelay = interval;
            }
            inputCleared = false;
            return true;
        }
        return false;
    }

    public Direction? DirectionFromInput()
    {
        int direction = Engine.CameraManager.DirectionOffset * 2;
        if (Engine.InputManager.Up())
        {
            direction = direction % 8;
        }
        else if (Engine.InputManager.Right())
        {
            direction = (direction + 2) % 8;
        }
        else if (Engine.InputManager.Down())
        {
            direction = (direction + 4) % 8;
        }
        else if (Engine.InputManager.Left())
        {
            direction = (direction + 6) % 8;
        }
        else
        {
            return null;
        }
        return (Direction)direction;
    }
}
