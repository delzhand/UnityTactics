using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    Front,
    Side,
    Back
}

public class Facer : MonoBehaviour
{

    Vector3 lastPosition;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Engine.InputManager.Attach == this)
        {
            Direction? d = Engine.InputManager.DirectionFromInput();
            if (d != null)
            {
                FaceDirection((Direction)d);
                Engine.SoundManager.PlaySound("cursor_change");
            }

            if (Engine.InputManager.Accept)
            {
                Engine.InputManager.Accept = false;
                GetComponent<ActiveTurn>().DoneFacing();
                Engine.SoundManager.PlaySound("cursor_select");
            }
            else if (Engine.InputManager.Cancel)
            {
                Engine.InputManager.Cancel = false;
                if (GetComponent<ActiveTurn>().ActPoints > 0 || GetComponent<ActiveTurn>().MovePoints > 0)
                {
                    Destroy(transform.Find("Facing Selector").gameObject);
                    GetComponent<ActiveTurn>().WaitState = WaitState.None;
                    Engine.AssignControl(GetComponent<ActiveTurn>());
                    Engine.SoundManager.PlaySound("cursor_cancel");
                }
            }
        }
    }

    public void UpdateFacing()
    {
        if (transform.position != lastPosition)
        {
            Vector3 velocity = transform.position - lastPosition;
            float angle = Mathf.Atan2(velocity.x, velocity.z);
            transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * angle, 0);
            lastPosition = transform.position;
        }
    }

    public void FaceDirection(Direction d)
    {
        switch(d)
        {
            case Direction.North:
                FaceDirection(0);
                break;
            case Direction.Northeast:
                FaceDirection(45);
                break;
            case Direction.East:
                FaceDirection(90);
                break;
            case Direction.Southeast:
                FaceDirection(135);
                break;
            case Direction.South:
                FaceDirection(180);
                break;
            case Direction.Southwest:
                FaceDirection(225);
                break;
            case Direction.West:
                FaceDirection(270);
                break;
            case Direction.Northwest:
                FaceDirection(315);
                break;
        }
    }

    public void FaceDirection(int degrees)
    {
        transform.eulerAngles = new Vector3(0, degrees, 0);
    }

    public void FaceTile(Tile t)
    {
        Vector3 r = transform.position - t.transform.position;
        float angle = Mathf.Atan2(r.x, r.z);
        angle *= Mathf.Rad2Deg;
        angle += 180;
        angle = (int)angle / 90 * 90;
        transform.eulerAngles = new Vector3(0, angle, 0);
    }

    public Direction CurrentDirection()
    {
        return (Direction)(transform.eulerAngles.y / 45);
    }
}