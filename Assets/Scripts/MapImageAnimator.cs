using UnityEngine;
using System.Collections;

public class MapImageAnimator : MonoBehaviour
{
    [Header("Animation")]
    public float slideDownDuration = 0.6f;
    public float slideUpDuration = 0.4f;
    public AnimationCurve slideDownCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve slideUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPositionAbove;
    private Vector2 endPositionVisible;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        startPositionAbove = new Vector2(rectTransform.anchoredPosition.x, 
                                        rectTransform.rect.height);
        endPositionVisible = rectTransform.anchoredPosition;  
    }

    public void SlideDown()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(DoSlideDown());
    }

    public void SlideUp()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(DoSlideUp());
    }

    private IEnumerator DoSlideDown()
    {
        rectTransform.anchoredPosition = startPositionAbove;
        canvasGroup.alpha = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < slideDownDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / slideDownDuration);
            float curveValue = slideDownCurve.Evaluate(progress);

            rectTransform.anchoredPosition = Vector2.Lerp(startPositionAbove, endPositionVisible, curveValue);
            canvasGroup.alpha = curveValue;

            yield return null;
        }

        rectTransform.anchoredPosition = endPositionVisible;
        canvasGroup.alpha = 1f;
        animationCoroutine = null;
    }

    private IEnumerator DoSlideUp()
    {
        float elapsedTime = 0f;
        Vector2 currentPos = rectTransform.anchoredPosition;

        while (elapsedTime < slideUpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / slideUpDuration);
            float curveValue = slideUpCurve.Evaluate(progress);

            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, startPositionAbove, curveValue);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveValue);

            yield return null;
        }

        rectTransform.anchoredPosition = startPositionAbove;
        canvasGroup.alpha = 0f;
        animationCoroutine = null;
    }

    public void Stop()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }
}
