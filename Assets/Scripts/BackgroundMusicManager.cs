using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("Audio Source")]
    private AudioSource bgmAudioSource;

    [Header("Volume Settings")]
    [SerializeField]
    private float normalVolume = 0.5f;  
    [SerializeField]
    private float duckVolume = 0.15f;   
    [SerializeField]
    private float fadeDuration = 0.5f;  

    private Coroutine fadeCoroutine = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        bgmAudioSource = GetComponent<AudioSource>();
        if (bgmAudioSource == null)
        {
            Debug.LogError("BackgroundMusicManager: AudioSource tidak ditemukan!");
            return;
        }

        bgmAudioSource.volume = normalVolume;
        
        if (!bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }

        Debug.Log("Background Music Manager siap. Normal Volume: " + normalVolume);
    }

    public void DuckVolume()
    {
        if (bgmAudioSource == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeVolume(duckVolume));
        Debug.Log("Background Music: Duck (turun ke " + duckVolume + ")");
    }

    public void RestoreVolume()
    {
        if (bgmAudioSource == null) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeVolume(normalVolume));
        Debug.Log("Background Music: Restore (naik ke " + normalVolume + ")");
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = bgmAudioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            bgmAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        bgmAudioSource.volume = targetVolume;
        fadeCoroutine = null;
    }
}
