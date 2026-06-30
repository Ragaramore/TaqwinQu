using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ButtonClickFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale")]
    public float pressedScale = 0.92f;
    public float pressSpeed = 12f;

    [Header("Color")]
    public bool useColor = true;
    public Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    [Header("Optional Click Sound")]
    public AudioClip clickSound;

    private Vector3 originalScale;
    private Image targetImage;
    private Color originalColor;
    private Coroutine scaleCoroutine;

    void Awake()
    {
        originalScale = transform.localScale;
        targetImage = GetComponent<Image>();
        if (targetImage != null) originalColor = targetImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useColor && targetImage != null)
            targetImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useColor && targetImage != null)
            targetImage.color = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartScaleRoutine(originalScale * pressedScale);
        if (clickSound != null)
        {
            if (Camera.main != null)
                AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
            else
                AudioSource.PlayClipAtPoint(clickSound, Vector3.zero);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartScaleRoutine(originalScale);
    }

    void StartScaleRoutine(Vector3 target)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(target));
    }

    IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * pressSpeed);
            yield return null;
        }
        transform.localScale = target;
        scaleCoroutine = null;
    }
}
