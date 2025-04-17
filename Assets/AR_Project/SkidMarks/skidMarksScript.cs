using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class SkidMarksScript : MonoBehaviour
{
    public List<TrailRenderer> renderer = new List<TrailRenderer>();
    public void StartTrial()
    {
        foreach(TrailRenderer render in renderer)
        {
            render.emitting = true;
        }
    }
    public void StopTrial()
    {
        foreach (TrailRenderer render in renderer)
        {
            render.emitting = false;
        }
    }
}
