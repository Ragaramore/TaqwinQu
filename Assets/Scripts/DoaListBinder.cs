using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoaListBinder : MonoBehaviour
{
    [Tooltip("Buttons to bind to DoaAudioManager entries. Order matters.")]
    public Button[] buttons;

    void Start()
    {
        var mgr = DoaAudioManager.Instance;
        if (mgr == null)
        {
            Debug.LogError("DoaAudioManager not found in scene. Cannot bind buttons.");
            return;
        }

        int count = buttons != null ? buttons.Length : 0;
        for (int i = 0; i < count; i++)
        {
            int index = i;
            if (buttons[i] == null) continue;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                DoaAudioManager.Instance.PlayDoaAudio(index);
                var panel = GetComponent<DoaPanelManager>();
                if (panel != null && !panel.IsPanelActive()) panel.SetPanelActive(true);
            });

            var txt = buttons[i].GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = "Doa " + (index + 1);
            }
        }

        Debug.Log($"DoaListBinder: bound {count} buttons to DoaAudioManager entries.");
    }
}
