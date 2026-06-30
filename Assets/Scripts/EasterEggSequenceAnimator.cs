using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EasterEggSequenceAnimator : MonoBehaviour
{
    [Header("References")]
    public Image targetImage;

    [Header("Movement")]
    public bool useMovement = false;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public float movementDuration = 0.6f;

    [Header("Intro Animation")]
    public Sprite[] introFrames;
    public float introFrameRate = 0.08f;

    [Header("Loop Animation")]
    public Sprite[] loopFrames;
    public float loopFrameRate = 0.12f;

    [Header("State")]
    public bool playOnStart = false;
    [Tooltip("Jika true, sequence hanya akan berjalan ketika dipanggil manual (mis. lewat click).")]
    public bool requireManualTrigger = true;

    private Coroutine playRoutine;
    private bool isPlaying;
    private bool stopRequested;
    private Sprite originalSprite;
    private RectTransform targetRect;
    private Vector2 originalAnchoredPosition;

    public event Action SequenceFinished;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null)
        {
            originalSprite = targetImage.sprite;
            targetRect = targetImage.rectTransform;
        }
        else
        {
            targetRect = GetComponent<RectTransform>();
        }

        if (targetRect != null)
        {
            originalAnchoredPosition = targetRect.anchoredPosition;
        }
    }

    private void Start()
    {
        if (playOnStart && !requireManualTrigger)
        {
            PlaySequence();
        }
    }

    public void PlaySequence()
    {
        if (targetImage == null)
        {
            return;
        }

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        stopRequested = false;
        playRoutine = StartCoroutine(DoPlaySequence());
    }

    public void StopSequence()
    {
        stopRequested = true;

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        isPlaying = false;
        RestoreOriginalSprite();
        SequenceFinished?.Invoke();
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    private IEnumerator DoPlaySequence()
    {
        isPlaying = true;

        if (useMovement && targetRect != null)
        {
            targetRect.anchoredPosition = startPosition;
        }

        if (introFrames != null && introFrames.Length > 0)
        {
            yield return StartCoroutine(PlayFrames(introFrames, introFrameRate));
        }

        if (stopRequested)
        {
            Finish();
            yield break;
        }

        if (loopFrames == null || loopFrames.Length == 0)
        {
            if (useMovement && targetRect != null)
            {
                yield return StartCoroutine(MoveToPosition(endPosition, movementDuration));
            }

            Finish();
            yield break;
        }

        bool moveToEnd = true;
        while (!stopRequested)
        {
            if (useMovement && targetRect != null)
            {
                yield return StartCoroutine(MoveToPosition(moveToEnd ? endPosition : startPosition, movementDuration));
            }

            if (stopRequested)
            {
                break;
            }

            yield return StartCoroutine(PlayFrames(loopFrames, loopFrameRate));

            moveToEnd = !moveToEnd;
        }

        Finish();
    }

    private IEnumerator PlayFrames(Sprite[] frames, float frameRate)
    {
        for (int i = 0; i < frames.Length; i++)
        {
            if (stopRequested)
            {
                yield break;
            }

            if (frames[i] != null)
            {
                targetImage.sprite = frames[i];
            }

            yield return new WaitForSeconds(frameRate);
        }
    }

    private void Finish()
    {
        isPlaying = false;
        playRoutine = null;
        RestoreOriginalSprite();
        SequenceFinished?.Invoke();
    }

    private IEnumerator MoveToPosition(Vector2 destination, float duration)
    {
        if (targetRect == null)
        {
            yield break;
        }

        if (duration <= 0f)
        {
            targetRect.anchoredPosition = destination;
            yield break;
        }

        Vector2 start = targetRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration && !stopRequested)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            targetRect.anchoredPosition = Vector2.Lerp(start, destination, t);
            yield return null;
        }

        if (!stopRequested)
        {
            targetRect.anchoredPosition = destination;
        }
    }

    private void RestoreOriginalSprite()
    {
        if (targetImage != null && originalSprite != null)
        {
            targetImage.sprite = originalSprite;
        }

        if (targetRect != null)
        {
            targetRect.anchoredPosition = originalAnchoredPosition;
        }
    }
}
