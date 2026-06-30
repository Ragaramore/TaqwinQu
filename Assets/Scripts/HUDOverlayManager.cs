using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class HUDOverlayManager : MonoBehaviour
{
    public static HUDOverlayManager Instance { get; private set; }
    private const string VolumePrefsKey = "GameMasterVolume";
    private bool isLoadingMapLocation;

    [Header("Panels")]
    [SerializeField] private GameObject mapOverlayPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hudButtonsPanel;  
    private MapPanelAnimator mapAnimator;  

    [Header("Buttons")]
    [SerializeField] private Button mapButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeMapButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Button backToMainMenuButton;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 1f;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (mapButton != null)
        {
            mapButton.onClick.AddListener(OpenMap);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }

        if (closeMapButton != null)
        {
            closeMapButton.onClick.AddListener(CloseMap);
        }

        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.AddListener(CloseSettings);
        }

        if (backToMainMenuButton != null)
        {
            backToMainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }

    private void Start()
    {
        ApplyResponsiveLayout();

        if (mapOverlayPanel != null)
        {
            mapAnimator = mapOverlayPanel.GetComponent<MapPanelAnimator>();
            if (mapAnimator == null)
            {
                Debug.LogWarning("MapPanelAnimator tidak ditemukan di mapOverlayPanel!");
            }
        }

        float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, defaultVolume);
        ApplyVolume(savedVolume);

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(savedVolume);
            volumeSlider.onValueChanged.AddListener(ApplyVolume);
        }

        if (mapOverlayPanel != null)
        {
            mapOverlayPanel.SetActive(false);
        }
        ShowHUDButtons();
        CloseSettings();
    }

    private void ApplyResponsiveLayout()
    {
        StretchToParent(mapOverlayPanel);
        StretchToParent(settingsPanel);
        StretchToParent(hudButtonsPanel);
    }

    private static void StretchToParent(GameObject panelObject)
    {
        if (panelObject == null)
        {
            return;
        }

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    private void OnDestroy()
    {
        if (mapButton != null)
        {
            mapButton.onClick.RemoveListener(OpenMap);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OpenSettings);
        }

        if (closeMapButton != null)
        {
            closeMapButton.onClick.RemoveListener(CloseMap);
        }

        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.RemoveListener(CloseSettings);
        }

        if (backToMainMenuButton != null)
        {
            backToMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(ApplyVolume);
        }
    }

    public void OpenMap()
    {
        HideHUDButtons();

        if (mapAnimator != null)
        {
            mapAnimator.SlideDown();
        }
        else if (mapOverlayPanel != null)
        {
            mapOverlayPanel.SetActive(true);
        }
    }

    public void CloseMap()
    {
        if (mapAnimator != null)
        {
            mapAnimator.SlideUp();
        }
        else if (mapOverlayPanel != null)
        {
            mapOverlayPanel.SetActive(false);
        }

        ShowHUDButtons();
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

    public void ApplyVolume(float value)
    {
        float clampedValue = Mathf.Clamp01(value);
        AudioListener.volume = clampedValue;
        PlayerPrefs.SetFloat(VolumePrefsKey, clampedValue);
        PlayerPrefs.Save();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void LoadMapLocation(string sceneName, float closeAnimationDelay = 0.5f)
    {
        if (isLoadingMapLocation)
        {
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("ERROR: sceneName kosong di LoadMapLocation");
            return;
        }

        StartCoroutine(LoadMapLocationRoutine(sceneName, Mathf.Max(0f, closeAnimationDelay)));
    }

    public void HideHUDButtons()
    {
        if (hudButtonsPanel != null)
        {
            hudButtonsPanel.SetActive(false);
        }
    }

    public void ShowHUDButtons()
    {
        if (hudButtonsPanel != null)
        {
            hudButtonsPanel.SetActive(true);
        }
    }

    private IEnumerator LoadMapLocationRoutine(string sceneName, float closeAnimationDelay)
    {
        isLoadingMapLocation = true;

        HideHUDButtons();

        if (mapAnimator != null)
        {
            mapAnimator.SlideUp();
        }
        else if (mapOverlayPanel != null)
        {
            mapOverlayPanel.SetActive(false);
        }

        if (closeAnimationDelay > 0f)
        {
            yield return new WaitForSeconds(closeAnimationDelay);
        }

        SceneManager.LoadScene(sceneName);
    }
}
