using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public Vector3 OriginPosition;
    public float OriginHRotation;
    public float OriginVRotation;
    public float OriginSize;

    public Vector3 TargetPosition;
    public float TargetHRotation;
    public float TargetVRotation;
    public float TargetSize;

    public float Transition;

    public float Duration;
    public float Timer;

    private float playSpeed = 0;

    public bool Locked = false;

    public int DirectionOffset;

    public Camera Camera;

    private void Awake()
    {
        Camera = GetComponentInChildren<Camera>();
    }

    // Use this for initialization
    void Start () {
        playSpeed = 0;
    }

    // Update is called once per frame
    void Update () {
        Timer += (playSpeed * Time.deltaTime);
        if (Timer > Duration || Timer < 0)
        {
            playSpeed = 0;
            Locked = false;
        }
        Transition = Mathf.Clamp(Timer / Duration, 0, 1);
        UpdatePosition();
        UpdateInput();
    }

    public void UpdateInput()
    {
        if (!Locked)
        {
            if (Engine.InputManager.RotateLeft())
            {
                setOriginsToCurrent();
                setTargetsToCurrent();
                TargetHRotation = (OriginHRotation + 45);
                PlayForward();
            }
            if (Engine.InputManager.RotateRight())
            {
                setOriginsToCurrent();
                setTargetsToCurrent();
                TargetHRotation = (OriginHRotation - 45);
                PlayForward();
            }
            if (Engine.InputManager.TiltDown())
            {
                setOriginsToCurrent();
                setTargetsToCurrent();
                TargetVRotation = Mathf.Clamp(OriginVRotation - 5, 15, 65);
                Duration = .25f;
                PlayForward();
            }
            if (Engine.InputManager.TiltUp())
            {
                setOriginsToCurrent();
                setTargetsToCurrent();
                TargetVRotation = Mathf.Clamp(OriginVRotation + 5, 15, 65);
                Duration = .25f;
                PlayForward();
            }
            //if (Engine.InputManager.ZoomIn())
            //{
            //    setOriginsToCurrent();
            //    setTargetsToCurrent();
            //    TargetSize = Camera.orthographicSize - .25f;
            //    Duration = .1f;
            //    PlayForward();
            //}
            //if (Engine.InputManager.ZoomOut())
            //{
            //    setOriginsToCurrent();
            //    setTargetsToCurrent();
            //    TargetSize = Camera.orthographicSize + .25f;
            //    Duration = .1f;
            //    PlayForward();
            //}
        }
    }

    private void setOriginsToCurrent()
    {
        OriginHRotation = transform.rotation.eulerAngles.y;
        OriginVRotation = transform.GetChild(0).localEulerAngles.x;
        OriginPosition = transform.position;
        //OriginSize = Camera.orthographicSize;
    }

    private void setTargetsToCurrent()
    {
        TargetHRotation = transform.rotation.eulerAngles.y;
        TargetVRotation = transform.GetChild(0).localEulerAngles.x;
        TargetPosition = transform.position;
        //TargetSize = Camera.orthographicSize;
    }

    public void SetTargetPosition(Vector3 position, float duration)
    {
        setOriginsToCurrent();
        setTargetsToCurrent();
        TargetPosition = position;
        Duration = duration;
        PlayForward();
    }

    public void UpdatePosition()
    {
        float easeTransition = Mathf.Sin(Mathf.PI / 2 * Transition);

        transform.position = Vector3.Lerp(OriginPosition, TargetPosition, easeTransition);
        transform.localEulerAngles = new Vector3(0, Mathf.Lerp(OriginHRotation, TargetHRotation, easeTransition), 0);
        transform.GetChild(0).localEulerAngles = new Vector3(Mathf.Lerp(OriginVRotation, TargetVRotation, easeTransition), 0, 0);

        // Update direction offset
        int angle = (int)Mathf.Round(transform.localEulerAngles.y / 45f);
        if (SettingsManager.DirectionMode == 1)
        {
            angle += 1;
        }
        angle = (int)(angle % 8)/2;
        DirectionOffset = angle;

        float size = Mathf.Clamp(Camera.orthographicSize + Engine.InputManager.Zoom() / 10, 1.25f, 6);
        Camera.orthographicSize = size;
    }

    public void PlayForward()
    {
        Timer = 0;
        playSpeed = 1;
        Locked = true;
    }
}
