using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(Image))]
public class ExitMenu : NetworkBehaviour
{
    private Image buttonImage;

    [Header("Impostazioni Visive")]
    public Color normalColor = Color.white;
    public Color mutedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void ExitMenuButton()
    {
        if (NetworkClient.isConnected)
        {
            SceneManager.LoadScene("MainMenu");
            NetworkManager.singleton.StopClient();
        }

        
    }
}