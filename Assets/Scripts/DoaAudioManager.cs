using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DoaAudioManager : MonoBehaviour
{
    public static DoaAudioManager Instance { get; private set; }
    private const float CompletionThreshold = 0.1f;

    [System.Serializable]
    public class DoaAudio
    {
        public DoaData doaData;       
        public AudioClip audioClip;   
    }

    [SerializeField]
    private List<DoaAudio> doaAudios = new List<DoaAudio>();

    [Header("UI Controls (Optional)")]
    [SerializeField] private Slider progressSlider;

    private AudioSource audioSource;
    private int currentDoaIndex = -1;
    private bool isSeeking;
    private bool resumeAfterSeek;
    private bool completionRecordedForCurrentDoa;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 

        DoaProgressTracker.RegisterTotalDoaCount(GetAvailableDoaCount());
        SetupSliders();
    }

    private void OnEnable()
    {
        if (progressSlider != null)
        {
            progressSlider.onValueChanged.AddListener(SeekByTime);
            BindSliderEvents();
        }
    }

    private void OnDisable()
    {
        if (progressSlider != null)
        {
            progressSlider.onValueChanged.RemoveListener(SeekByTime);
        }
    }

    private void BindSliderEvents()
    {
        if (progressSlider == null)
        {
            return;
        }

        EventTrigger eventTrigger = progressSlider.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = progressSlider.gameObject.AddComponent<EventTrigger>();
        }

        eventTrigger.triggers.Clear();

        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener(_ => BeginSeek());
        eventTrigger.triggers.Add(beginDragEntry);

        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener(_ => EndSeek());
        eventTrigger.triggers.Add(endDragEntry);

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener(_ => BeginSeek());
        eventTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener(_ => EndSeek());
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    void Update()
    {
        UpdateProgressSlider();
        CheckForDoaCompletion();
    }

    private int GetAvailableDoaCount()
    {
        int count = 0;

        for (int i = 0; i < doaAudios.Count; i++)
        {
            if (doaAudios[i].doaData != null)
            {
                count++;
            }
        }

        return count;
    }

    private void CheckForDoaCompletion()
    {
        if (completionRecordedForCurrentDoa)
        {
            return;
        }

        if (audioSource == null || audioSource.clip == null || audioSource.isPlaying || audioSource.loop)
        {
            return;
        }

        if (currentDoaIndex < 0 || currentDoaIndex >= doaAudios.Count)
        {
            return;
        }

        if (audioSource.time + CompletionThreshold < audioSource.clip.length)
        {
            return;
        }

        DoaData doaData = doaAudios[currentDoaIndex].doaData;
        if (doaData != null)
        {
            DoaProgressTracker.MarkDoaCompleted(doaData);
        }

        completionRecordedForCurrentDoa = true;
    }

    private void SetupSliders()
    {
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.wholeNumbers = false;
            progressSlider.SetValueWithoutNotify(0f);
        }

    }

    private void UpdateProgressSlider()
    {
        if (isSeeking || progressSlider == null || audioSource == null || audioSource.clip == null)
        {
            return;
        }

        if (audioSource.clip.length <= 0f)
        {
            progressSlider.SetValueWithoutNotify(0f);
            return;
        }

        progressSlider.maxValue = audioSource.clip.length;
        progressSlider.SetValueWithoutNotify(audioSource.time);
    }

    public void PlayDoaAudio(int doaIndex)
    {
        if (doaIndex < 0 || doaIndex >= doaAudios.Count)
        {
            Debug.LogWarning($"Doa index {doaIndex} tidak valid!");
            return;
        }

        currentDoaIndex = doaIndex;
        AudioClip clip = doaAudios[doaIndex].audioClip;
        
        if (clip != null)
        {
            completionRecordedForCurrentDoa = false;

            if (audioSource == null)
            {
                if (Camera.main != null)
                    AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
                else
                    AudioSource.PlayClipAtPoint(clip, Vector3.zero);
            }
            else
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.time = 0f;
                audioSource.Play();
            }

            if (progressSlider != null)
            {
                progressSlider.maxValue = clip.length;
                progressSlider.SetValueWithoutNotify(0f);
            }

            string doaName = doaAudios[doaIndex].doaData != null ? doaAudios[doaIndex].doaData.namaDoa : "Unknown";
            Debug.Log($"Playing: {doaName} (clip: {clip.name}, length: {clip.length:0.00}s)");
        }
        else
        {
            string doaName = doaAudios[doaIndex].doaData != null ? 
                doaAudios[doaIndex].doaData.namaDoa : "Unknown";
            Debug.LogWarning($"Audio untuk {doaName} belum di-assign!");
        }
    }

    public void PlayDoaAudioByData(DoaData doaData)
    {
        int index = doaAudios.FindIndex(d => d.doaData == doaData);
        if (index >= 0)
        {
            PlayDoaAudio(index);
        }
        else
        {
            Debug.LogWarning($"Doa '{doaData.namaDoa}' tidak ditemukan di audio list!");
        }
    }

    public void PlayDoaAudioByName(string doaName)
    {
        int index = doaAudios.FindIndex(d => d.doaData != null && d.doaData.namaDoa == doaName);
        if (index >= 0)
        {
            PlayDoaAudio(index);
        }
        else
        {
            Debug.LogWarning($"Doa dengan nama '{doaName}' tidak ditemukan!");
        }
    }

    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Audio stopped");
        }

        completionRecordedForCurrentDoa = false;
    }

    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log("Audio paused");
        }
    }

    public void ResumeAudio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("Audio resumed");
        }
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public string GetCurrentDoaName()
    {
        if (currentDoaIndex >= 0 && currentDoaIndex < doaAudios.Count)
        {
            if (doaAudios[currentDoaIndex].doaData != null)
            {
                return doaAudios[currentDoaIndex].doaData.namaDoa;
            }
        }
        return "Tidak ada";
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public void SeekByNormalized(float normalizedValue)
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return;
        }

        normalizedValue = Mathf.Clamp01(normalizedValue);
        audioSource.time = normalizedValue * audioSource.clip.length;

        if (progressSlider != null)
        {
            progressSlider.SetValueWithoutNotify(audioSource.time);
        }
    }

    public void SeekByTime(float timeSeconds)
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return;
        }

        SeekBySeconds(timeSeconds);
    }

    public void BeginSeek()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return;
        }

        if (isSeeking)
        {
            return;
        }

        resumeAfterSeek = audioSource.isPlaying;
        isSeeking = true;

        if (resumeAfterSeek)
        {
            audioSource.Pause();
        }
    }

    public void EndSeek()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            isSeeking = false;
            resumeAfterSeek = false;
            return;
        }

        if (!isSeeking)
        {
            return;
        }

        isSeeking = false;

        if (resumeAfterSeek)
        {
            audioSource.Play();
        }

        resumeAfterSeek = false;
    }

    public void SeekBySeconds(float seconds)
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return;
        }

        audioSource.time = Mathf.Clamp(seconds, 0f, audioSource.clip.length);
    }

    public void SkipSeconds(float deltaSeconds)
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return;
        }

        SeekBySeconds(audioSource.time + deltaSeconds);
    }

    public float GetCurrentTime()
    {
        if (audioSource == null)
        {
            return 0f;
        }

        return audioSource.time;
    }

    public float GetDuration()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return 0f;
        }

        return audioSource.clip.length;
    }

    public bool IsCurrentDoa(DoaData doaData)
    {
        if (doaData == null || doaAudios == null || doaAudios.Count == 0)
            return false;

        if (currentDoaIndex < 0 || currentDoaIndex >= doaAudios.Count)
            return false;

        return doaAudios[currentDoaIndex].doaData == doaData;
    }
}
