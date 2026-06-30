using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjekEasterEgg : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sequence Animator")]
    public EasterEggSequenceAnimator sequenceAnimator;

    [Header("Behavior")]
    public bool toggleOffOnSecondClick = true;

    [Header("Hover")]
    public bool useHoverColor = true;
    public Color hoverColor = Color.white;

    private Image targetImage;
    private Color originalColor;
    private bool isPointerOver;
    private bool hoverLocked;

    private void Awake()
    {
        targetImage = GetComponent<Image>();

        if (targetImage != null)
        {
            originalColor = targetImage.color;

            Sprite sprite = targetImage.sprite;
            if (sprite != null && sprite.texture != null && sprite.texture.isReadable)
            {
                targetImage.alphaHitTestMinimumThreshold = 0.1f;
            }
        }

        if (sequenceAnimator != null)
        {
            sequenceAnimator.SequenceFinished += HandleSequenceFinished;
        }
    }

    private void OnDestroy()
    {
        if (sequenceAnimator != null)
        {
            sequenceAnimator.SequenceFinished -= HandleSequenceFinished;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sequenceAnimator == null)
        {
            return;
        }

        if (toggleOffOnSecondClick && sequenceAnimator.IsPlaying())
        {
            sequenceAnimator.StopSequence();
            return;
        }

        hoverLocked = true;
        if (targetImage != null)
        {
            targetImage.color = originalColor;
        }

        sequenceAnimator.PlaySequence();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        if (!useHoverColor || targetImage == null || hoverLocked)
        {
            return;
        }

        targetImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        if (!useHoverColor || targetImage == null)
        {
            return;
        }

        targetImage.color = originalColor;
    }

    private void HandleSequenceFinished()
    {
        hoverLocked = false;

        if (targetImage == null || !useHoverColor)
        {
            return;
        }

        if (isPointerOver)
        {
            targetImage.color = hoverColor;
        }
        else
        {
            targetImage.color = originalColor;
        }
    }
}
