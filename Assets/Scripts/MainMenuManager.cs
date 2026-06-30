using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private const string ResolutionPrefKey = "MainMenuResolutionIndex";
    private const string FullscreenPrefKey = "MainMenuFullscreen";

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Buttons (Optional Auto-Wiring)")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button soundOnButton;
    [SerializeField] private Button soundOffButton;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Progress")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text progressText;
    [SerializeField] private Text starsText;
    [SerializeField, Min(0)] private int expectedDoaCount = 0;

    [Header("Close Button Colors")]
    [SerializeField] private Color closeNormalColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color closeHoverColor = new Color(1f, 0.95f, 0.6f, 1f);

    private readonly List<Resolution> availableResolutions = new List<Resolution>();
    private readonly UnityEngine.Vector2Int[] allowedResolutions = new UnityEngine.Vector2Int[]
    {
        new UnityEngine.Vector2Int(1280, 720),
        new UnityEngine.Vector2Int(1600, 900),
        new UnityEngine.Vector2Int(1360, 768),
        new UnityEngine.Vector2Int(1920, 1080),
        new UnityEngine.Vector2Int(1920, 1200),
    };

    [Header("Button Hover")]
    [SerializeField] private bool applyHoverColor = true;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color hoverColor = new Color(1f, 0.95f, 0.6f, 1f);
    
    [Header("Audio Fade")]
    [Range(0f, 1f)] public float normalVolume = 0.6f;
    [Range(0f, 1f)] public float fadeInStartVolume = 0.1f;
    public float fadeInDuration = 3f;

    [Header("Scene")]
    public string gameSceneName = "Masjid";

    private void Awake()
    {
        AutoAssignButtonsIfMissing();

        if (applyHoverColor)
        {
            ApplyButtonColors(playButton);
            ApplyButtonColors(settingsButton);
            ApplyButtonColors(closeSettingsButton, closeNormalColor, closeHoverColor);
            ApplyButtonColors(exitButton);
            ApplyButtonColors(soundOnButton);
            ApplyButtonColors(soundOffButton);
        }

        WireButton(playButton, PlayGame);
        WireButton(settingsButton, OpenSettings);
        WireButton(closeSettingsButton, CloseSettings);
        WireButton(exitButton, QuitGame);
        WireButton(soundOnButton, PlaySound);
        WireButton(soundOffButton, StopSound);
    }

    private void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = fadeInStartVolume;
            audioSource.Play();
            StartCoroutine(FadeInAudio());
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        InitializeResolutionSettings();
        RefreshProgressDisplay();
    }

    private void OnDestroy()
    {
        UnwireButton(playButton, PlayGame);
        UnwireButton(settingsButton, OpenSettings);
        UnwireButton(closeSettingsButton, CloseSettings);
        UnwireButton(exitButton, QuitGame);
        UnwireButton(soundOnButton, PlaySound);
        UnwireButton(soundOffButton, StopSound);

        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(ApplyResolution);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(ApplyFullscreen);
        }
    }

    private void WireButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private void UnwireButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(action);
    }

    private void AutoAssignButtonsIfMissing()
    {
        Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include);

        if (playButton == null)
        {
            playButton = FindButtonByName(buttons, "play");
        }

        if (exitButton == null)
        {
            exitButton = FindButtonByName(buttons, "exit", "quit");
        }

        if (settingsButton == null)
        {
            settingsButton = FindButtonByName(buttons, "settings", "setting");
        }

        if (closeSettingsButton == null)
        {
            closeSettingsButton = FindButtonByName(buttons, "close settings", "closesettings", "close", "back", "return");
        }

        if (soundOnButton == null)
        {
            soundOnButton = FindButtonByName(buttons, "sound on", "soundon", "audio on", "audioon", "music on", "musicon");
        }

        if (soundOffButton == null)
        {
            soundOffButton = FindButtonByName(buttons, "sound off", "soundoff", "audio off", "audiooff", "music off", "musicoff");
        }
    }

    private Button FindButtonByName(Button[] buttons, params string[] keywords)
    {
        foreach (Button btn in buttons)
        {
            if (btn == null)
            {
                continue;
            }

            string n = btn.gameObject.name.ToLowerInvariant();
            for (int i = 0; i < keywords.Length; i++)
            {
                if (n.Contains(keywords[i]))
                {
                    return btn;
                }
            }
        }

        return null;
    }

    private void ApplyButtonColors(Button button)
    {
        if (button == null)
        {
            return;
        }

        ApplyButtonColors(button, normalColor, hoverColor);
    }

    private void ApplyButtonColors(Button button, Color normal, Color hover)
    {
        if (button == null)
        {
            return;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = hover;
        colors.pressedColor = normal;  
        colors.selectedColor = normal;
        colors.disabledColor = normal;
        colors.fadeDuration = 0.1f;  
        button.colors = colors;
    }

    private IEnumerator FadeInAudio()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Lerp(fadeInStartVolume, normalVolume, progress);
            }
            
            yield return null;
        }
        
        if (audioSource != null)
            audioSource.volume = normalVolume;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void InitializeResolutionSettings()
    {
        if (resolutionDropdown != null)
        {
            PopulateResolutionDropdown();

            int savedIndex = PlayerPrefs.GetInt(ResolutionPrefKey, GetCurrentResolutionIndex());
            savedIndex = Mathf.Clamp(savedIndex, 0, Mathf.Max(0, availableResolutions.Count - 1));

            resolutionDropdown.onValueChanged.RemoveListener(ApplyResolution);
            resolutionDropdown.SetValueWithoutNotify(savedIndex);
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
        }

        if (fullscreenToggle != null)
        {
            bool savedFullscreen = PlayerPrefs.GetInt(FullscreenPrefKey, Screen.fullScreen ? 1 : 0) == 1;
            fullscreenToggle.onValueChanged.RemoveListener(ApplyFullscreen);
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
            fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);

            if (resolutionDropdown == null)
            {
                Screen.fullScreen = savedFullscreen;
            }
        }
    }

    private void PopulateResolutionDropdown()
    {
        availableResolutions.Clear();

        for (int i = 0; i < allowedResolutions.Length; i++)
        {
            var v = allowedResolutions[i];
            Resolution r = new Resolution();
            r.width = v.x;
            r.height = v.y;
            #if UNITY_2023_2_OR_NEWER
            r.refreshRateRatio = Screen.currentResolution.refreshRateRatio;
            #else
            r.refreshRate = Screen.currentResolution.refreshRate;
            #endif
            availableResolutions.Add(r);
        }

        List<string> options = new List<string>(availableResolutions.Count);
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            options.Add(resolution.width + " x " + resolution.height);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
    }

    private int GetCurrentResolutionIndex()
    {
        Resolution current = Screen.currentResolution;

        int best = 0;
        long bestDiff = long.MaxValue;

        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            if (resolution.width == current.width && resolution.height == current.height)
            {
                return i;
            }

            long diff = System.Math.Abs((long)resolution.width * resolution.height - (long)current.width * current.height);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = i;
            }
        }

        return best;
    }

    public void ApplyResolution(int index)
    {
        if (availableResolutions.Count == 0)
        {
            return;
        }

        index = Mathf.Clamp(index, 0, availableResolutions.Count - 1);
        Resolution selectedResolution = availableResolutions[index];
        bool isFullscreen = fullscreenToggle != null ? fullscreenToggle.isOn : Screen.fullScreen;

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);
        PlayerPrefs.SetInt(ResolutionPrefKey, index);
        PlayerPrefs.Save();
    }

    public void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenPrefKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlaySound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RefreshProgressDisplay()
    {
        int totalDoaCount = DoaProgressTracker.GetRegisteredTotalDoaCount(expectedDoaCount);
        int completedDoaCount = DoaProgressTracker.GetCompletedStarCount();

        float percentage = 0f;
        if (totalDoaCount > 0)
        {
            percentage = (completedDoaCount / (float)totalDoaCount) * 100f;
        }

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 100f;
            progressSlider.SetValueWithoutNotify(percentage);
            progressSlider.interactable = false;
        }

        if (progressText != null)
        {
            if (totalDoaCount > 0)
            {
                progressText.text = percentage.ToString("0") + "%" + " (" + completedDoaCount + "/" + totalDoaCount + ")";
            }
            else
            {
                progressText.text = completedDoaCount + " bintang";
            }
        }

        if (starsText != null)
        {
            starsText.text = completedDoaCount + "⭐";
        }
    }
}
