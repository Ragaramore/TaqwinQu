using UnityEngine;

public class FloatingAnimator : MonoBehaviour
{
    [Header("Vertical Movement (Naik-Turun)")]
    [SerializeField] private float verticalSpeed = 1.5f;  
    [SerializeField] private float verticalDistance = 30f; 

    [Header("Horizontal Movement (Kiri-Kanan)")]
    [SerializeField] private float horizontalSpeed = 1f;  
    [SerializeField] private float horizontalDistance = 20f; 

    [Header("Animation Curve (Optional)")]
    [SerializeField] private AnimationCurve verticalCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve horizontalCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private float elapsedTime = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("FloatingAnimator harus attach ke UI element dengan RectTransform!");
            enabled = false;
            return;
        }

        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float verticalMovement = Mathf.Sin(elapsedTime * verticalSpeed * Mathf.PI) * verticalDistance;
        
        float horizontalMovement = Mathf.Sin((elapsedTime * horizontalSpeed * Mathf.PI) + (Mathf.PI * 0.5f)) * horizontalDistance;

        rectTransform.anchoredPosition = startPosition + new Vector2(horizontalMovement, verticalMovement);
    }

    public void SetSpeeds(float vSpeed, float hSpeed)
    {
        verticalSpeed = vSpeed;
        horizontalSpeed = hSpeed;
    }

    public void ResetAnimation()
    {
        elapsedTime = 0f;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPosition;
        }
    }

    public void PauseAnimation()
    {
        enabled = false;
    }

    public void ResumeAnimation()
    {
        enabled = true;
    }
}
