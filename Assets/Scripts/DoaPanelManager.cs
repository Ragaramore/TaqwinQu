using UnityEngine;
using UnityEngine.UI;

public class DoaPanelManager : MonoBehaviour
{
    public static DoaPanelManager Instance { get; private set; }

    public static DoaPanelManager GetInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindObjectOfType<DoaPanelManager>();
        if (Instance != null)
        {
            return Instance;
        }

        var allManagers = Resources.FindObjectsOfTypeAll<DoaPanelManager>();
        foreach (var manager in allManagers)
        {
            if (manager != null && manager.gameObject.scene.IsValid() && manager.gameObject.scene.isLoaded)
            {
                Instance = manager;
                break;
            }
        }

        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("Panel")]
    [SerializeField]
    private GameObject doaPanel;  

    [Header("Buttons")]
    [SerializeField]
    private Button closeButton;   
    [SerializeField]
    private Button soundOnButton; 
    [SerializeField]
    private Button soundOffButton; 

    [Header("Visual Feedback (Optional)")]
    private DoaAudioManager audioManager;
    private float lastPlayCallTime = -10f;
    private DoaData currentSelectedDoa = null;  

    void Start()
    {
        audioManager = DoaAudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("DoaAudioManager tidak ditemukan di scene!");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        if (soundOnButton != null)
        {
            soundOnButton.onClick.AddListener(OnSoundOnClicked);
        }

        if (soundOffButton != null)
        {
            soundOffButton.onClick.AddListener(OnSoundOffClicked);
        }

        if (doaPanel != null)
        {
            doaPanel.SetActive(true);
        }

    }

    void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        if (soundOnButton != null)
        {
            soundOnButton.onClick.RemoveListener(OnSoundOnClicked);
        }

        if (soundOffButton != null)
        {
            soundOffButton.onClick.RemoveListener(OnSoundOffClicked);
        }
    }

    private void OnCloseButtonClicked()
    {
        OnCloseButtonPressed();
    }

    private void OnSoundOnClicked()
    {
        OnSoundOnButtonPressed();
    }

    private void OnSoundOffClicked()
    {
        OnSoundOffButtonPressed();
    }

    public void OnCloseButtonPressed()
    {
        if (doaPanel != null)
        {
            doaPanel.SetActive(false);
            Debug.Log("Panel doa ditutup");
            currentSelectedDoa = null;  
            
            if (audioManager != null)
            {
                audioManager.StopAudio();
            }
            
            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.RestoreVolume();
            }

            var hudManager = HUDOverlayManager.Instance;
            if (hudManager != null)
            {
                hudManager.ShowHUDButtons();
            }
        }
    }

    public void OnSoundOnButtonPressed()
    {
        if (Time.time - lastPlayCallTime < 0.15f)
        {
            Debug.Log("OnSoundOnButtonPressed diabaikan karena Play baru saja dipanggil");
            return;
        }

        if (audioManager == null)
        {
            Debug.LogError("DoaAudioManager tidak ditemukan!");
            return;
        }

        if (audioManager.IsPlaying())
        {
            audioManager.PauseAudio();
            Debug.Log("Audio di-pause");
            
            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.RestoreVolume();
            }
        }
        else
        {
            if (currentSelectedDoa != null)
            {
                if (audioManager.IsCurrentDoa(currentSelectedDoa) && audioManager.GetCurrentTime() > 0f && !audioManager.IsPlaying())
                {
                    audioManager.ResumeAudio();
                    Debug.Log("Audio di-resume dari posisi terakhir: " + audioManager.GetCurrentTime());
                    
                    if (BackgroundMusicManager.Instance != null)
                    {
                        BackgroundMusicManager.Instance.DuckVolume();
                    }
                }
                else
                {
                    audioManager.PlayDoaAudioByData(currentSelectedDoa);
                    lastPlayCallTime = Time.time;
                    Debug.Log("Audio di-mainkan untuk doa: " + currentSelectedDoa.namaDoa);

                    if (BackgroundMusicManager.Instance != null)
                    {
                        BackgroundMusicManager.Instance.DuckVolume();
                    }
                }
            }
            else
            {
                audioManager.ResumeAudio();
                
                if (BackgroundMusicManager.Instance != null)
                {
                    BackgroundMusicManager.Instance.DuckVolume();
                }
                
                Debug.LogWarning("Tidak ada doa yang dipilih. Klik InteractableObject terlebih dahulu!");
            }
        }
    }

    public void OnSoundOffButtonPressed()
    {
        if (audioManager != null)
        {
            audioManager.PauseAudio();
            
            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.RestoreVolume();
            }
        }
        Debug.Log("Tombol Sound Off diklik - audio di-pause");
    }

    public void PlayDoaByIndex(int doaIndex)
    {
        if (audioManager != null)
        {
            audioManager.StopAudio();
            audioManager.PlayDoaAudio(doaIndex);
            lastPlayCallTime = Time.time;
            
        }
    }

    public void PlayDoaByData(DoaData doaData)
    {
        if (audioManager != null)
        {
            currentSelectedDoa = doaData;  
            audioManager.StopAudio();
            audioManager.PlayDoaAudioByData(doaData);
            lastPlayCallTime = Time.time;
            Debug.Log("Mainkan doa: " + doaData.namaDoa);
        }
    }

    public void PlayDoaByName(string doaName)
    {
        if (audioManager != null)
        {
            audioManager.PlayDoaAudioByName(doaName);
            
        }
    }


    public void SetPanelActive(bool active)
    {
        if (doaPanel != null)
        {
            doaPanel.SetActive(active);
        }
    }

    public bool IsPanelActive()
    {
        return doaPanel != null && doaPanel.activeSelf;
    }

    public void SetCurrentDoaAndPlay(DoaData doaData)
    {
        if (doaData != null)
        {
            currentSelectedDoa = doaData;
            
            
            Debug.Log("Panel: Menampilkan doa '" + doaData.namaDoa + "'");

            var hudManager = HUDOverlayManager.Instance;
            if (hudManager != null)
            {
                hudManager.HideHUDButtons();
            }
        }
    }
}
