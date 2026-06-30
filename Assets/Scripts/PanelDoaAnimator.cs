using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelDoaAnimator : MonoBehaviour
{
    [Header("References")]
    public GameObject panelRoot; 
    public CanvasGroup canvasGroup; 

    [Header("Animation")]
    public float showDuration = 0.6f;

    void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;
        if (canvasGroup == null)
        {
            canvasGroup = panelRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = panelRoot.AddComponent<CanvasGroup>();
        }

        panelRoot.SetActive(false);
    }

    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(DoShow());
    }

    IEnumerator DoShow()
    {
        panelRoot.SetActive(true);

        canvasGroup.alpha = 0f;
        float t = 0f;
        while (t < showDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / showDuration);
            canvasGroup.alpha = k;
            
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return null;
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(DoHide());
    }

    IEnumerator DoHide()
    {
        float dur = 0.25f;
        float t = 0f;
        float start = canvasGroup.alpha;
        while (t < dur)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, t / dur);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        panelRoot.SetActive(false);
    }
}
