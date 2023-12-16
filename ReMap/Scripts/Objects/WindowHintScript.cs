using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("ReMap/Window Hint", 0)]
public class WindowHintScript : MonoBehaviour
{
    public RectTransform rectTransform;
    public float HalfHeight = 64;
    public float HalfWidth = 72;
    public Vector3 Right = Vector3.zero;

    void OnDrawGizmos()
    {
        if (rectTransform != null) rectTransform.sizeDelta = new Vector2(HalfWidth * 2, HalfHeight * 2);

        Quaternion rotation = Quaternion.Euler(this.transform.eulerAngles);
        Right = rotation * Vector3.right;
        Right.x = Mathf.Round(Right.x * 1e6f) / 1e6f;
        Right.y = Mathf.Round(Right.y * 1e6f) / 1e6f;
        Right.z = Mathf.Round(Right.z * 1e6f) / 1e6f;
    }
}
