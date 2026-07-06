using UnityEngine;
using TMPro;

public class PopupController : MonoBehaviour
{
    public GameObject popup;
    public TMP_Text messageText;
    public CanvasGroup menuGroup;

    void Start()
    {
        popup.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        messageText.text = message;
        popup.SetActive(true);

        SetMenuInteractable(false);
    }

    public void HidePopup()
    {
        popup.SetActive(false);

        SetMenuInteractable(true);
    }

    void SetMenuInteractable(bool value)
    {
        if (menuGroup == null) return;

        menuGroup.interactable = value;
        menuGroup.blocksRaycasts = value;
        menuGroup.alpha = value ? 1f : 0.5f;
    }
}