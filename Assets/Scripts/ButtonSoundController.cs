using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundController : MonoBehaviour
{
    public enum ButtonType { SoundOn, SoundOff, Close }
    
    [Header("Tipe Button")]
    public ButtonType buttonType = ButtonType.SoundOn;

    private Button button;
    private DoaPanelManager panelManager;

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        panelManager = DoaPanelManager.GetInstance();
        if (panelManager == null)
        {
            Debug.LogError("DoaPanelManager tidak ditemukan di scene!");
        }
    }

    void OnButtonClick()
    {
        if (panelManager == null)
        {
            panelManager = DoaPanelManager.GetInstance();
        }

        if (panelManager == null) return;

        switch (buttonType)
        {
            case ButtonType.SoundOn:
                panelManager.OnSoundOnButtonPressed();
                Debug.Log("Button Sound On diklik");
                break;

            case ButtonType.SoundOff:
                panelManager.OnSoundOffButtonPressed();
                Debug.Log("Button Sound Off diklik");
                break;

            case ButtonType.Close:
                panelManager.OnCloseButtonPressed();
                Debug.Log("Button Close diklik");
                break;
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
