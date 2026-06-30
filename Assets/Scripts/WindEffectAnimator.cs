using UnityEngine;

public class WindEffectAnimator : MonoBehaviour
{
    [Header("Wind Motion")]
    [SerializeField] private bool enableWind = true;
    [SerializeField] private float windSpeed = 0.9f;
    [SerializeField] private float horizontalStrength = 8f;
    [SerializeField] private float verticalStrength = 3f;
    [SerializeField] private float rotationStrength = 2.5f;

    [Header("Gust")]
    [SerializeField] private bool enableGust = true;
    [SerializeField] private float gustSpeed = 0.35f;
    [SerializeField] private float gustStrength = 1.4f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector3 startRotation;
    private Vector3 startScale;
    private float elapsedTime;
    private float seed;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("WindEffectAnimator harus attach ke UI element dengan RectTransform!");
            enabled = false;
            return;
        }

        startPosition = rectTransform.anchoredPosition;
        startRotation = rectTransform.localEulerAngles;
        startScale = rectTransform.localScale;
        seed = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        if (!enableWind)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        float windTime = elapsedTime * windSpeed + seed;
        float baseNoiseX = Mathf.PerlinNoise(seed, windTime) * 2f - 1f;
        float baseNoiseY = Mathf.PerlinNoise(seed + 13.37f, windTime * 0.8f) * 2f - 1f;
        float baseNoiseZ = Mathf.PerlinNoise(seed + 29.11f, windTime * 0.6f) * 2f - 1f;

        float gust = 1f;
        if (enableGust)
        {
            float gustWave = Mathf.Sin((elapsedTime * gustSpeed) + seed) * 0.5f + 0.5f;
            gust = Mathf.Lerp(1f, gustStrength, gustWave);
        }

        Vector2 windOffset = new Vector2(
            baseNoiseX * horizontalStrength * gust,
            baseNoiseY * verticalStrength * gust
        );

        float windTilt = baseNoiseZ * rotationStrength * gust;

        rectTransform.anchoredPosition = startPosition + windOffset;
        rectTransform.localRotation = Quaternion.Euler(startRotation + new Vector3(0f, 0f, windTilt));
        rectTransform.localScale = startScale * (1f + (baseNoiseY * 0.01f * gust));
    }

    public void SetWindStrength(float horizontal, float vertical, float rotation)
    {
        horizontalStrength = Mathf.Max(0f, horizontal);
        verticalStrength = Mathf.Max(0f, vertical);
        rotationStrength = Mathf.Max(0f, rotation);
    }

    public void SetWindSpeed(float speed)
    {
        windSpeed = Mathf.Max(0f, speed);
    }

    public void ResetAnimation()
    {
        elapsedTime = 0f;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPosition;
            rectTransform.localRotation = Quaternion.Euler(startRotation);
            rectTransform.localScale = startScale;
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
