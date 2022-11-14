using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class LinkedZiplineScript : MonoBehaviour
{
    [Header("Warning: Smoothing will slightly\nchange location of nodes.")]
    [Header("Smoothing previews will be shown in\nblue, GetBezierOfPath is a rough\nestimate of the path")]
    [Header("Try not to go to crazy on the smooth amount.")]
    public bool enableSmoothing = false;
    public int smoothAmount = 50;

    [Header("Checked: GetAllPointsOnBezier\nUnchecked: GetBezierOfPath")]
    public bool smoothType = true;
}
