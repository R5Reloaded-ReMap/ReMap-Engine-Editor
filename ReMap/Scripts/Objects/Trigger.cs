using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ReMap/Trigger", 0)]
public class Trigger : MonoBehaviour
{
    public float radius = 1.0f;
    public float height = 1.0f;
    public int numSides = 16;
    public Color color = Color.red;

    private void OnDrawGizmos()
    {
        GizmosExtensions.DrawWireCylinder(transform.position, radius / 2, height * 2, color, numSides);
    }
}
