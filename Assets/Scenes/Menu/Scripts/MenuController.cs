using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public PopupController popup;

    public void OnPlayPressed()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            popup.ShowPopup("Insert name");
            return;
        }

        SceneFlowManager.Instance.playerName = nameInput.text;

        // TODO: QuickMatch dopo
        Debug.Log("QuickMatch not implemented");
    }

    public void OnPartyPressed()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            popup.ShowPopup("Insert name");
            return;
        }

        SceneFlowManager.Instance.playerName = nameInput.text;
        SceneFlowManager.Instance.GoToLobby();
    }
}