using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearance : MonoBehaviour {
    public GameObject Skeleton;
    public GameObject Head;
    public GameObject Chest;
    public GameObject Trunk;
    public GameObject Hips;
    public GameObject ArmUpperRight;
    public GameObject ArmUpperLeft;
    public GameObject ArmLowerRight;
    public GameObject ArmLowerLeft;
    public GameObject LegUpperRight;
    public GameObject LegUpperLeft;
    public GameObject LegLowerRight;
    public GameObject LegLowerLeft;

    public static readonly float scale = .1125f;

    private void Update()
    {
        float light = Mathf.Min(GameObject.Find("Stage/Directional Light").GetComponent<Light>().intensity + .2f, 1);
        Head.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        Chest.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        Trunk.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        Hips.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        ArmUpperLeft.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        ArmUpperRight.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        ArmLowerRight.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        ArmLowerLeft.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        LegLowerLeft.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        LegLowerRight.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        LegUpperLeft.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
        LegUpperRight.GetComponent<Renderer>().material.color = new Color(light, light, light, 1);
    }

    public void Configure()
    {
        Skeleton = gameObject.GetComponentInChildren<Animator>().gameObject;
        SetPiece(Trunk, "Trunk", new Vector3(0, .0005f, 0), Vector3.zero);
        SetPiece(Chest, "Trunk/Chest", new Vector3(0, -.0017f, 0), Vector3.zero);
        SetPiece(Head, "Trunk/Chest/Head", new Vector3(0, .002f, 0), Vector3.zero);
        SetPiece(ArmUpperRight, "Trunk/Chest/Shoulder R", new Vector3(0, .000f, .000f), new Vector3(0, 90, 90));
        SetPiece(ArmUpperLeft, "Trunk/Chest/Shoulder L", new Vector3(0, .000f, .000f), new Vector3(0, -90, -90));
        SetPiece(ArmLowerRight, "Trunk/Chest/Shoulder R/Forearm R", new Vector3(0, .00f, .0045f), new Vector3(0, 90, 90));
        SetPiece(ArmLowerLeft, "Trunk/Chest/Shoulder L/Forearm L", new Vector3(0, .00f, .0045f), new Vector3(0, -90, -90));
        SetPiece(Hips, "Hips", new Vector3(0, -.0005f, 0), new Vector3(180, 0, 0));
        SetPiece(LegUpperRight, "Hips/Top Leg R", new Vector3(0, .0045f, 0), new Vector3(180, -90, 0));
        SetPiece(LegUpperLeft, "Hips/Top Leg L", new Vector3(0, .0045f, 0), new Vector3(180, 90, 0));
        SetPiece(LegLowerRight, "Hips/Top Leg R/Low Leg R", new Vector3(0, .0005f, 0), new Vector3(180, 0, 0));
        SetPiece(LegLowerLeft, "Hips/Top Leg L/Low Leg L", new Vector3(0, .0005f, 0), new Vector3(180, 0, 0));
    }

    public void Cleanup()
    {
        Skeleton = gameObject.GetComponentInChildren<Animator>().gameObject;
        CleanPiece(Trunk, "Trunk");
        CleanPiece(Chest, "Trunk/Chest");
        CleanPiece(Head, "Trunk/Chest/Head");
        CleanPiece(ArmUpperRight, "Trunk/Chest/Shoulder R");
        CleanPiece(ArmUpperLeft, "Trunk/Chest/Shoulder L");
        CleanPiece(ArmLowerRight, "Trunk/Chest/Shoulder R/Forearm R");
        CleanPiece(ArmLowerLeft, "Trunk/Chest/Shoulder L/Forearm L");
        CleanPiece(Hips, "Hips");
        CleanPiece(LegUpperRight, "Hips/Top Leg R");
        CleanPiece(LegUpperLeft, "Hips/Top Leg L");
        CleanPiece(LegLowerRight, "Hips/Top Leg R/Low Leg R");
        CleanPiece(LegLowerLeft, "Hips/Top Leg L/Low Leg L");
    }

    public void SetPiece(GameObject g, string bonePath, Vector3 position, Vector3 euler)
    {
        Transform bone = Skeleton.transform.Find("Armature.001/" + bonePath).transform;
        g.transform.parent = bone;
        g.transform.localPosition = position;
        g.transform.localEulerAngles = euler;
        g.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void CleanPiece(GameObject g, string bonePath)
    {
        Transform bone = Skeleton.transform.Find("Armature.001/" + bonePath).transform;
        foreach (MeshRenderer m in bone.GetComponentsInChildren<MeshRenderer>())
        {
            if (g != m.gameObject)
            {
                DestroyImmediate(m.gameObject);
            }
        }

    }
}
