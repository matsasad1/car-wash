using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class BenDayUIBinder : MonoBehaviour
{
    [SerializeField] Image targetImage; // optional; will use this Image's material
    [SerializeField] string rectSizeProp = "_RectSize";

    RectTransform rt;
    Material mat;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (!targetImage) targetImage = GetComponent<Image>();
        if (targetImage) mat = targetImage.material;
    }

    void LateUpdate()
    {
        if (!mat || !rt) return;
        var size = rt.rect.size;
        if (size.x <= 0f || size.y <= 0f) return;
        mat.SetVector(rectSizeProp, new Vector4(size.y, size.x, 0, 0));
    }
}
