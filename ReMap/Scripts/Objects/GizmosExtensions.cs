using UnityEngine;

public static class GizmosExtensions
{
    public static void DrawWireCylinder(Vector3 position, float radius, float height, Color col, int numSides = 16)
    {
        float angleDelta = 2 * Mathf.PI / numSides;
        Vector3 prevPoint = Vector3.zero;
        Vector3 currPoint = Vector3.zero;
        
        for (int i = 0; i <= numSides; i++)
        {
            float angle = i * angleDelta;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            
            currPoint = new Vector3(x, 0, z);
            
            if (i > 0)
            {
                Gizmos.color = col;
                Gizmos.DrawLine(position + prevPoint + Vector3.up * height / 2, position + currPoint + Vector3.up * height / 2);
                Gizmos.DrawLine(position + prevPoint - Vector3.up * height / 2, position + currPoint - Vector3.up * height / 2);
                Gizmos.DrawLine(position + prevPoint + Vector3.up * height / 2, position + prevPoint - Vector3.up * height / 2);
                Gizmos.color = Color.white;
            }
            
            prevPoint = currPoint;
        }
    }
}
