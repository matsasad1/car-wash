// Attach to your UI Image
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class HalftoneUIBinder : MonoBehaviour
{
    public Material mat;

    void Update()
    {
        if (!mat) return;
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 size = rt.rect.size;
        mat.SetVector("_RectSize", new Vector4(size.x, size.y, 0, 0));
    }
}
