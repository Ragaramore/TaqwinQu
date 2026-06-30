using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjekInteraktif : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data Spesifik Objek Ini")]
    public DoaData dataDoa; 

    [Header("Efek Hover")]
    public bool gunakanWarnaHover = false;
    public Color warnaHover = Color.white;

    [Header("Referensi UI Pop-up")]
    public GameObject panelUI;
    public Text uiJudul;
    public Image uiArab;
    public Text uiLatin;
    public Text uiArti;

    private Image gambarObjek;
    private Color warnaAwal;

    void Start()
    {
        gambarObjek = GetComponent<Image>();
        if (gambarObjek != null)
        {
            warnaAwal = gambarObjek.color;
            gambarObjek.alphaHitTestMinimumThreshold = 0.1f;
        }

        if (panelUI != null)
        {
            panelUI.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gunakanWarnaHover && gambarObjek != null)
        {
            gambarObjek.color = warnaHover;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gunakanWarnaHover && gambarObjek != null)
        {
            gambarObjek.color = warnaAwal;
        }
    }

    public void TampilkanData()
    {
        if (dataDoa != null)
        {
            if (uiJudul != null)
            {
                uiJudul.text = dataDoa.namaDoa;
            }
            if (uiArab != null)
            {
                uiArab.sprite = dataDoa.gambarArab;
                uiArab.enabled = dataDoa.gambarArab != null;
            }
            if (uiLatin != null)
            {
                uiLatin.text = dataDoa.teksLatin;
            }
            if (uiArti != null)
            {
                uiArti.text = dataDoa.teksArti;
            }

            if (uiJudul == null || uiArti == null)
            {
                Debug.LogWarning("ObjekInteraktif di '" + gameObject.name + "' belum lengkap referensi UI. " +
                                 "Assign uiJudul dan uiArti di Inspector.");
            }

            var panelManager = DoaPanelManager.GetInstance();
            Debug.Log("DEBUG: panelManager ditemukan? " + (panelManager != null));
            if (panelManager != null)
            {
                panelManager.SetCurrentDoaAndPlay(dataDoa);
                Debug.Log("DEBUG: SetCurrentDoaAndPlay dipanggil untuk: " + dataDoa.namaDoa);
            }
            else
            {
                Debug.LogError("ERROR: DoaPanelManager tidak ditemukan di scene!");
            }

            if (panelUI != null)
            {
                PanelDoaAnimator animator = panelUI.GetComponent<PanelDoaAnimator>();
                if (animator != null)
                {
                    if (!panelUI.activeSelf)
                        panelUI.SetActive(true);
                    animator.Show();
                }
                else
                {
                    panelUI.SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogWarning("File Data Doa belum dimasukkan ke objek ini!");
        }
    }
}
