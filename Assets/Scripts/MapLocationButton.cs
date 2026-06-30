using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapLocationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad;

    [Header("Animation")]
    [SerializeField] private bool closeMapWithAnimation = true;
    [SerializeField] private float mapCloseDelay = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private bool useColorHover = true;
    [SerializeField] private Color hoverColor = Color.yellow;
    private Color originalColor;

    [Header("Optional - Loading")]
    [SerializeField] private bool useLoadingTransition = false;
    [SerializeField] private float transitionDuration = 0.5f;

    private Button button;
    private Image image;
    private CanvasGroup canvasGroup;
    private bool isTransitioning;

    private void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        if (image != null)
        {
            originalColor = image.color;
            image.alphaHitTestMinimumThreshold = 0.1f;
            Debug.Log($"MapLocationButton: Alpha threshold set to 0.1 untuk {gameObject.name}");
        }

        if (useColorHover && image == null)
        {
            Debug.LogWarning($"MapLocationButton di {gameObject.name}: Image component TIDAK DITEMUKAN! Hover color tidak akan bekerja.");
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(GoToLocation);
            Debug.Log($"MapLocationButton setup untuk {gameObject.name}: akan load scene '{sceneToLoad}'");
        }
        else
        {
            Debug.LogWarning($"MapLocationButton di {gameObject.name}: Button component tidak ada. Hover tetap berfungsi tapi onClick mungkin perlu setup manual.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useColorHover && image != null)
        {
            image.color = hoverColor;
            Debug.Log($"Hover: {gameObject.name} color changed to {hoverColor}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useColorHover && image != null)
        {
            image.color = originalColor;
            Debug.Log($"Exit Hover: {gameObject.name} color changed to {originalColor}");
        }
    }

    public void GoToLocation()
    {
        if (isTransitioning)
        {
            return;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"ERROR: Scene name kosong di {gameObject.name}!");
            return;
        }

        Debug.Log($"MapLocationButton: Loading scene '{sceneToLoad}'...");

        if (!SceneExists(sceneToLoad))
        {
            Debug.LogError($"ERROR: Scene '{sceneToLoad}' TIDAK ADA di Build Settings!");
            return;
        }

        isTransitioning = true;

        if (closeMapWithAnimation && HUDOverlayManager.Instance != null)
        {
            HUDOverlayManager.Instance.LoadMapLocation(sceneToLoad, mapCloseDelay);
            return;
        }

        if (useLoadingTransition)
        {
            StartCoroutine(LoadSceneWithTransition());
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (name == sceneName)
                return true;
        }
        return false;
    }

    private System.Collections.IEnumerator LoadSceneWithTransition()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / transitionDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(GoToLocation);
        }
    }
}
