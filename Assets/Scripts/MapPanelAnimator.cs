using UnityEngine;
using System.Collections;

public class MapPanelAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapImageAnimator mapImageAnimator;

    [Header("Audio")]
    public AudioSource paperSFX;  
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    private void Awake()
    {
        if (mapImageAnimator == null)
        {
            mapImageAnimator = GetComponentInChildren<MapImageAnimator>();
        }
        
        gameObject.SetActive(false);
    }

    public void SlideDown()
    {
        gameObject.SetActive(true);

        if (paperSFX != null)
        {
            paperSFX.volume = sfxVolume;
            paperSFX.Play();
        }

        if (mapImageAnimator != null)
        {
            mapImageAnimator.SlideDown();
        }
    }

    public void SlideUp()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (mapImageAnimator != null)
        {
            mapImageAnimator.SlideUp();
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(HidePanelAfterDelay());
        }
    }

    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    public void Stop()
    {
        if (mapImageAnimator != null)
        {
            mapImageAnimator.Stop();
        }
        StopAllCoroutines();
    }
}
