using UnityEngine;

public class LinkedZiplineScript : MonoBehaviour
{
    [Header("Warning: Smoothing will slightly\nchange location of nodes.")]
    [Header("Smoothing previews will be shown in\nblue, GetBezierOfPath is a rough\nestimate of the path")]
    [Header("Try not to go to crazy on the smooth amount.")]
    public bool EnableSmoothing = false;
    public int SmoothAmount = 50;

    [Header("Checked: GetAllPointsOnBezier\nUnchecked: GetBezierOfPath")]
    public bool SmoothType = true;
}
