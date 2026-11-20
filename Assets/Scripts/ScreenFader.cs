using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fade a negro para VR. Usa un Canvas World Space que sigue a la cámara principal
/// (un Canvas Screen Space no se renderiza bien en el visor).
/// Lo crea el GameManager y vive con él entre escenas.
/// </summary>
public class ScreenFader : MonoBehaviour
{
    private const float Distance = 0.3f;     // metros delante de la cámara
    private const float CanvasMeters = 2f;   // tamaño del panel, de sobra para cubrir el FOV
    private const float CanvasPixels = 1000f;

    private Canvas canvas;
    private Image image;
    private Camera cam;
    private float alpha;

    public static ScreenFader Create(Transform parent)
    {
        GameObject go = new GameObject("ScreenFader");
        go.transform.SetParent(parent, false);
        return go.AddComponent<ScreenFader>();
    }

    private void Awake()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = short.MaxValue;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(CanvasPixels, CanvasPixels);
        canvasRect.localScale = Vector3.one * (CanvasMeters / CanvasPixels);

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(transform, false);

        image = imageObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;

        RectTransform imageRect = image.rectTransform;
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        canvas.enabled = false;
    }

    private void LateUpdate()
    {
        if (!canvas.enabled) return;

        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Transform camTransform = cam.transform;
        transform.SetPositionAndRotation(
            camTransform.position + camTransform.forward * Distance,
            camTransform.rotation);
    }

    public IEnumerator Fade(float target, float duration)
    {
        float start = alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(start, target, elapsed / duration));
            yield return null;
        }

        SetAlpha(target);
    }

    public void SetAlpha(float value)
    {
        alpha = Mathf.Clamp01(value);

        Color c = image.color;
        c.a = alpha;
        image.color = c;

        canvas.enabled = alpha > 0.001f;
    }
}
